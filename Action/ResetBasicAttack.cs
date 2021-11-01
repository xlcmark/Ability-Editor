using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Action/ResetBasicAttack")]
public class ResetBasicAttack : BaseAction
{
    public override void Apply(Transform owner, EventParameter e, Ability ability, Modifier modifier)
    {
        MovementController mc = ability.Owner.GetComponent<MovementController>();
        if (mc == null) return;
        mc.ResetBasicAttack();
    }
}
