using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Action/Heal")]
public class Heal : BaseAction
{
    public enum ValueType { Constant, MaxHpPersent}
    public ValueType valueType;
    [ValueAuto]
    public value Amount;

    public override void DoAction(Transform target, Ability ability, Modifier modifier)
    {
        DamageSystem ds = target.GetComponent<DamageSystem>();
        Unit unit = ability.Owner.GetComponent<Unit>();
        Unit tarUnit = target.GetComponent<Unit>();
        if(ds==null || unit == null || tarUnit==null) { Debug.Log("hadn't ds or unit"); return; }

        ds.TakeHeal(Amount.GetFinalValue(unit,tarUnit,ability.Level), valueType);
    }
}
