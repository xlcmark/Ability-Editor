using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseAttTrackingProjectile : MonoBehaviour
{
    Unit target;
    DamageSystem targetDS;
    public float speed=20;
    Action<Unit,DamageSystem> OnHit;
    bool IsHit;
    public GameObject[] DisableImmediateVfxs;//除了這些特效要延遲銷毀
    void Update()
    {
        if (IsHit) return;
        if (target == null || !target.isActiveAndEnabled) {
            DestroyProticle();
            return;
        }
        Vector3 targetPos = target.transform.position + Vector3.up;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        Vector3 dir = targetPos - transform.position;
        if(dir!=Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
        if (Vector3.Distance(transform.position, targetPos) <= .1)
        {
            OnHit(target,targetDS);
            DestroyProticle();
        }
    }
    public void SetProjectile(Unit _target,DamageSystem _ds,Action<Unit,DamageSystem> _OnHit)
    {
        target = _target;
        targetDS = _ds;
        OnHit = _OnHit;
        IsHit = false;
        foreach (var vfx in DisableImmediateVfxs)
        {
            vfx.SetActive(true);
        }
    }
    private void DestroyProticle()
    {
        Invoke("CloseProjectile", 1);
        foreach (var vfx in DisableImmediateVfxs)
        {
            vfx.SetActive(false);
        }
        IsHit = true;
    }
    private void CloseProjectile()
    {
        gameObject.SetActive(false);
    }

}
