using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Action/TriggerAbility")]
public class TriggerAbility : BaseAction
{
    [Header("用法：此Action用在被動技能上，被GameEvent觸發後，由AbilityEvent發動。",order =1),Space(-10, order = 2),Header( "觸發的Action放在OnAbilityStart裡面。", order = 3)]
    [Range(0,4)]
    public int abilityNumber;


    public override void DoAction(Transform target, Ability ability, Modifier modifier)
    {
        AbilitySystem abilitySystem = ability.Owner.GetComponent<AbilitySystem>();
        MovementController mc = ability.Owner.GetComponent<MovementController>();
        if (modifier == null && ability == abilitySystem.abilities[abilityNumber]) return;//不能用abilityevent呼叫此action呼叫本身的ability 會造成死循環

        mc.ChangeState(MovementController.ComandState.SpellPrepare, Vector3.zero, target, abilitySystem.abilities[abilityNumber]);

    }
}
