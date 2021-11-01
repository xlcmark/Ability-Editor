using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationVfxEvent : MonoBehaviour
{
    Animator anim;
    Unit unit;
    public MeleeWeaponTrail weaponTrail;
    AnimationEventBehaviour[] animationEventBehaviours;

    public class AnimVfx
    {
        public int Hash { private set; get; }
        public string VfxName { private set; get; }
        public AnimVfx(int _hash, string _name) { Hash = _hash; VfxName = _name; }
    }
    public List<AnimVfx> animVfxes = new List<AnimVfx>();

    private void Start()
    {
        anim = GetComponent<Animator>();
        unit = GetComponent<Unit>();
        unit.OnDead+= Unit_OnDead;
        //從Animator抓出behaviour
        animationEventBehaviours=anim.GetBehaviours<AnimationEventBehaviour>();
        for (int i = 0; i < animationEventBehaviours.Length; i++)
        {
            animationEventBehaviours[i].OnExit += VfxClose;
        }
    }

    //將特效名稱打在string使用
    public void VFXPlayOnUnit(AnimationEvent _event)
    {
        if (_event.animatorStateInfo.normalizedTime > 1) return;//僅loop第一輪觸發
        animVfxes.Add(new AnimVfx(_event.animatorStateInfo.fullPathHash, _event.stringParameter));
        unit.AddVFX(_event.stringParameter, AttachEffectPoint.unit, Quaternion.identity);
    }
    public void VFXPlayOnTop(AnimationEvent _event)
    {
        if (_event.animatorStateInfo.normalizedTime > 1) return;//僅loop第一輪觸發
        animVfxes.Add(new AnimVfx(_event.animatorStateInfo.fullPathHash, _event.stringParameter));
        unit.AddVFX(_event.stringParameter, AttachEffectPoint.top, Quaternion.identity);
    }
    public void VFXPlayOnBody(AnimationEvent _event)
    {
        if (_event.animatorStateInfo.normalizedTime > 1) return;//僅loop第一輪觸發
        animVfxes.Add(new AnimVfx(_event.animatorStateInfo.fullPathHash, _event.stringParameter));
        unit.AddVFX(_event.stringParameter, AttachEffectPoint.body, Quaternion.identity);
    }
    public void VFXPlayOnHands(AnimationEvent _event)
    {
        if (_event.animatorStateInfo.normalizedTime > 1) return;//僅loop第一輪觸發
        animVfxes.Add(new AnimVfx(_event.animatorStateInfo.fullPathHash, _event.stringParameter));
        unit.AddVFX(_event.stringParameter, AttachEffectPoint.hands, Quaternion.identity);
    }
    public void VFXPlayOnWeapon(AnimationEvent _event)
    {
        if (_event.animatorStateInfo.normalizedTime > 1) return;//僅loop第一輪觸發
        animVfxes.Add(new AnimVfx(_event.animatorStateInfo.fullPathHash, _event.stringParameter));
        unit.AddVFX(_event.stringParameter, AttachEffectPoint.weapon, Quaternion.identity);
    }
    public void WeaponTrailOpen(AnimationEvent _event)
    {
        if (_event.animatorStateInfo.normalizedTime > 1) return;//僅loop第一輪觸發
        animVfxes.Add(new AnimVfx(_event.animatorStateInfo.fullPathHash, _event.stringParameter));
        if (weaponTrail != null)
        {
            weaponTrail.Emit = true;
        }
    }
    public void VfxClose(int _hash)
    {
        for (int i = 0; i < animVfxes.Count; i++)
        {
            if (animVfxes[i].Hash == _hash)
            {
                if (animVfxes[i].VfxName != null)
                {
                    unit.RemoveVFX(animVfxes[i].VfxName);
                }
                animVfxes.Remove(animVfxes[i]);
                i--;
            }
        }
        if (weaponTrail!=null)
            weaponTrail.Emit = false;
    }
    public void VfxCloseAll()
    {
        for (int i = 0; i < animVfxes.Count; i++)
        {
            if (animVfxes[i].VfxName != null)
                unit.RemoveVFX(animVfxes[i].VfxName);
        }
        animVfxes.Clear();
        if (weaponTrail != null)
            weaponTrail.Emit = false;

    }

    void Unit_OnDead(Unit obj)
    {
        VfxCloseAll();
    }

}