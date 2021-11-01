using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class GM
{
    private static GM instance;

    public static GM Instance
    {
        get
        {
            if(instance== null)
            {
                instance = new GM();
            }
            return instance;
        }
    }
    public  ObjectPool<BaseAttTrackingProjectile> projectilePool = new ObjectPool<BaseAttTrackingProjectile>();
    public Unit player;
}
