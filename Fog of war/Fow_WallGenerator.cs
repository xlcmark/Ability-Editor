using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fow_WallGenerator : MonoBehaviour
{
    public Texture2D map;
    public GameObject wallPrefab;
    public GameObject[] Brushes;
    public float delta;
    public Vector3 BeginPos { get { return CalculateBeginPos(); } }

    public void CreateWall()
    {
        if (transform.Find("root"))
        {
            DestroyImmediate(transform.Find("root").gameObject);
        }
        Transform rootGO = new GameObject("root").transform;
        rootGO.parent = transform;
        Transform wallGroup = new GameObject("wallGroup").transform;
        wallGroup.parent = rootGO;
        Transform brushGroup = new GameObject("brushGroup").transform;
        brushGroup.parent = rootGO;

        Vector3 beginPos = BeginPos;

        for (int i = 0; i < map.width; i++)
        {
            for (int j = 0; j < map.height; j++)
            {
                Color pixelColor = map.GetPixel(i, j);
                if (pixelColor.a == 0) continue;
                GameObject prefab;
                Transform parent;
                if(pixelColor == Color.black)
                {
                    prefab = wallPrefab;
                    parent = wallGroup;
                }
                else
                {
                    prefab = Brushes[(j+i) % Brushes.Length];
                    parent = brushGroup;
                }
                Vector3 pos = new Vector3(beginPos.x+ delta* (i+0.5f), 0,beginPos.z+delta *( 0.5f+j));
                GameObject clone= Instantiate(prefab, pos, Quaternion.identity, parent);
                clone.transform.localScale = delta * Vector3.one;
            }
        }
    }
    private Vector3 CalculateBeginPos()
    {
        float x = map.width * delta * 0.5f;
        float z = map.height * delta * 0.5f;
        return transform.position-new Vector3(x, 0, z);
    }
}
