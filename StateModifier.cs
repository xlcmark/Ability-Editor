using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateModifier 
{
    public enum Type { Constant,Percentage}
    public Type type;
    public readonly float Value;
    public readonly object Source;
    public StateModifier(float value,Type _type,object source)
    {
        Value = value;
        type = _type;
        Source = source;
    }
    public StateModifier(float value,Type _type) : this(value, _type, null) { }
}
