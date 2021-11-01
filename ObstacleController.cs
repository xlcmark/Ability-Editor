using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshObstacle))]
public class ObstacleController : MonoBehaviour
{
    NavMeshAgent agent;
    NavMeshObstacle obstacle;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();
    }
    public void SetDestination(Vector3 pos)
    {
        if (agent.enabled == true)
        {
            agent.SetDestination(pos);
        }
        else
        {
            obstacle.enabled = false;
            StartCoroutine(DelaySetDestination(pos));
        }
    }
    IEnumerator DelaySetDestination(Vector3 pos)
    {
        yield return new WaitForSeconds(Time.deltaTime*2);
        if (obstacle.enabled == false)
        {
            agent.enabled = true;
            agent.SetDestination(pos);
        }
    }
    public void StopAndObstacle()
    {
        agent.enabled = false;
        obstacle.enabled = true;
    }
}
