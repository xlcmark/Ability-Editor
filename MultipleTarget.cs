using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="Target/Mutiple")]
public class MultipleTarget : AbilityTarget
{

    public enum Center { Owner,Target,Point,Caster}
    public Center center;
    [System.Flags]
    public enum ColliderType { Circle=1,Square=2,Sector=4}
    [EnumFlags]
    public ColliderType colliderType;


    [ConditionHide("colliderType", (int)(ColliderType.Circle | ColliderType.Sector))]
    public int Radius;

    [ConditionHide("colliderType", (int)ColliderType.Sector)]
    public int CircleAngle;

    [ConditionHide("colliderType", (int)ColliderType.Square)]
    public int squareWidth;

    [ConditionHide("colliderType", (int)ColliderType.Square)]
    public int squareHeight;



}
