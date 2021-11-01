using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Action/DelayAction")]
public class DelayAction : BaseAction
{
    public float delayTime;
    [Expandable(true)]
    public List<BaseAction> actions;


    public override void Apply(Transform owner, EventParameter e, Ability ability, Modifier modifier)
    {
        Thinker.instance.StartCoroutine(delayTimer(owner, e, ability, modifier));
    }
    private IEnumerator delayTimer(Transform owner, EventParameter e, Ability ability, Modifier modifier)
    {
        ability.OnDurationTimer(delayTime);
        yield return new WaitForSeconds(delayTime);
        if (owner != null && ability != null)
        {
            for (int i = 0; i < actions.Count; i++)
            {
                actions[i].Apply(owner, e, ability, modifier);
            }
        }
    }
}
//此target不用填