using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonedUnit : Unit
{
    private float lifeTime;
    private Unit master;
    private float MaxDistance=20;

    IEnumerator LifeTimeBar()
    {
        while (lifeTime>0)
        {
            yield return new WaitForSeconds(.25f);
            lifeTime -= .25f;
            Guard();
        }
        //destroy
        Die();
    }
    public void SetSummonedUnit(float _lifeTime,Unit _master)
    {
        lifeTime = _lifeTime;
        master = _master;
        team = master.team;

        StartCoroutine(LifeTimeBar());
    }
    //超出範圍反回主人身邊
    private void Guard()
    {
        if(Vector3.Distance(master.transform.position, transform.position) > MaxDistance)
        {
            transform.position = master.transform.position;
        }
    }
}
