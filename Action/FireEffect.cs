using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttachEffectPoint {unit, top, body, hands, weapon};
[CreateAssetMenu(menuName = "Action/FireEffect")]
public class FireEffect : BaseAction
{
    public enum Type
    {
        point,
        direction
    }
    public string vfxName;
    [Tooltip("點或向量")]
    public Type type;
    [Tooltip("Target選擇target時才有效")]
    public AttachEffectPoint attachEffectPoint;

    public override void DoAction(Transform target,Ability ability,Modifier modifier)
    {
        Vector3 dir = target.position - ability.Owner.position;
        dir.y = 0;
        Unit tarUnit = target.GetComponent<Unit>();
        if (tarUnit == null)
        {
            Debug.Log("no <Unit>");
            return;
        }
        switch (type)
        {
            case Type.point:
                tarUnit.AddVFX(vfxName, attachEffectPoint, Quaternion.identity);
                break;
            case Type.direction:
                tarUnit.AddVFX(vfxName, attachEffectPoint, Quaternion.LookRotation(dir));
                break;
            default:
                break;
        }
    }
    public override void DoAction(Vector3 point,Ability ability,Modifier modifier)
    {
        Vector3 dir = point - ability.Owner.position;
        dir.y = 0;
        switch (type)
        {
            case Type.point:
                GameEffectManager.instance.AddWorldEffect(vfxName, point, Quaternion.LookRotation(dir));
                break;
            case Type.direction:
                GameEffectManager.instance.AddWorldEffect(vfxName, ability.Owner.position, Quaternion.LookRotation(dir));
                break;
            default:
                break;
        }
    }
}
//caster與direction為無效組合