using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderSwitcher:MonoBehaviour
{
    public static ShaderSwitcher instance;
    public enum shaderEffect { origin, invisible}
    private Dictionary<shaderEffect, Shader> shaderList = new Dictionary<shaderEffect, Shader>();
    private void Awake()
    {
        if (instance == null) instance = this;
        shaderList.Add(shaderEffect.invisible, Shader.Find("Custom/invisible"));
    }
    public Material[] ChangeMaterials(shaderEffect _effect,Material[] _oldMats)
    {
        Shader effShader = shaderList[_effect];
        Material[] newMats = new Material[_oldMats.Length];
        for (int i = 0; i < newMats.Length; i++)
        {
            if (_oldMats[i] == null) continue;
            Material newMat = new Material(_oldMats[i]);
            newMat.shader = effShader;
            SetParameter(_effect, newMat);
            newMats[i] = newMat;
        }
        return newMats;
    }
    private void SetParameter(shaderEffect _shaderEffect, Material material)
    {
        switch (_shaderEffect)
        {
            case shaderEffect.invisible:
                material.SetFloat("_AlphaScale", 0.3f);
                break;

            default:
                break;
        }
    }


}
