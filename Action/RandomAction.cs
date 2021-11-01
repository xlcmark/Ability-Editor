using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Action/RandomAction")]
public class RandomAction : BaseAction
{
    [ValueAuto]
    public value chance;
    [Expandable(true)]
    public List<BaseAction> OnSuccessActions;
    [Expandable(true)]
    public List<BaseAction> OnFailActions;

    public override void Apply(Transform owner, EventParameter e,Ability ability,Modifier modifier)
    {
        int level = Mathf.Clamp(ability.Level - 1, 0, chance.values.Length - 1);
        if (Random.Range(0, 100) <= chance.values[level])
        {
            //成功時
            foreach (var act in OnSuccessActions)
            {
                act.Apply(owner, e,ability,modifier);
            }
        }
        else
        {
            //失敗時
            foreach (var act in OnFailActions)
            {
                act.Apply(owner, e,ability,modifier);
            }

        }
    }
}
//random的target不用填