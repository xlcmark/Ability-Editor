using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityTarget : ScriptableObject
{

    public UnitTeam team;
   

    public UnitType type;
   

    public UnitFlag flag;


}
[System.Flags]
public enum UnitTeam {  Enemy=1, Friendly=2 }
[System.Flags]
public enum UnitType { Solider=1, Hero=2, Building=4, Creep=8 }
[System.Flags]
public enum UnitFlag { Dead=1, Invisible=2, Invulnerable=4 }



