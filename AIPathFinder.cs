using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPathFinder : MonoBehaviour
{
    Transform[] destinations;
    int desIndex;

    MovementController mc;

    private void Awake()
    {
        mc = GetComponent<MovementController>();
    }
    private void Update()
    {
        //check destination
        if (mc.target != null) return;
        CheckIndex(transform.position);
    }
    //when spwan unit 
    public void SetPath(Transform[] _destinations)
    {
        desIndex = 0;
        destinations = _destinations;
        SetDestination(destinations[0].position);
    }

    public void CheckIndex(Vector3 curPos)
    {
        if (desIndex == destinations.Length - 1) return;
        if ((curPos-destinations[desIndex].position).sqrMagnitude<1f)
        {
            desIndex=Mathf.Clamp( desIndex+1,0,destinations.Length-1);
            SetDestination(destinations[desIndex].position);
        }
    }

    private void SetDestination(Vector3 pos)
    {
        if (mc == null) return;
        mc.ChangeState(MovementController.ComandState.Attack, pos, null);
    }
}
