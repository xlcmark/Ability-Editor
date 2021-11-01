using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Action/ConditionPositionWithOwner")]
public class ConditionPositionWithOwner : BaseAction
{
    [Tooltip("小於某個距離，不用時設為0")]
    public float lessThanDistance;
    [Tooltip("打勾啟用")]
    public bool IsTargetFacingToOwner;
    [Tooltip("打勾啟用")]
    public bool IsOwnerFacingTarget;

    [Expandable(true)]
    public List<BaseAction> OnSuccessActions;
    [Expandable(true)]
    public List<BaseAction> OnFailActions;

    private bool ExtraCondition(Transform target,Transform owner)
    {
        Vector3 ownerPos = new Vector3(owner.position.x, 0, owner.position.z);
        Vector3 tarPos = new Vector3(target.position.x, 0, target.position.z);
        if (lessThanDistance!=0 && Vector3.Distance(ownerPos, tarPos) > lessThanDistance)
        {
            return false;
        }
        if (IsTargetFacingToOwner)
        {
            Vector3 vec = ownerPos-tarPos;
            if (Vector3.Angle(vec,target.forward)>90)
            {
                return false;
            }
        }
        if (IsOwnerFacingTarget)
        {
            Vector3 vec = tarPos - ownerPos;
            if(Vector3.Angle(vec, owner.forward) > 90)
            {
                return false;
            }
        }
        return true;
    }
    public override void Apply(Transform owner, EventParameter e, Ability ability, Modifier modifier)
    {
        if (abilityTarget == null)
        {
            return;
        }

        if (abilityTarget is SingleTarget)
        {
            switch (((SingleTarget)abilityTarget).target)
            {
                case SingleTarget.Target.Target:
                    if (e.target != null)
                    {
                        if (Condition(e.target, ability.Owner))
                        {
                            if (ExtraCondition(e.target, owner))
                            {
                                foreach (var act in OnSuccessActions)
                                {
                                    act.Apply(owner, e, ability, modifier);
                                }
                            }
                            else
                            {
                                foreach (var act in OnFailActions)
                                {
                                    act.Apply(owner, e, ability, modifier);
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("No e.target!!!");
                    }
                    break;
                case SingleTarget.Target.Caster:
                    if (Condition(ability.Owner, ability.Owner))
                    {
                        if (ExtraCondition(ability.Owner, owner))
                        {
                            foreach (var act in OnSuccessActions)
                            {
                                act.Apply(owner, e, ability, modifier);
                            }
                        }
                        else
                        {
                            foreach (var act in OnFailActions)
                            {
                                act.Apply(owner, e, ability, modifier);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        else if (abilityTarget is MultipleTarget)
        {
            Vector3 center = Vector3.zero;
            Vector3 dir = Vector3.zero;
            switch (((MultipleTarget)abilityTarget).center)
            {
                case MultipleTarget.Center.Owner:
                    if (owner != null)
                    {
                        center = owner.position;
                        dir = e.point - owner.position;
                        dir.y = 0;
                    }
                    else
                    {
                        Debug.Log("No owner!!!");
                    }
                    break;
                case MultipleTarget.Center.Target:
                    if (e.target != null)
                        center = e.target.position;
                    else
                    {
                        Debug.Log("No e.target!!!");
                    }
                    break;
                case MultipleTarget.Center.Point:
                    if (e.point != null)
                    {
                        center = e.point;
                    }
                    else
                    {
                        Debug.Log("No e.point!!!");
                    }
                    break;

                default:
                    break;
            }
            List<Collider> hitColliders = OverlapCollider(center, ((MultipleTarget)abilityTarget).Radius, dir);
            foreach (var col in hitColliders)
            {
                e.target = col.transform;//e的目標換成受檢驗的單一目標
                if (Condition(col.transform, ability.Owner))
                {
                    if (ExtraCondition(col.transform, owner))
                    {
                        foreach (var act in OnSuccessActions)
                        {
                            act.Apply(owner, e, ability, modifier);//action裡的target是已篩選好得目標
                        }
                    }
                    else
                    {
                        foreach (var act in OnFailActions)
                        {
                            act.Apply(owner, e, ability, modifier);
                        }
                    }
                }
            }
        }
        else if (abilityTarget is ModOwnersTarget)
        {
            List<Unit> TarUnits = ability.GetModOwners(((ModOwnersTarget)abilityTarget).ModName);
            if (TarUnits == null) return;
            for (int i = 0; i < TarUnits.Count; i++)
            {
                if (Condition(TarUnits[i].transform, ability.Owner))
                {
                    if (ExtraCondition(TarUnits[i].transform,owner))
                    {
                        foreach (var act in OnSuccessActions)
                        {
                            act.Apply(owner, e, ability, modifier);//action裡的target是已篩選好得目標
                        }
                    }
                    else
                    {
                        foreach (var act in OnFailActions)
                        {
                            act.Apply(owner, e, ability, modifier);
                        }
                    }
                }
            }
        }
    }
}
