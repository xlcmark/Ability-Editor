using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroDeathTimerData_GM : MonoBehaviour
{
    public static HeroDeathTimerData_GM instance;
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    public int[] deathTimer;
}
