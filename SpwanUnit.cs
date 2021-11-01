using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwanUnit : MonoBehaviour
{
    [System.Serializable]
    public class SpwanPrefab
    {
        public GameObject prefab;
        public int count;
        public Transform spwanPoint;
        public List<GameObject> unitPool=new List<GameObject>();
    }
    public SpwanPrefab[] spwanPrefabs;

    public Transform[] destinations;

    public float beginTime;
    public float rate;

    private GameObject CreateUnit(SpwanPrefab _prefab)
    {
        GameObject unit=null;
        unit = Instantiate(_prefab.prefab);
        _prefab.unitPool.Add(unit);
        unit.SetActive(false);
        return unit;
    }
    private GameObject GetUnit(SpwanPrefab _prefab)
    {
        GameObject unit=null;
        for (int i = 0; i < _prefab.unitPool.Count; i++)
        {
            if (_prefab.unitPool[i].activeInHierarchy)
            {
                continue;
            }
            unit = _prefab.unitPool[i];
            break;
        }
        if (unit == null)
        {
            unit = CreateUnit(_prefab);
        }
        return unit;
    }

    private void Start()
    {
        for (int i = 0; i < spwanPrefabs.Length; i++)
        {
            for (int j = 0; j < spwanPrefabs[i].count*3; j++)
            {
                CreateUnit(spwanPrefabs[i]);
            }
        }
        StartCoroutine(BeginDelayTimer());
    }
    private IEnumerator BeginDelayTimer()
    {
        yield return new WaitForSeconds(beginTime);
        while(true)
        {
            Spwan();
            yield return new WaitForSeconds(rate);
        }
    }

    private void Spwan()
    {
        for (int i = 0; i < spwanPrefabs.Length; i++)
        {
            for (int j = 0; j < spwanPrefabs[i].count; j++)
            {
                GameObject clone = GetUnit(spwanPrefabs[i]);
                clone.transform.position = spwanPrefabs[i].spwanPoint.position;
                clone.transform.rotation = spwanPrefabs[i].spwanPoint.rotation;
                clone.SetActive(true);

                Unit unit = clone.GetComponent<Unit>();
                if (unit != null)
                {
                    unit.ResetUnit();
                }

                AIPathFinder aIPathFinder = clone.GetComponent<AIPathFinder>();
                if (aIPathFinder != null)
                {
                    aIPathFinder.SetPath(destinations);
                }
            }
        }
    }


}
