using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GameEffectInfo
{
    public string VfxName;
    public string filePath;
    public int repeatCount;
    public GameEffectInfo(string _name,string _path,int _count)
    {
        VfxName = _name;
        filePath = _path;
        repeatCount = _count;
    }
}
