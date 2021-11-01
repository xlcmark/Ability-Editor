using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Action/ApplyModifier")]
public class ApplyModifier : BaseAction
{

    [Expandable]
    public Modifier AddModifier;

    public override void DoAction(Transform target,Ability ability,Modifier modifier)
    {
        Unit targetUnit = target.GetComponent<Unit>();
        if(targetUnit!=null)
        {
           targetUnit.AddModifier(AddModifier, ability);
        }
    }
}
