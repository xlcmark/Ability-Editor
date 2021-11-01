using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotSpring : MonoBehaviour
{
    public Unit.Team team;
    List<DamageSystem> units = new List<DamageSystem>();
    public float rad=10;
    public float heal=100;
    public Transform center;
    void Start()
    {
        StartCoroutine(Search());
        StartCoroutine(HealLoop());
    }
    IEnumerator Search()
    {
        while (true)
        {
            units.Clear();
            Collider[] cols = Physics.OverlapSphere(center.position, rad, LayerMask.GetMask("Unit"));
            if (cols.Length > 0)
            {
                for (int i = 0; i < cols.Length; i++)
                {
                    Unit unit = cols[i].GetComponent<Unit>();
                    if (unit!=null && unit.team == team && unit.type!=UnitType.Building)
                    {
                        DamageSystem ds = unit.GetComponent<DamageSystem>();
                        if (ds != null)
                            units.Add(ds);
                    }
                }
            }
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator HealLoop()
    {
        while (true) 
        {
            if (units.Count != 0)
            {
                for (int i = 0; i < units.Count; i++)
                {
                    if(Vector3.Distance(units[i].transform.position,center.position)<rad)
                        units[i].TakeHeal(heal, Heal.ValueType.Constant);
                }
            }
            yield return new WaitForSeconds(0.33f);
        }
    }
        

}
