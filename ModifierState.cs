using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class ModifierState : ScriptableObject
{
    public enum StateType
    {
        Stun,
        Blind,
        Silence,
        Invisible,
        Invulnerable,//無敵
        MagicImmune,//魔免
        Airborne,//滯空
        ImmuneDeath,//免疫死亡
    }
    public StateType stateType;
}
