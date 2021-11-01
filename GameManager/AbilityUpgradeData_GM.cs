using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUpgradeData_GM : MonoBehaviour
{
    public static AbilityUpgradeData_GM instance;
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    public int[] normalSkill=new int[5];
    public int[] BigSkill = new int[3];
}
