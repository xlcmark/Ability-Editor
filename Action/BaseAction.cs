using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : ScriptableObject
{
    [Expandable]
    public AbilityTarget abilityTarget;

    //參數： 事件擁有者，事件參數，擁有此行動的技能，擁有此行動的modifier
    public virtual void Apply(Transform owner,EventParameter e,Ability ability,Modifier modifier)//ability與modifier為擁有者
    {
        if (abilityTarget == null)
        {
            return;
        }

        #region 分類單體目標或多重目標

        if(abilityTarget is SingleTarget)
        {
            switch (((SingleTarget)abilityTarget).target)
            {
                case SingleTarget.Target.Owner:
                    if (owner != null && owner.gameObject.activeInHierarchy)
                    {
                        if(Condition(owner,ability.Owner))
                            DoAction(owner,ability,modifier);
                    }
                    else
                    {
                        Debug.Log("No Owner!!!");
                    }
                    break;
                case SingleTarget.Target.Target:
                    if (e!=null && e.target != null && e.target.gameObject.activeInHierarchy)
                    {
                        if (Condition(e.target, ability.Owner))
                        {
                            DoAction(e.target, ability, modifier);
                        }
                    }
                    else
                    {
                        Debug.Log("No e.target!!!");
                    }
                    break;
                case SingleTarget.Target.Point:
                    if (e.point != null)
                        DoAction(e.point, ability, modifier);
                    else
                    {
                        Debug.Log("No e.point!!!");
                    }
                    break;
                case SingleTarget.Target.Caster:
                    DoAction(ability.Owner, ability, modifier);
                    break;
                default:
                    break;
            }
        }
        else if(abilityTarget is MultipleTarget)
        {
            Vector3 center=Vector3.zero;
            Vector3 dir = Vector3.zero;
            switch (((MultipleTarget)abilityTarget).center)
            {
                case MultipleTarget.Center.Owner://指向技
                    if (owner != null && owner.gameObject.activeInHierarchy)
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
                    if (e.target != null && e.target.gameObject.activeInHierarchy)
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
                case MultipleTarget.Center.Caster:
                    DoAction(ability.Owner, ability, modifier);
                    break;
                default:
                    break;
            }
            List<Collider>hitColliders = OverlapCollider(center, ((MultipleTarget)abilityTarget).Radius, dir);
            foreach (var col in hitColliders)
            {
                if(Condition(col.transform, ability.Owner))
                    DoAction(col.transform, ability, modifier);
            }
        }
        else if(abilityTarget is ModOwnersTarget)
        {
            List<Unit> TarUnits = ability.GetModOwners(((ModOwnersTarget)abilityTarget).ModName);
            if (TarUnits == null) return;
            for (int i = 0; i < TarUnits.Count; i++)
            {
                if (Condition(TarUnits[i].transform, ability.Owner))
                {
                    DoAction(TarUnits[i].transform, ability, modifier);
                }
            }
        }

        #endregion

    }
    public virtual void DoAction(Transform target,Ability ability,Modifier modifier) { Debug.Log("Nothing in here"); }
    public virtual void DoAction(Vector3 point,Ability ability,Modifier modifier) { Debug.Log("Nothing in here"); }

    //條件
    protected bool Condition(Transform tar,Transform caster)
    {
        Unit unitTar = tar.GetComponent<Unit>();
        Unit unitCaster = caster.GetComponent<Unit>();
        if (unitTar != null && unitCaster!=null)
        {
            if (abilityTarget.team == UnitTeam.Enemy)
            {
                if (unitTar.team == unitCaster.team) return false;
            }
            else if (abilityTarget.team == UnitTeam.Friendly)
            {
                if (unitTar.team != unitCaster.team) return false;
            }

            if ((unitTar.type & abilityTarget.type) == 0)
                return false;
            if ((unitTar.flag!=0)&&((unitTar.flag & abilityTarget.flag) == 0))
                return false;

            return true;
        }
        return false;
    }
    //multiple collider
    protected List<Collider> OverlapCollider(Vector3 center,float rad,Vector3 dir)
    {
        List<Collider> hitColliders=new List<Collider>();
        switch (((MultipleTarget)abilityTarget).colliderType)
        {
            case MultipleTarget.ColliderType.Sector:

                //目標在夾角內
                foreach (var c in Physics.OverlapSphere(center, rad))
                {
                    Vector3 tarDir = c.transform.position - center;
                    tarDir.y = 0;
                    if (Vector3.Angle(tarDir, dir) <(float) ((MultipleTarget)abilityTarget).CircleAngle / 2)
                    {
                        hitColliders.Add(c);
                    }
                }

                break;
            case MultipleTarget.ColliderType.Circle:

                hitColliders.AddRange(Physics.OverlapCapsule(center,center+Vector3.up*10, rad)); 
                break;
            default:
                break;
        }
        return hitColliders;
    }
}

