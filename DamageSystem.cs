using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DamageSystem : MonoBehaviour
{
    public Unit unit { private set; get; }
    public int MaxHP{private set;get; }
    public float curHP { get; private set; }
    public int MaxMana{ get; private set; }
    public float curMana { get; private set; }

    private int PhyArmor { get { return Mathf.RoundToInt(unit.PhyArmor.FinalValue); } }
    private int MagArmor { get { return Mathf.RoundToInt(unit.MagArmor.FinalValue); } }

    public class ShieldValue
    {
        public Modifier Source;
        public int Value;
        public ShieldValue(Modifier _source,int _value)
        {
            Source = _source;
            Value = _value;
        }
    }
    public List<ShieldValue> Shields=new List<ShieldValue>();

    public event Action OnInit;
    public event Action OnDamaged;
    public event Action OnHealed;
    public event Action OnCostMana;
    public event Action OnRecoverMana;
    public event Action OnMaxHpChanged;
    public event Action OnMaxMpChanged;
    public event Action OnShieldChanged;
    public event Action<int, DamageType> OnDamagedDetail;
    public event Action<int> OnHealDetail;
    public event Action OnReset;

    private EventControler ec;
    public DeathPerfab deadPrefab;
    private bool Isdead;

    private void Awake()
    {
        unit = GetComponent<Unit>();
        unit.OnInit += Init;
        unit.OnPropertyChanged += MaxHpChanged;
        unit.OnPropertyChanged += MaxMpChanged;
        unit.OnAddShield += Unit_OnAddShield;
        unit.OnRemoveShield += Unit_OnRemoveShield;
        unit.OnReset+= Unit_OnReset;

        if (deadPrefab != null)
        {
            deadPrefab= Instantiate(deadPrefab);
            deadPrefab.ClosePrefab();
        }
    }

    public void Init()
    {
        UpgradeSystem us = unit.GetComponent<UpgradeSystem>();
        if (us != null)
        {
            us.OnUpgraded += UpgradeSystem_OnUpgrade;
        }

        MaxHP = Mathf.RoundToInt(unit.Health.FinalValue);
        MaxMana = Mathf.RoundToInt(unit.Mana.FinalValue);
        curHP = MaxHP;
        curMana = MaxMana;
      
        ec = GetComponent<EventControler>();

        StartCoroutine(HPRegen());
        StartCoroutine(MPRegen());

        OnInit?.Invoke();
    }

    public void CostMana(int amount)
    {
        curMana = Mathf.Clamp(curMana - amount, 0, MaxMana);
        OnCostMana?.Invoke();
    }

    public void RecoverMana(float amount)
    {
        curMana = Mathf.Clamp(curMana + amount, 0, MaxMana);
        OnRecoverMana?.Invoke();
    }

    public int TakeDamage(float dmg,DoDamage.ValueType valueType,AttackInfo atkInfo)
    {
        int realDmg = 0;
        //傷害類型
        switch (valueType)
        {
            case DoDamage.ValueType.CurHpPersent:
                realDmg = Mathf.RoundToInt( dmg/100 * curHP);
                break;
            case DoDamage.ValueType.MaxHpPersent:
                realDmg= Mathf.RoundToInt(dmg/100 * MaxHP);
                break;
            case DoDamage.ValueType.MissHpPersent:
                realDmg = Mathf.RoundToInt(dmg / 100 * (MaxHP-curHP));
                break;
            default:
                realDmg = Mathf.RoundToInt(dmg);
                break;
        }
        //計算防禦
        switch (atkInfo.damageType)
        {
            case DamageType.Physics:
                realDmg = Mathf.RoundToInt((1-ArmorFormula(PhyArmor)) * realDmg);
                break;
            case DamageType.Magic:
                realDmg = Mathf.RoundToInt((1-ArmorFormula(MagArmor)) * realDmg);
                break;
            default:
                break;
        }
        //Debug.Log(name + "承受了" + realDmg + "的傷害");
        //護盾
        if (Shields.Count > 0)
        { 
            realDmg = ShieldCalculate(realDmg);
        }
        //扣血
        curHP = Mathf.Clamp(curHP - realDmg, 0, MaxHP);
        //UI事件
        OnDamaged?.Invoke();
        OnDamagedDetail?.Invoke(realDmg, atkInfo.damageType);
        //gameEvent
        EventParameter e=new EventParameter();
        e.target = atkInfo.SourceTransform;
        if (atkInfo.IsBaseAttack)//被普攻打到的事件
        {
            ec.OnEvent(EventType.OnAttacked, e);
        }
        ec.OnEvent(EventType.OnDamaged, e);//被攻擊事件
        if (realDmg >= 0)
        {
            ec.OnEvent(EventType.OnHurt, e);//受傷扣血事件
        }
        //死亡
        if (curHP <= 0 && !Isdead && unit.ImmuneDeath==0)//無死亡免疫
        {
            Dead(atkInfo);
        }
        return realDmg;

    }
    private float ArmorFormula(int amount)
    {
        return (float)amount / (100 + amount);
    }

    public void TakeHeal(float amount,Heal.ValueType valueType)
    {
        if (curHP == MaxHP) return;
        switch (valueType)
        {
            case Heal.ValueType.Constant:
                break;
            case Heal.ValueType.MaxHpPersent:
                amount=amount/100*MaxHP;
                break;
            default:
                break;
        }

        curHP = Mathf.Clamp(curHP + amount, 0, MaxHP);
        OnHealed?.Invoke();
        OnHealDetail?.Invoke(Mathf.RoundToInt( amount));
    }

    public float GetHealthNormalized()
    {
        return (float)curHP / MaxHP;
    }
    public float GetManaNormalized()
    {
        return (float)curMana / MaxMana;
    }

    private IEnumerator HPRegen()
    {
        while (true)
        {
            if(Mathf.RoundToInt( curHP)!=MaxHP)
                TakeHeal(unit.HealthRegen.FinalValue,Heal.ValueType.Constant);
            yield return new WaitForSeconds(1);
        }
    }
    private IEnumerator MPRegen()
    {
        while (true)
        {
            if(Mathf.RoundToInt(curMana)!=MaxMana)
                RecoverMana(unit.ManaRegen.FinalValue);
            yield return new WaitForSeconds(1);
        }
    }
    //最大生命值改變curhp也要改變
    private void MaxHpChanged()
    {
        if (MaxHP !=Mathf.RoundToInt(unit.Health.FinalValue))
        {
            int ExtendHP = Mathf.RoundToInt(unit.Health.FinalValue) - MaxHP;
            if (ExtendHP > 0)//增加要加，減少不用
            {
                curHP +=ExtendHP;
            }
            curHP = Mathf.Clamp(curHP, 0, unit.Health.FinalValue);
            MaxHP = Mathf.RoundToInt(unit.Health.FinalValue);
            OnMaxHpChanged?.Invoke();
        }
    }

    private void MaxMpChanged()
    {
        if (MaxMana != Mathf.RoundToInt(unit.Mana.FinalValue))
        {
            int ExtendMP = Mathf.RoundToInt(unit.Mana.FinalValue) - MaxMana;
            if (ExtendMP > 0)
            {
                curMana += ExtendMP;
            }
            curMana = Mathf.Clamp(curMana, 0, unit.Mana.FinalValue);
            MaxMana = Mathf.RoundToInt(unit.Mana.FinalValue);
            OnMaxMpChanged?.Invoke();
        }
    }

    void Unit_OnAddShield(Modifier mod, int value)
    {
        Shields.Add(new ShieldValue(mod, value));
        OnShieldChanged?.Invoke();
    }

    void Unit_OnRemoveShield(Modifier mod, int value)
    {
        for (int i = 0; i < Shields.Count; i++)
        {
            if (Shields[i].Source == mod)
            {
                Shields.RemoveAt(i);
            }
        }
        OnShieldChanged?.Invoke();
    }

    private int ShieldCalculate(int dmg)
    {
        for (int i = 0; i <Shields.Count; i++)
        {
            if (dmg >= Shields[i].Value)
            {
                dmg -= Shields[i].Value;
                unit.RemoveModifier(Shields[i].Source.ModifierName);
            }
            else
            {
                Shields[i].Value -= dmg;
                return 0;
            }
        }
        return dmg;
    }
    public int GetShieldAmount()
    {
        int amount = 0;
        for (int i = 0; i < Shields.Count; i++)
        {
            amount += Shields[i].Value;
        }
        return amount;
    }

    public void Dead(AttackInfo atkInfo)
    {
        Isdead = true;
        //給予擊殺者錢
        Unit tarUnit = atkInfo.SourceTransform.GetComponent<Unit>();
        if (tarUnit==HeroManager.instance.heroes[0].hero)//代表玩家
        {
            MoneySystem.instance.GainMoney(unit.killedMoney);
            MoneyNumFadeOut moneyNum= Instantiate(MoneySystem.instance.MoneyNumUIPrefab, transform.position+Vector3.up*3, Quaternion.identity);
            moneyNum.SetNumber(unit.killedMoney);
        }
        //經驗圈內獲得經驗
        List< UpgradeSystem> colUGSs=new List<UpgradeSystem>();

        foreach (var col in Physics.OverlapSphere(transform.position, UpgradeExpData_GM.instance.ExpRange, LayerMask.GetMask("Unit")))
        {
            UpgradeSystem colUGS = col.GetComponent<UpgradeSystem>();
            if (colUGS != null)
            {
                if (colUGS.unit.team != unit.team)
                {
                    colUGSs.Add(colUGS);
                }
            }
        }
        for (int i = 0; i < colUGSs.Count; i++)
        {
            colUGSs[i].GainExp(unit.killedExp / colUGSs.Count);//經驗平分
        }
        //anim
        if (deadPrefab != null)
        {
            deadPrefab.OpenPrefab();
            deadPrefab.transform.position = transform.position;
            deadPrefab.transform.rotation = transform.rotation;
        }
        //event
        if(ec!=null)
            ec.OnEvent(EventType.OnOwnerDeath, null);
        unit.Die();//讓unit觸發死亡事件

    }
    void UpgradeSystem_OnUpgrade()
    {
        MaxHpChanged();
        MaxMpChanged();
    }
    //復活時回滿血
    void Unit_OnReset()
    {
        Isdead = false;
        curMana = MaxMana;
        curHP = MaxHP;

        OnReset?.Invoke();
    }

}
