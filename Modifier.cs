using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class Modifier : ScriptableObject
{
    [HideInInspector]
    public Ability RefAbility;//擁有者

    public Sprite sprite;
    public string ModifierName;

    public enum MutipleType//疊加類型//同名modifier務必設一樣
    {
        normal,//例如：一般裝備
        overlay,//疊層數，例如：毒類技能
        none//不能疊加，例如：唯一被動
    }
    public MutipleType mutipleType;

    private int overlayCount=1;//疊的層數
    [ConditionHide("mutipleType",(int)MutipleType.overlay)]
    public int MaxOverlay=1;//最大層數
    public int OverlayCount
    {
        get { return overlayCount; }
        set
        {
            overlayCount = Mathf.Clamp(value, 1, MaxOverlay);
        }
    }
    [System.Serializable]
    public class ModVfx
    {
        public string vfxName;
        public AttachEffectPoint point;
    }
    public ModVfx modVfx;//modifier的特效 跟modifier綁在一起 消失時一起消失
    [Tooltip("天生的技能，角色死亡時不會刪除")]
    public bool IsPassive;//天生的 永久的
    public bool IsHidden;
    [ValueAuto]
    public value Duration;
    [HideInInspector]
    public float dur;//正在持續時間

    //持續性技能ex中毒
    [ValueAuto]
    public value ThinkerInternal;//間隔多久

    [System.Serializable]
    public class OverrideAnimation
    {
        public bool IsUsed;
        public AnimationClip Idle;
        public AnimationClip Run;
        public AnimationClip Attack01;
        public AnimationClip Die;
        public AnimationClip Spell;
        public AnimationClip Spell2;
    }

    public OverrideAnimation overrideAnimation;

    public ModifierAnimation modifierAnimation;

    public BaseAttTrackingProjectile changedProjectile;

    [Expandable(true)]
    public List<ModiferProperty> properties;

    [Expandable(true)]
    public List<ModifierState> staties;

    [Expandable(true)]
    public List<GameEvent> events;

    [System.Serializable]
    public class BasicAtkBonus
    {
        public DamageType AtkType;
        [ValueAuto]
        public value value;
        public string vfxName;
    }

    public BasicAtkBonus basicAtkBonus;

    public event System.Action<int> OnOverlay;
    public void TriggerOnOverlay()
    {
        OnOverlay?.Invoke(OverlayCount);
    }

}
public enum ModifierAnimation { none, loopAction01, loopAction02 };