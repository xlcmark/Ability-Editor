using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AbilitySystem : MonoBehaviour
{
    public int SkillPoint = 1;
    public Ability[] abilities = new Ability[5];

    public Ability[] Equips = new Ability[6];//裝備也用ability

    public List<Ability> ChangedAbilities;//被替換下來的技能

    private DamageSystem damageSystem;
    private Unit unit;

    public delegate void OnAbilityChanged(int i);
    public OnAbilityChanged OnEquipChanged;
    public OnAbilityChanged OnEquipConsumableChanged;
    public OnAbilityChanged OnSkillChanged;

    private void Awake()
    {
        unit = GetComponent<Unit>();
        unit.OnLockAbility += LockAllAbility;
        unit.OnUnlockAbility += UnlockAllAbility;
        damageSystem = GetComponent<DamageSystem>();
        damageSystem.OnInit += Initial;//在ds之後初始化
    }
    private void OnDisable()
    {
        damageSystem.OnInit -= Initial;
    }

    public void GetNewAility(Ability newAbility,int i)
    {
        abilities[i] = Instantiate(newAbility);
        if(abilities[i].Level>0 || abilities[i].IsLearned)
            abilities[i].Init(transform);//初始化
    }
    //初始化裝備欄
    public void InitNewEquip(Ability newAbility, int i)
    {
        Equips[i] = Instantiate(newAbility);
        Equips[i].Init(transform);//初始化
    }

    //裝備
    public bool AddNewEquip(Ability item)
    {
        //消耗品疊加
        if ((item.behavior & Ability.Behavior.Consumable) != 0)//是否為消好品
        {
            for (int i = 0; i < Equips.Length; i++)
            {
                if (Equips[i] != null && Equips[i].name == item.name && Equips[i].ConsumablesCount < Equips[i].MaxConsumablesCount)
                {
                    Equips[i].ConsumablesCount++;
                    OnEquipConsumableChanged?.Invoke(i);
                    return true;
                }
            }
        }
        //找空位放
        for (int i = 0; i < Equips.Length; i++)
        {
            if (Equips[i] == null)
            {
                item.Init(transform);//先初始化
                Equips[i] = item;
                OnEquipChanged?.Invoke(i);
                return true;
            }
        }
        return false;
    }

    //卸裝 或減少消耗品
    public bool RemoveEquip(Ability OldItem)
    {
        bool IsConsumable = ((OldItem.behavior & Ability.Behavior.Consumable) != 0);

        for (int i = 0; i < Equips.Length; i++)
        {
            if (Equips[i] == OldItem)
            {
                if (IsConsumable && OldItem.ConsumablesCount > 1)//若是消耗品，消耗品-1
                {
                    OldItem.ConsumablesCount--;
                    OnEquipConsumableChanged?.Invoke(i);
                }
                else
                {
                    Equips[i] = null;
                    OldItem.ResetAbility();//重置
                    Destroy(OldItem);
                    OnEquipChanged?.Invoke(i);
                }
                return true;
            }
        }
        return false;
     }

    private void Initial()
    {
        //技能
        for (int i = 0; i < abilities.Length; i++)
        {
            if(abilities[i]!=null)
                GetNewAility(abilities[i], i);
        }
        //裝備
        for (int i = 0; i < Equips.Length; i++)
        {
            if (Equips[i] != null)
            {
                InitNewEquip(Equips[i], i);
            }
        }

    }

    public void StartActiveAbility(Ability ability,EventParameter e)
    {
        if (ability!=null && ability.curState==Ability.AbilityState.Ready)
        {
            ability.ActiveAbility(e);
                //消耗品使用消耗
                if ((ability.behavior & Ability.Behavior.Consumable) != 0)
                {
                    RemoveEquip(ability);
                }
            
        }
    }

    public void UpgradeAbility(Ability ability)
    {
        ability.Level++;
        SkillPoint--;
        if (ability.Level == 1)
        {
            ability.Init(transform);//初始化
        }
    }

    public void ChangedAbility(Ability OldAbility,Ability newAbility,EventParameter e)
    {
        for (int i = 0; i < abilities.Length; i++)
        {
            if (abilities[i]?.AbilityName == OldAbility.AbilityName)
            {
                int level = abilities[i].Level;//取得舊技能等級

                if (!ChangedAbilities.Contains(abilities[i]))
                    ChangedAbilities.Add(abilities[i]);

                Ability abl = ChangedAbilities.Find(x => x.AbilityName == newAbility.AbilityName);
                //switch
                if (abl != null)
                {
                    abilities[i] = abl;
                }
                else
                {
                    GetNewAility(newAbility, i);
                }

                abilities[i].Level = level;//新技能同步舊技能等級

                if ((newAbility.behavior & Ability.Behavior.FollowPreAbility) != 0)//跟隨上一技能的e(李星q2)
                {
                    abilities[i].SetEventParameter(e);
                }

                OnSkillChanged?.Invoke(i);//通知button

                unit.CheckState();//檢查技能狀態

                abilities[i].SwitchedAbility();//觸發交換技能事件

                return;
            }
        }
    }
    #region lock ability
    public void LockAllAbility()
    {
        LockAbility();
        LockEquip();
    }
    private void LockAbility()
    {
        foreach (var ability in abilities)
        {
            if(ability!=null)
                ability.LockAbility();
        }
    }
    private void LockEquip()
    {
        foreach (var equip in Equips)
        {
            if (equip != null)
                equip.LockAbility();
        }
    }
    public void UnlockAllAbility()
    {
        UnlockAbility();
        UnlockEquip();
    }
    private void UnlockAbility()
    {
        foreach (var ability in abilities)
        {
            if (ability != null)
                ability.UnLockAbility();
        }
    }
    private void UnlockEquip()
    {
        foreach (var equip in Equips)
        {
            if (equip != null)
                equip.UnLockAbility();
        }
    }
    #endregion
}
