using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Action/TrackingProjectile")]

public class TrackingProjectile : BaseAction
{
    public GameObject projectile;
    [Expandable(true)]
    public List<BaseAction> OnHitActions;

    public override void DoAction(Transform target, Ability ability, Modifier modifier)
    {
        Vector3 spwanPoint= ability.Owner.position;
        if (ability.Owner.GetComponent<MovementController>()?.firePoint != null)
            spwanPoint = ability.Owner.GetComponent<MovementController>().firePoint.position;
        GameObject obj = Instantiate(projectile, spwanPoint, Quaternion.LookRotation(ability.Owner.forward));
        TrackingProjectileMove tpm = obj.GetComponent<TrackingProjectileMove>();
        if (tpm != null)
        {
            tpm.ability = ability;
            tpm.modifier = modifier;
            tpm.abilityTarget = abilityTarget;
            tpm.OnHitActions = OnHitActions;
            tpm.target = target;
        }
        else
        {
            Debug.Log("NO TrackingProjectileMove Component");
        }

    }
}
