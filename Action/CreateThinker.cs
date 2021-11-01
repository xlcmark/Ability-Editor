using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Action/CreateThinker")]

public class CreateThinker : BaseAction
{
    [ValueAuto]
    public value Duration;
    [ValueAuto]
    public value Internal;
    [Expandable(true)]
    public List<BaseAction> actions;

    public override void Apply(Transform owner, EventParameter e, Ability ability, Modifier modifier)
    {

        Thinker.instance.StartCoroutine(OnThinker(owner, e, ability, modifier));//呼叫mono來執行
    }

    public IEnumerator OnThinker(Transform owner,EventParameter e,Ability ability,Modifier modifier)
    {
        int i = Mathf.Clamp(ability.Level - 1, 0, Duration.values.Length - 1);
        float dur = Duration.values[i];
        int j = Mathf.Clamp(ability.Level - 1, 0, Internal.values.Length - 1);
        float _internal = Internal.values[j];
        while (dur>0)
        {
            dur -= _internal;
            yield return new WaitForSeconds(_internal);
            //doActions

                foreach (var act in actions)
                {
                    act.Apply(owner, e, ability, modifier);
                }
            
        }
    }

}//createThinker的target不用放東西
//與modifier的thinker不一樣的地方在modifier是直接啟動，這個action是透過事件啟動
