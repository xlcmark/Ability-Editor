using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Action/LinearProjectile")]

public class LinearProjectile : BaseAction
{
    public GameObject projectile;
    [Expandable(true)]
    public List<BaseAction> OnHitActions;

    public float speed;
    public float MaxDistance;
    public bool IsGoPass;
    public LayerMask layerMask;

    public override void DoAction(Vector3 point, Ability ability, Modifier modifier)
    {
        Vector3 dir = point - ability.Owner.position;
        dir.y = 0;
        Vector3 spwanPoint = ability.Owner.position;
        if (ability.Owner.GetComponent<MovementController>()?.firePoint != null)
            spwanPoint = ability.Owner.GetComponent<MovementController>().firePoint.position;
        GameObject obj=Instantiate(projectile, spwanPoint, Quaternion.LookRotation(dir));
        LinearProjectileMove pm = obj.GetComponent<LinearProjectileMove>();
        if (pm != null)
        {
            pm.SetProjectile(ability, modifier, abilityTarget, OnHitActions, layerMask, speed, MaxDistance, IsGoPass, null);
        }
        else
        {
            Debug.Log("NO ProjectileMove Component");
        }

    }
}
