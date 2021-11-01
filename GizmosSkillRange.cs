using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosSkillRange : MonoBehaviour
{
    public int range;
    public int angle;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, angle / 2, 0) * transform.forward*range);
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, -angle / 2, 0) * transform.forward*range);

    }
}
