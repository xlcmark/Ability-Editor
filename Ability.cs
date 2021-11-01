using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Ability : ScriptableObject
{
    [System.Flags]
    public enum Behavior
    {
        Passive = 1,
        NoTarget = 2,
        UnitTarget = 4,
        Point = 8,
        Channelled = 16,//持續
        Toggle = 32,//開關ex:蒙多W
        UseIndicator = 64,//使用技能指示計
        Consumable = 128,//消耗品
        Item = 256,//此技能為物品
        BigSkill = 512,//此技能為大招
        FollowPreAbility=1024,//跟隨上一part的對象 ex:李星q2
        Dont_ResumeMovement=2048,//技能使用完 不回上一動
        IgnoreSilence=4096,//禁言也可用
    }

    public Behavior behavior;

    public string AbilityName;
    public Sprite sprite;
    public enum AbilityAnimation { spell1,spell2,spell3,spell4,none};
    public AbilityAnimation abilityAnimation;
    [TextArea]
    [Tooltip("字串中加上<valueName>可替換成變數")]
    public string Description;

    public bool IsLearned;
    public int MaxLevel = 5;
    private int level = 0;//0級代表未學會
    public int Level//設定技能等級，決定變數的值
    {
        set
        {
            level = Mathf.Clamp(value, 0, MaxLevel);//限制技能等級
        }
        get { return level; }
    }
    [ValueAuto]
    public value CoolDown;

    [ValueAuto]
    public value spendMana;
    public float SpendMana
    {
        get
        {
            if (spendMana != null)
            {
                int num = Mathf.Clamp(Level, 0, spendMana.values.Length - 1);//避免超過value設定的長度
                return spendMana.values[num];
            }
            else
                return 0;
        }
    }
    [ConditionHide("behavior", (int)(Behavior.Point | Behavior.UnitTarget))]
    public float SpellRange;

    [ConditionHide("behavior", (int)Behavior.UseIndicator)]
    [Tooltip("如果有技能指示計，把相關的multipleTarget(相同的參數)抓進來，如果沒有創新的")]
    public MultipleTarget SkillIndicatorInfo;

    [ConditionHide("behavior", (int)Behavior.UnitTarget)]
    [Tooltip("拉取同樣邏輯的singleTarget")]
    public SingleTarget UnitTargetFilter;

    [ConditionHide("behavior", (int)Behavior.Consumable)]
    public int ConsumablesCount = 1;
    [ConditionHide("behavior", (int)Behavior.Consumable)]
    public int MaxConsumablesCount = 5;

    [ConditionHide("behavior", (int)Behavior.Item)]
    public int Price;

    [ConditionHide("behavior", (int)Behavior.Channelled)]
    public float ChannelTime;

    [ConditionHide("behavior", (int)Behavior.Toggle)]
    public float ToggleOnCoolDownTime;//開關冷卻時間
    [ConditionHide("behavior", (int)Behavior.Toggle)]
    public Sprite ToggleOffSprite;

    public float SpellNeedTime;//詠唱時間

    [HideInInspector]
    public Transform Owner;//擁有此能力的人

    [Expandable(true)]
    public List<AbilityEvent> abilityEvents;


    [Expandable(true)]
    public List<Modifier> passiveModifiers;

    [Expandable(true)]
    public List<value> ValueList;//變數，各等級變數的值，例如：各級傷害值  //將數值統一打在這，名稱用來辨識，再拉到各個變數區域，例如dodamage.value

    private DamageSystem ds;
    private Unit ownerUnit;
    private EventParameter eventParameter;
    public bool IsToggleOn { private set; get; }
    public bool IsLock { private set; get; }
    public Coroutine curCoro { private set; get; }

    public event System.Action<float,float> OnCDTimerUpdate;
    public event System.Action<float> OnSpell;
    public event System.Action<bool> OnLockChanged;
    public event System.Action<bool> OnNoManaChanged;
    public event System.Action<bool> OnToggleChanged;
    public event System.Action<float> OnDuration;//某些技能有持續時間Ex:李星q2，在delayaction裡觸發
    public event System.Action OnPreExe;//執行技能前

    public void Init(Transform _owner)//在abilitySystem中執行
    {
        Owner = _owner;
        //把被動技能加到擁有者
        ds = Owner.GetComponent<DamageSystem>();
        ownerUnit = Owner.GetComponent<Unit>();
        if (ownerUnit == null)
        {
            Debug.Log("Owner hasn't a compoment<Unit>!!!");
            return;
        }
        foreach (var pm in passiveModifiers)
        {
            ownerUnit.AddModifier(pm,this);
        }
        //初始state machine
        ChangedState(AbilityState.Ready);

        ds.OnCostMana += OnCheckCostMana;
        ds.OnRecoverMana += OnCheckRecoverMana;

        if ((behavior & Behavior.Item) != 0 || IsLearned)
        {
            Level = 1;
        }

        //初始化事件
        abilityEvents.Find(x => x.type == AbilityEvent.Type.OnAbilityInitial)?.OnTrigger(Owner, null, this);

    }
    //重置，將unit裡的modifier移除
    public void ResetAbility()
    {
        if (ownerUnit == null) return;

        for (int i = 0; i < passiveModifiers.Count; i++)
        {
            ownerUnit.RemoveModifier(passiveModifiers[i].ModifierName);
        }
        Owner = null;
        //退訂
        ds.OnCostMana -= OnCheckCostMana;
        ds.OnRecoverMana -= OnCheckRecoverMana;
    }
    //交換完技能觸發事件
    public void SwitchedAbility()
    {
        abilityEvents.Find(x => x.type == AbilityEvent.Type.OnAbilitySwitched)?.OnTrigger(Owner, null, this);
    }

    #region Use Ability

    public void ActiveAbility(EventParameter e)
    {
        if((behavior & Behavior.FollowPreAbility) == 0)//跟隨上一個技能不用覆蓋eventParameter
            eventParameter = e;
        if (SpendMana > ds.curMana) return;
        if (IsLock) return;
        if (curState == AbilityState.Ready)
            ChangedState(AbilityState.Spell);

    }
    public void SetEventParameter(EventParameter e)
    {
        eventParameter = e;
    }

    //詠唱時間
    private IEnumerator SpellTimer()
    {
        float timer = 0;
        float fillamount=0;
        while (timer < SpellNeedTime)
        {
            fillamount = timer / SpellNeedTime;
            OnSpell?.Invoke(fillamount);
            yield return new WaitForSeconds(.1f);
            timer += .1f;
        }
        //詠唱完
        OnPreExe?.Invoke();//讓movement先改狀態

        if((behavior & Behavior.Channelled) != 0)
        {
            Execute();//先執行
            ChangedState(AbilityState.Channell);//持續施法
        }
        else if ((behavior & Behavior.Toggle) != 0)
        {
            ToggleExecute();
            ChangedState(AbilityState.CoolDown);
        }
        else
        {
            Execute();
            ChangedState(AbilityState.CoolDown);
        }
    }
    private void Execute()
    {
        ds.CostMana((int)SpendMana);//耗魔

        //技能本身的事件
        abilityEvents.Find(x => x.type == AbilityEvent.Type.OnAbilityStart)?.OnTrigger(Owner, eventParameter, this);

        //使用技能的事件
        Owner.GetComponent<EventControler>().OnEvent(EventType.OnAbilityStart, eventParameter);
    }
    private void ToggleExecute()//開關類
    {
        if (IsToggleOn)
        {
            OnToggleOff();
        }
        else
        {
            ds.CostMana((int)SpendMana);//耗魔
            OnToggleOn();
        }
        IsToggleOn = !IsToggleOn;
        OnToggleChanged?.Invoke(IsToggleOn);
    }

    private IEnumerator ChannellTimer()
    {
        AbilityEvent ae=abilityEvents.Find(x => x.type == AbilityEvent.Type.OnChannel);
        float timer = 0;
        float interval = 0.2f;
        while (timer < ChannelTime)
        {
            yield return new WaitForSeconds(interval);
            timer += interval;
            ae?.OnTrigger(Owner, eventParameter, this);
        }
        //持續施法結束
        abilityEvents.Find(x => x.type == AbilityEvent.Type.OnChannelFinish)?.OnTrigger(Owner, eventParameter, this);

        ChangedState(AbilityState.CoolDown);
    }
    public void InteruptChannel()
    {
        if (curState != AbilityState.Channell) return;

        Thinker.instance.StopCoroutine(curCoro);
        //打斷
        abilityEvents.Find(x => x.type == AbilityEvent.Type.OnChannelInterrupted)?.OnTrigger(Owner, eventParameter, this);

        ChangedState(AbilityState.CoolDown);
    }

    private void OnToggleOn()
    {
        abilityEvents.Find(x => x.type == AbilityEvent.Type.OnToggleOn)?.OnTrigger(Owner, eventParameter, this);
    }
    private void OnToggleOff()
    {
        abilityEvents.Find(x => x.type == AbilityEvent.Type.OnToggleOff)?.OnTrigger(Owner, eventParameter, this);
    }


    private IEnumerator CDTimer()
    {
        float timer=0;
        int num = 0;
        if (CoolDown != null)
        {
            num = Mathf.Clamp(Level, 0, CoolDown.values.Length - 1);
            timer = CoolDown.values[num];
        }
        if ((behavior & Behavior.Toggle) != 0 && IsToggleOn)//二技冷卻時間
            timer = ToggleOnCoolDownTime;

        float fillamount=0;

        while (timer > 0)
        {
            fillamount = timer / CoolDown.values[num];
            OnCDTimerUpdate?.Invoke(fillamount,timer);
            yield return new WaitForSeconds(.1f);
            timer -= .1f;
        }
        //cd結束
        ChangedState(AbilityState.Ready);
    }

    public void OnDurationTimer(float Time)
    {
        OnDuration?.Invoke(Time);
    }

    #endregion
    #region state machine
    public enum AbilityState
    {
        NotLearned,
        Ready,
        Spell,
        CoolDown,
        NoMana,
        Lock,
        Channell,
    }
    public AbilityState curState { private set; get; }

    private void ChangedState(AbilityState newState)
    {
        ExitState();
        curState = newState;
        EnterState();
    }

    private void EnterState()
    {
        switch (curState)
        {
            case AbilityState.Ready:
                if (IsLock)//cd完進入ready時要判定
                {
                    ChangedState(AbilityState.Lock);
                    return;
                }

                OnCheckCostMana();//檢查有無魔力

                break;
            case AbilityState.Spell:
                curCoro=Thinker.instance.StartCoroutine(SpellTimer());

                break;
            case AbilityState.CoolDown:
                curCoro = Thinker.instance.StartCoroutine(CDTimer());

                break;
            case AbilityState.NoMana:
                OnNoManaChanged?.Invoke(true);
                break;
            case AbilityState.Lock:
                OnLockChanged?.Invoke(true);
                break;
            case AbilityState.Channell:
                curCoro=Thinker.instance.StartCoroutine(ChannellTimer());
                break;
            
            default:
                break;
        }
    }
    private void ExitState()
    {
        switch (curState)
        {
            case AbilityState.Ready:

                break;
            case AbilityState.Spell:
                OnSpell?.Invoke(1);//1代表結束
                break;
            case AbilityState.CoolDown:
                OnCDTimerUpdate?.Invoke(0, 0);//0代表結束
                break;
            case AbilityState.NoMana:
                OnNoManaChanged?.Invoke(false);
                break;
            case AbilityState.Lock:
                OnLockChanged?.Invoke(false);
                break;
            case AbilityState.Channell:

                break;
            default:
                break;
        }
    }
    //中斷技能的操作為lock下一行再unlock
    public void LockAbility()
    {
        if ((behavior & Behavior.IgnoreSilence) != 0) return;
        if (IsLock) return;
        IsLock = true;
        if (curState == AbilityState.CoolDown) return;

        if (curCoro != null)
            Thinker.instance.StopCoroutine(curCoro);

        if (curState == AbilityState.Spell || curState == AbilityState.Channell)
        {
            ChangedState(AbilityState.CoolDown);
        }
        else
        {
            ChangedState(AbilityState.Lock);
        }
    }
    public void UnLockAbility()
    {
        if (!IsLock) return;
        IsLock = false;

        if (curState == AbilityState.CoolDown) return;

        ChangedState(AbilityState.Ready);

    }
    private void OnCheckCostMana()
    {
        if (Level == 0) return;
        if (curState != AbilityState.Ready) return;
        if (SpendMana > ds.curMana)
            ChangedState(AbilityState.NoMana);
    }
    private void OnCheckRecoverMana()
    {
        if (Level == 0) return;
        if (curState != AbilityState.NoMana) return;
        if (SpendMana <= ds.curMana)
            ChangedState(AbilityState.Ready);
    }
    #endregion

    #region mod的擁有者
    private Dictionary<string, List<Unit>> modOwners = new Dictionary<string, List<Unit>>();
    public void AddModOwners(string _modName,Unit _unit)
    {
        List<Unit> owners = null;
        if (modOwners.TryGetValue(_modName, out owners))
        {
            owners.Add(_unit);
        }
        else
        {
            owners = new List<Unit>();
            owners.Add(_unit);
            modOwners.Add(_modName, owners);
        }
    }
    public void RemoveModOwners(string _modName,Unit _unit)
    {
        List<Unit> owners = null;
        if (modOwners.TryGetValue(_modName, out owners))
        {
            if (owners != null)
            {
                owners.Remove(_unit);
            }
        }
    }
    public List<Unit> GetModOwners(string _modName)
    {
        List<Unit> owners = null;
        modOwners.TryGetValue(_modName, out owners);
        return owners;
    }

    #endregion
}
