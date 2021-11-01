using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;
[System.Serializable]
public class CharacterState 
{
    public string StateName { private set; get; }
    public float BaseValue { private set; get; }
    private List<StateModifier> stateModifiers = new List<StateModifier>();
    public float BonusValue { get { return CalculateBonusValue(); } }
    public float FinalValue
    {
        get
        {
            if (IsDirty)
            {
                _finalValue = BaseValue + BonusValue;
                IsDirty = false;
            }
            return _finalValue;
        }
    }
    bool IsDirty=true;
    private float _finalValue;

    private float CalculateBonusValue()
    {
        float constantValue=0;
        float percentageValue = 0;

        for (int i = 0; i < stateModifiers.Count; i++)
        {
            switch (stateModifiers[i].type)
            {
                case StateModifier.Type.Constant:
                    constantValue += stateModifiers[i].Value;
                    break;
                case StateModifier.Type.Percentage:
                    percentageValue += stateModifiers[i].Value;
                    break;
                default:
                    break;
            }
        }

        return  constantValue + (BaseValue + constantValue) * (percentageValue / 100);
    }

    public void AddModifier(StateModifier mod)
    {
        IsDirty = true;
        stateModifiers.Add(mod);
    }
    public void RemoveModifier(StateModifier mod)
    {
        IsDirty = true;
        stateModifiers.Remove(mod);
    }
    public void RemoveAllModidiferFromSource(object source)
    {
        for (int i = 0; i < stateModifiers.Count; i++)
        {
            if (stateModifiers[i].Source == source)
            {
                RemoveModifier(stateModifiers[i]);
            }
        }
    }
    public StateModifier GetLastModifier()
    {
        return stateModifiers[stateModifiers.Count - 1];
    }

    public void SetBaseValue(float amount)
    {
        BaseValue = amount;
        IsDirty = true;
    }
    public CharacterState(string _name,float _baseValue)
    {
        StateName = _name;
        SetBaseValue(_baseValue);
    }

}
