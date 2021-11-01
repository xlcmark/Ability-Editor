using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AbilityEvent : ScriptableObject
{
    public enum Type
    {
        OnAbilityStart,
        OnChannel,
        OnChannelFinish,
        OnChannelInterrupted,
        OnToggleOn,
        OnToggleOff,
        OnAbilityInitial,
        OnAbilitySwitched,//換技能完
    }
    public Type type;
    [Expandable(true)]
    public List<BaseAction> actions;

    public void OnTrigger(Transform owner, EventParameter e, Ability ability)
    {
        for (int i = 0; i < actions.Count; i++)
        {
            actions[i].Apply(owner, e, ability, null);
        }
    }
}