using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Action/ActOnTarget")]
public class ActOnTarget : BaseAction
{
    [Expandable(true)]
    public List<BaseAction> actions;
    public override void DoAction(Transform target,Ability ability,Modifier modifier)
    {
        foreach (var act in actions)
        {
            if (target == null || !target.gameObject.activeInHierarchy) break;
            act.DoAction(target,ability,modifier);
        }
    }
    public override void DoAction(Vector3 point, Ability ability, Modifier modifier)
    {
        foreach (var act in actions)
        {
            act.DoAction(point, ability, modifier);
        }
    }
}
//actions裡面的target不用填