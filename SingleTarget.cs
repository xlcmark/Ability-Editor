using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Target/Single")]
public class SingleTarget : AbilityTarget
{
    public enum Target { Owner, Target, Point ,Caster}//owner為事件擁有者，caster為技能擁有者，兩者有時相同。
    public Target target;


}



