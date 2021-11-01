using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="Action/DoDamage")]
public class DoDamage : BaseAction
{
    public DamageType damageType;

    public enum ValueType { Constant,MaxHpPersent,CurHpPersent,MissHpPersent}
    public ValueType valueType;
    [ValueAuto]
    public value Damage;

    public override void DoAction(Transform target,Ability ability,Modifier modifier)
    {
        DamageSystem ds = target.GetComponent<DamageSystem>();
        Unit OwnerUnit = ability.Owner.GetComponent<Unit>();
        Unit tarUnit = target.GetComponent<Unit>();
        if (OwnerUnit == null || ds == null || tarUnit==null) { Debug.Log("hadn't ds or unit"); return; }

        AttackInfo attackInfo = new AttackInfo(damageType, ability.Owner, ability);

        int realDmg = ds.TakeDamage(Damage.GetFinalValue(OwnerUnit,tarUnit, ability.Level), valueType, attackInfo);

        SkillLifeSteal(realDmg, ability.Owner);
    }
    //技能吸血，抓取角色的數值，通常為0
    private void SkillLifeSteal(int dmg,Transform caster)
    {
        Unit unit = caster.GetComponent<Unit>();
        DamageSystem ds = caster.GetComponent<DamageSystem>();
        if (unit == null || ds == null) return;

        ds.TakeHeal(Mathf.RoundToInt(dmg * unit.SkillLifeSteal.FinalValue),Heal.ValueType.Constant);

    }

}
public enum DamageType { Physics, Magic, Pure }