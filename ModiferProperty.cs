using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class ModiferProperty : ScriptableObject
{
    public enum PropertyType
    {
        PhyArmor,
        PhyArmor_Percentage,
        MagArmor,
        MagArmor_Percentage,
        Health,
        Health_Percentage,
        HealthREGEN,
        AttackRange,
        AttackSpeed_Percentage,
        MoveSpeed,
        MoveSpeed_Percentage,
        Mana,
        Mana_Percentage,
        ManaREGEN,
        Attack,
        Attack_Percentage,
        CriticalStrike,
        LifeSteal,
        SkillLifeSteal,
        Shield,
    }
    public PropertyType propertyType;
    [ValueAuto]
    public value value;
}
