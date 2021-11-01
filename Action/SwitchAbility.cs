using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Action/SwitchAbility")]
public class SwitchAbility : BaseAction
{
    public Ability OldAbility;
    public Ability newAbility;

    public override void Apply(Transform owner, EventParameter e, Ability ability, Modifier modifier)
    {
        AbilitySystem abilitySystem = ability.Owner.GetComponent<AbilitySystem>();
        abilitySystem.ChangedAbility(OldAbility, newAbility, e);
    }
}
//此target不用填