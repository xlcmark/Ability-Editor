using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEffect : MonoBehaviour
{
    public ParticleSystem ps;
    public TrailRenderer trail;
    public string EffectName;
    public bool Isloop=false;//持續性
    private Transform[] GOs;

    public void Init(GameEffectInfo info)
    {
        ps = GetComponent<ParticleSystem>();
        trail = GetComponent<TrailRenderer>();
        EffectName = info.VfxName;
        gameObject.SetActive(false);
        if (ps != null)
        {
            Isloop = ps.main.loop;
        }
        if (trail != null)
        {
            Isloop = true;
        }
        GOs = GetComponentsInChildren<Transform>();
    }
    public void Play()
    {
        gameObject.SetActive(true);
        if (ps != null)
        {
            ps.Simulate(0, true, true);//restart
            ps.Play();
        }
    }
    public void Close()
    {
        //layer
        ChangedLayer( LayerMask.NameToLayer("Default"));

        transform.SetParent(null);
        gameObject.SetActive(false);
    }
    public void ChangedLayer(int _layer)
    {
        foreach (var go in GOs)
        {
            go.gameObject.layer = _layer;
        }
    }
}
