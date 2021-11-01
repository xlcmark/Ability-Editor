using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearProjectileMove : MonoBehaviour
{
    //儲存此技能的參數
    private Ability ability;
    private Modifier modifier;
    private AbilityTarget abilityTarget;
    private List<BaseAction> OnHitActions = new List<BaseAction>();

    private List<Transform> hitObjs = new List<Transform>();//擊中的對象
    private Vector3 startPos;

    private float speed;
    private float distance;//飛行距離
    private bool IsGoPass;//是否穿透
    private LayerMask hitLayer;
    private Unit moveBlockUnit;//碰撞器跟隨此單位移動，用於moveBlock，一般不用填
    private Unit Caster;

    public GameObject[] DisableImmediateVfxs;//立即銷毀，除了這些特效要延遲銷毀ex:讓軌跡跑完
    public string muzzleVfx;
    public string hitBodyVfx;
    public string hitVfx;

    [Space]
    [Header("Collider")]
    public float width;//決定有多少間隔
    public float interval = 1;//射線的間隔，最小單位的直徑

    public void SetProjectile(Ability _ability,Modifier _modifier,AbilityTarget _abilityTarget,List<BaseAction> _actions,LayerMask _layer,float _speed,float _dis,bool _IsGoPass,Unit _moveBlockUnit)
    {
        ability = _ability;
        modifier = _modifier;
        abilityTarget = _abilityTarget;
        OnHitActions = _actions;
        hitLayer = _layer;
        speed = _speed;
        distance = _dis;
        IsGoPass = _IsGoPass;
        moveBlockUnit = _moveBlockUnit;
        Caster = ability.Owner.GetComponent<Unit>();
    }
    private void Start()
    {
        startPos = transform.position;
        if (muzzleVfx.Length != 0)
        {
            GameEffectManager.instance.AddWorldEffect(muzzleVfx, transform.position, transform.rotation);
        }

    }
    private void Update()
    {
        OnRayCast(transform.position);//原點
        for (int i = 1; i <= width/interval; i++)//因寬度增加的點
        {
            Vector3 Ipos = i * transform.right * interval;
            OnRayCast(transform.position + Ipos);
            OnRayCast(transform.position - Ipos);
        }

        //move
        transform.Translate(speed*transform.forward*Time.deltaTime ,Space.World);

        if (Vector3.Distance(transform.position, startPos) > distance)
        {
            ProjectileDestroy();
        }
    }
    private void OnRayCast(Vector3 pos)
    {
        Ray ray= new Ray(pos, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, speed * Time.deltaTime, hitLayer))
        {
            if (IsNotHitBefore(hit.transform))//有無碰撞過
            {
                Unit unitTar = hit.transform.GetComponent<Unit>();
                if (unitTar != null)//是單位
                {
                    if (Condition(unitTar,Caster))//通過條件
                    {
                        OnHit(hit.transform);
                    }
                }
                else//不是單位
                {
                    OnHit(hit.transform);
                }
            }
        }

    }
    //條件
    private bool Condition(Unit unitTar,Unit unitCaster)
    {
        if (unitTar != null)
        {
            if (abilityTarget.team == UnitTeam.Enemy)
            {
                if (unitTar.team == unitCaster.team) return false;
            }
            else if (abilityTarget.team == UnitTeam.Friendly)
            {
                if (unitTar.team != unitCaster.team) return false;
            }

            if ((unitTar.type & abilityTarget.type) == 0)
                return false;
            if ((unitTar.flag != 0) && ((unitTar.flag & abilityTarget.flag) == 0))
                return false;

            return true;
        }
        return false;
    }
    //有沒有擊中過
    private bool IsNotHitBefore(Transform obj)
    {
        if (obj.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            obj = obj.root;//牆全部只看一次
        }

        if (hitObjs.Contains(obj)){
            return false;
        }
        else
        {
            hitObjs.Add(obj);
            return true;
        }
    }

    private void OnHit(Transform hitObj)
    {
        Transform owner = ability.Owner;
        //碰撞使moveblockUnit停止
        if (moveBlockUnit != null)
        {
            owner = moveBlockUnit.transform;
            if (!IsGoPass)
            {
                if (moveBlockUnit.curMoveCoro != null)
                {
                    Thinker.instance.StopCoroutine(moveBlockUnit.curMoveCoro);
                    moveBlockUnit.RemoveAirBorne();
                }
            }
        }
        //doActions
        EventParameter e = new EventParameter();
        e.target = hitObj;
        e.projectilePos = transform.position;

        for (int i = 0; i < OnHitActions.Count; i++)
        {
            OnHitActions[i].Apply(owner, e, ability, modifier);
        }
        //effect
        if (hitVfx.Length != 0)
        {
            GameEffectManager.instance.AddWorldEffect(hitVfx, hitObj.position, Quaternion.LookRotation(transform.forward));
        }
        if (hitBodyVfx.Length != 0)
        {
            Unit tarUnit = hitObj.GetComponent<Unit>();
            if (tarUnit != null && tarUnit.attachEffectPointPrefab.body!=null)
                GameEffectManager.instance.AddWorldEffect(hitBodyVfx, tarUnit.attachEffectPointPrefab.body.position, Quaternion.LookRotation(transform.forward));
        }
        //destroy
        if (!IsGoPass)
        {
            ProjectileDestroy();
        }
    }

    private void ProjectileDestroy()
    {
        foreach (var vfx in DisableImmediateVfxs)
        {
            vfx.SetActive(false);
        }
        Destroy(gameObject, 2);
        this.enabled = false;

    }

    private void OnDrawGizmos()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(transform.position + transform.right * width, .1f);
        Gizmos.DrawWireSphere(transform.position - transform.right * width, .1f);
       
        Gizmos.DrawRay(transform.position,transform.forward);//原點
        for (int i = 1; i <= width / interval; i++)//因寬度增加的點
        {
            Vector3 Ipos = i * transform.right * interval;
            Gizmos.DrawRay(transform.position+Ipos, transform.forward);
            Gizmos.DrawRay(transform.position-Ipos, transform.forward);
        }
    }


}
