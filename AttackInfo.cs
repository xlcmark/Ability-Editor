using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackInfo 
{
    public readonly DamageType damageType;
    public readonly Transform SourceTransform;
    public readonly Ability SourceAbility;
    public readonly bool IsBaseAttack;

    public AttackInfo(DamageType _type,Transform _sourceTran,Ability _sourceAb,bool _isBaseAtk)
    {
        damageType = _type;
        SourceTransform = _sourceTran;
        SourceAbility = _sourceAb;
        IsBaseAttack = _isBaseAtk;
    }
    public AttackInfo(DamageType _type, Transform _sourceTran, Ability _sourceAb)//技能攻擊
    :this(_type, _sourceTran, _sourceAb,false) { }

    public AttackInfo(Transform _sourceTran)//基礎攻擊
    : this(DamageType.Physics, _sourceTran, null, true) { }
}
