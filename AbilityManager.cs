using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour
{
    #region instance
    public static AbilityManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        UIController.onUpdateUI += SetAbilitySystem;
    }
    #endregion

    public SkillButton_UI[] skillButtons;

    public SkillButton_UI[] equipSkillButton;

    private Ability actingAbility;

    public AbilitySystem abilitySystem { private set; get; }
    private MovementController mc;
    private UpgradeSystem us;

    private void Start()
    {
        CursorControler.instance.onLeftMouseButtonDown += OnMouseButtonDown;
    }
    private void OnDisable()
    {
        CursorControler.instance.onLeftMouseButtonDown -= OnMouseButtonDown;
    }

    //設定技能到按鈕
    public void SetAbilitySystem(Unit unit)
    {
        if (abilitySystem != null)//退訂
        {
            abilitySystem.OnEquipChanged -= SetEquipToButton;
            abilitySystem.OnSkillChanged -= SetSkillToButton;
            abilitySystem.OnEquipConsumableChanged -= ChangedEquipConsumableCount;
        }
        if (us != null)
        {
            us.OnUpgraded -= ShowUpgradeButton;
        }

        //新的abilitySystem

        abilitySystem = unit.GetComponent<AbilitySystem>();
        abilitySystem.OnEquipChanged += SetEquipToButton;//訂閱
        abilitySystem.OnSkillChanged += SetSkillToButton;
        abilitySystem.OnEquipConsumableChanged += ChangedEquipConsumableCount;
        mc=unit.GetComponent<MovementController>();
        us = unit.GetComponent<UpgradeSystem>();
        us.OnUpgraded += ShowUpgradeButton;
        //set ability
        for (int i = 0; i < abilitySystem.abilities.Length; i++)
        {
            SetSkillToButton(i);
        }
        //初始將equip全加到按鈕
        for (int i = 0; i < abilitySystem.Equips.Length; i++)
        {
            SetEquipToButton(i);
        }

        ShowUpgradeButton();//檢查是否有升級點數
     
    }
    private void SetSkillToButton(int i)
    {
        if (i > skillButtons.Length - 1) return;
        skillButtons[i].UpdateButtonInfo(abilitySystem.abilities[i]);
    }
    private void SetEquipToButton(int i)
    {
        if (i > equipSkillButton.Length - 1) return;
        //equip
        equipSkillButton[i].UpdateButtonInfo(abilitySystem.Equips[i]);
    }
    private void ChangedEquipConsumableCount(int i)//改變消耗品數量
    {
        if (i > equipSkillButton.Length - 1) return;
        equipSkillButton[i].ConsumableDisplay();
    }

    //由按鈕啟動
    public void ActiveAbility(Ability ability)
    {
        actingAbility = ability;
        if (ability == null)
            return;
                   

        if ((ability.behavior & Ability.Behavior.Passive) != 0)
        {
            return;
        }
        else if ((ability.behavior & Ability.Behavior.NoTarget) != 0)
        {
            CursorControler.instance.ChangeMode(CursorControler.CursorSelectMode.Normal);
            OnMouseButtonDown(Vector3.zero, null);
        }
        else if((ability.behavior & Ability.Behavior.FollowPreAbility) != 0)
        {
            CursorControler.instance.ChangeMode(CursorControler.CursorSelectMode.Normal);
            OnMouseButtonDown(Vector3.zero, null);
        }
        else if ((ability.behavior & Ability.Behavior.UnitTarget) != 0)
        {
            CursorControler.instance.TargetFilter = ability.UnitTargetFilter;//對像過濾器
            CursorControler.instance.ChangeMode(CursorControler.CursorSelectMode.UnitTarget);
        }
        else if ((ability.behavior & Ability.Behavior.Point) != 0)
        {
            CursorControler.instance.ChangeMode(CursorControler.CursorSelectMode.Point);
        }
        //有無技能指示計
        if(mc.CurState!=MovementController.ComandState.Uncontroller && (ability.behavior & Ability.Behavior.UseIndicator) !=0)
        {
            SkillIndicator.instance.EnterSkillIndicatorInfo(ability);
            if((ability.behavior & Ability.Behavior.Point) != 0)//有技能指示計但無游標，例如範圍圓
            {
                if(ability.SkillIndicatorInfo.colliderType==MultipleTarget.ColliderType.Circle)
                    CursorControler.instance.ChangeMode(CursorControler.CursorSelectMode.none);
            }
        }
        else
        {
            SkillIndicator.instance.CancelSkillIndicator();
        }


    }
    //確定按下
    private void OnMouseButtonDown(Vector3 point,Transform target)
    {
        mc.ChangeState(MovementController.ComandState.SpellPrepare, point, target, actingAbility);
    }
    #region Ability Upgrade
    public void UpgradeAbility(Ability ability)
    {
        abilitySystem.UpgradeAbility(ability);

        if (abilitySystem.SkillPoint > 0)
            ShowUpgradeButton();
        else
            CloseUpgradeButton();
    }
    private void ShowUpgradeButton()
    {
        if (abilitySystem.SkillPoint == 0) return;
        for (int i = 0; i < skillButtons.Length; i++)
        {
            Ability a = skillButtons[i].ability;
            if (a == null) continue;
            int requireLevel = (a.behavior & Ability.Behavior.BigSkill) != 0 ?
            AbilityUpgradeData_GM.instance.BigSkill[a.Level] : AbilityUpgradeData_GM.instance.normalSkill[a.Level];
            if (us.Level >= requireLevel && a.Level<a.MaxLevel)
            {
                skillButtons[i].ShowUpgradeButton();
            }
            else
            {
                skillButtons[i].CloseUpgradeButton();
            }
        }
    }
    private void CloseUpgradeButton()
    {
        for (int i = 0; i < skillButtons.Length; i++)
        {
            skillButtons[i].CloseUpgradeButton();
        }
    }

    #endregion
}
