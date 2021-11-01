using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class GameEvent:ScriptableObject
{
    [HideInInspector]
    public Ability RefAbility;//擁有者
    [HideInInspector]
    public Modifier RefModifier;//擁有者


    public EventType eventType;

    [Expandable(true)]
    public List<BaseAction> actions;

    public void OnTrigger(Transform owner,EventParameter e)
    {
        for (int i = 0; i <actions.Count; i++)
        {
            actions[i].Apply(owner,e,RefAbility,RefModifier);
        }
    }


}
