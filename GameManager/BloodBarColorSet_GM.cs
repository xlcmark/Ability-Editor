using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodBarColorSet_GM : MonoBehaviour
{
    public static BloodBarColorSet_GM instance;

    public BloodBarEffectFallDown bloodBarEffect;

    private void Awake()
    {
        if (instance == null)
            instance = this;
                       
    }

    public Color friendlyPlane;
    public Color friendlyBloodBar;
    public Color friendlyBloodEff;
    public Color manaBar;
    public Color enemyPlane;
    public Color enemyBloodBar;
    public Color enemyBloodEff;
}
