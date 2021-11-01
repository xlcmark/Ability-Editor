using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Action/RemoveModifier")]
public class RemoveModifier : BaseAction
{
    public string RemoveModiferName;
    public override void DoAction(Transform target,Ability ability,Modifier modifier)
    {
        Unit targetUnit = target.GetComponent<Unit>();
        if (targetUnit != null)
        {
            targetUnit.RemoveModifier(RemoveModiferName);
        }
    }
}
