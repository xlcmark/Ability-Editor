using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPerfab : MonoBehaviour
{
    public void ClosePrefab()
    {
        gameObject.SetActive(false);
    }
    public void OpenPrefab()
    {
        gameObject.SetActive(true);
        Invoke("ClosePrefab", 1.5f);
    }

}
