using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeExpData_GM : MonoBehaviour
{
    public static UpgradeExpData_GM instance;
    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public int[] UpgradeExp;
    public float ExpRange;//獲得經驗圈
    public  ParticleSystem UpgradeFx;
}
