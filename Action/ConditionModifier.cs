using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Action/ConditionModifier")]
public class ConditionModifier : BaseAction//先不用
{
    public string ModifierName;
    public int NeedLayer = 1;//modifier層數
    [Expandable(true)]
    public List<BaseAction> AcessActions;
    [Expandable(true)]
    public List<BaseAction> FailActions;

    private bool ExtraCondition(Transform tar)
    {
        Unit unitTar = tar.GetComponent<Unit>();
        if (unitTar != null)
        {
            Modifier mod = unitTar.unitModifiers.Find(x => x.ModifierName == ModifierName);
            if (mod != null)
            {
                if (mod.OverlayCount >= NeedLayer)
                {
                    return true;
                }
            }
        }
        return false;
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
                case SingleTarget.Target.Owner:
                    if (owner != null)
                    {
                        if (Condition(owner, ability.Owner))
                        {
                            if (ExtraCondition(owner))
                            {
                                foreach (var act in AcessActions)
                                {
                                    act.Apply(owner, e, ability, modifier);
                                }
                            }
                            else
                            {
                                foreach (var act in FailActions)
                                {
                                    act.Apply(owner, e, ability, modifier);
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("No Owner!!!");
                    }
                    break;
                case SingleTarget.Target.Target:
                    if (e.target != null)
                    {
                        if (Condition(e.target, ability.Owner))
                        {
                            if (ExtraCondition(e.target))
                            {
                                foreach (var act in AcessActions)
                                {
                                    act.Apply(owner, e, ability, modifier);
                                }
                            }
                            else
                            {
                                foreach (var act in FailActions)
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
                        if (ExtraCondition(ability.Owner))
                        {
                            foreach (var act in AcessActions)
                            {
                                act.Apply(owner, e, ability, modifier);
                            }
                        }
                        else
                        {
                            foreach (var act in FailActions)
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
        if (abilityTarget is MultipleTarget)
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
                    if (ExtraCondition(col.transform))
                    {
                        foreach (var act in AcessActions)
                        {
                            act.Apply(owner, e, ability, modifier);//action裡的target是已篩選好得目標
                        }
                    }
                    else
                    {
                        foreach (var act in FailActions)
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
                    if (ExtraCondition(TarUnits[i].transform))
                    {
                        foreach (var act in AcessActions)
                        {
                            act.Apply(owner, e, ability, modifier);//action裡的target是已篩選好得目標
                        }
                    }
                    else
                    {
                        foreach (var act in FailActions)
                        {
                            act.Apply(owner, e, ability, modifier);
                        }
                    }
                }
            }
        }
    }
}
