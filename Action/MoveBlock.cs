using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//強制移動
[CreateAssetMenu(menuName = "Action/MoveBlock")]
public class MoveBlock : BaseAction
{
    public enum Type { point,direction}
    [Header("target+point=跳向敵人"),Space(-10),Header("target+point+oppositeDir=將敵人拉向自己"),Space(-10), Header("target+direction=將對像擊退"),Space(-10), Header("point+point=跳到指定地點"),Space(-10), Header("point+direction=dash")]
    public Type type;
    public bool oppositeDir;//反向量
    public float duration=0.2f;
    [Tooltip("控制高度，gravity為負值，0為dash")]
    public float g=-10;

    [ConditionHide("type",(int)Type.direction)]
    public float distance;

    public ModifierAnimation Animation;

    [Expandable(true)]
    public List<BaseAction> OnFinishMoveActions;

    [Header("碰撞器檢測")]//owner指被強制移動的對象
    public bool IsUsedCollider;
    [ConditionHide("IsUsedCollider",true)]
    public GameObject emptyCollidier;
    [ConditionHide("IsUsedCollider", true)]
    public bool IsGoPass;//碰撞器是否穿越
    [ConditionHide("IsUsedCollider", true)]
    public LayerMask hitlayer;
    [ConditionHide("IsUsedCollider", true)]
    public AbilityTarget hitTarget;
    [Expandable(true)]
    [Header("碰撞事件")]
    public List<BaseAction> OnHitActions;

    public override void DoAction(Transform target, Ability ability, Modifier modifier)
    {
        Unit tarUnit = target.GetComponent<Unit>();
        Unit ownerUnit = ability.Owner.GetComponent<Unit>();

        Coroutine coro;
        switch (type)
        {
            case Type.point:  
                if(oppositeDir)//拉向 ex:機器人Q  
                {
                    if (tarUnit != null)
                    {
                        if (tarUnit.curMoveCoro != null)
                        {
                            Thinker.instance.StopCoroutine(tarUnit.curMoveCoro);//停止舊的corotine
                        }
                        //開始新的corotine
                        EventParameter e = new EventParameter { target = target, point = ability.Owner.position };
                        coro = Thinker.instance.StartCoroutine(MoveToPoint(target, ability.Owner.position, ability.SpellRange,ability,modifier,e));
                        tarUnit.curMoveCoro = coro;
                    }
                }
                else//跳過去ex:賈克斯Q
                {
                    if (ownerUnit != null)
                    {
                        if (ownerUnit.curMoveCoro != null)
                        {
                            Thinker.instance.StopCoroutine(ownerUnit.curMoveCoro);//停止舊的corotine
                        }
                        //開始新的corotine
                        EventParameter e = new EventParameter { target = target, point = target.position };
                        coro = Thinker.instance.StartCoroutine(MoveToPoint(ability.Owner, target.position, ability.SpellRange, ability, modifier,e));
                        ownerUnit.curMoveCoro = coro;
                    }
                }
                break;
            case Type.direction://擊退 ex:砲娘R
                Vector3 dir = target.position - ability.Owner.position;
                dir.y = 0;
                dir.Normalize();
                if (tarUnit != null)
                {
                    if (tarUnit.curMoveCoro != null)
                    {
                        Thinker.instance.StopCoroutine(tarUnit.curMoveCoro);
                    }
                    coro= Thinker.instance.StartCoroutine(MoveToDirection(target, dir,ability,modifier));
                    tarUnit.curMoveCoro = coro;
                }
                break;
            default:
                break;
        }
    }
    public override void DoAction(Vector3 point, Ability ability, Modifier modifier)
    {
        Unit ownerUnit = ability.Owner.GetComponent<Unit>();
        Coroutine coro;
        switch (type)
        {
            case Type.point://指定位移 ex:砲娘W
                if (ownerUnit != null)
                {
                    if (ownerUnit.curMoveCoro != null)
                    {
                        Thinker.instance.StopCoroutine(ownerUnit.curMoveCoro);
                    }
                    EventParameter e = new EventParameter { target = ability.Owner, point = point };
                    coro = Thinker.instance.StartCoroutine(MoveToPoint(ability.Owner, point,ability.SpellRange,ability,modifier,e));
                    ownerUnit.curMoveCoro = coro;
                }
                break;
            case Type.direction://dash ex:雷文Q
                Vector3 dir = point - ability.Owner.position;
                dir.y = 0;
                dir.Normalize();
                if (ownerUnit != null)
                {
                    if (ownerUnit.curMoveCoro != null)
                    {
                        Thinker.instance.StopCoroutine(ownerUnit.curMoveCoro);
                    }
                    coro=Thinker.instance.StartCoroutine(MoveToDirection(ability.Owner, dir,ability,modifier));
                    ownerUnit.curMoveCoro = coro;
                }
                break;
            default:
                break;
        }
    }
    //兩點拋物線
    private IEnumerator MoveToPoint(Transform target,Vector3 point,float maxDis,Ability ability,Modifier modifier,EventParameter _e)//maxdis為spellrange
    {
        //animation
        Animator anim = target.GetComponent<Animator>();
        if (anim!=null && Animation!=ModifierAnimation.none)
        {
            anim.SetBool(Animation.ToString(), true);
        }

        Unit unit = target.GetComponent<Unit>();
        unit.AddAirBorne();

        point.y = 0;
        target.LookAt(point);

        float dis = Vector3.Distance(target.position, point);
        float time = Mathf.Clamp( duration * dis / maxDis,.1f,duration);//越遠跳的時間越長，跳得越高
        //初速
        Vector3 speed=new Vector3((point.x-target.position.x)/time, (point.y - target.position.y) / time-.5f*time * g, (point.z - target.position.z) / time);
        Vector3 gravity=Vector3.zero;

        float dTime = 0;//經過時間
        float perTime=0.02f;
        target.transform.position=new Vector3(target.transform.position.x,0, target.transform.position.z);//把高度先歸0
        while (dTime < time)
        {
            yield return new WaitForSeconds(perTime);

            dTime += perTime;
            gravity.y = g * dTime;

            target.Translate(speed * perTime,Space.World);
            target.Translate(gravity * perTime, Space.World);
            //確保y最小為0
            target.transform.position = new Vector3(target.transform.position.x, Mathf.Clamp(target.transform.position.y, 0, 100), target.transform.position.z);

        }
        unit.RemoveAirBorne();
        //close animation
        if (anim != null && Animation != ModifierAnimation.none)
        {
            anim.SetBool(Animation.ToString(), false);
        }
        // EventParameter e = new EventParameter { target = target,point=point };
        OnFinishMove(ability.Owner, _e, ability, modifier);

    }
    //拋物線向量
    private IEnumerator MoveToDirection(Transform target,Vector3 dir,Ability ability,Modifier modifier)
    {
        //animation
        Animator anim = target.GetComponent<Animator>();
        if (anim!=null && Animation != ModifierAnimation.none)
        {
            anim.SetBool(Animation.ToString(), true);
        }

        Unit unit = target.GetComponent<Unit>();
        unit.AddAirBorne();

        target.rotation = Quaternion.LookRotation(dir);
        if (oppositeDir)
            dir = -dir;
        //初速
        Vector3 speed = new Vector3(dir.x * distance / duration, dir.y * distance / duration - .5f * duration * g, dir.z * distance / duration);
        Vector3 gravity = Vector3.zero;

        float dTime = 0;//經過時間
        float perTime = 0.02f;
        target.transform.position = new Vector3(target.transform.position.x, 0, target.transform.position.z);//把高度先歸0
        //碰撞器
        if (emptyCollidier != null)
        {
            GameObject obj = Instantiate(emptyCollidier, new Vector3( target.transform.position.x,.5f,target.transform.position.z), Quaternion.LookRotation(dir));
            LinearProjectileMove pm = obj.GetComponent<LinearProjectileMove>();
            pm.SetProjectile(ability, modifier, hitTarget, OnHitActions, hitlayer,distance/duration,distance,IsGoPass,unit);
        }
        while (dTime < duration)
        {
            yield return new WaitForSeconds(perTime);

            dTime += perTime;
            gravity.y = g * dTime;
            target.Translate(speed * perTime, Space.World);
            target.Translate(gravity * perTime, Space.World);
            //確保y最小為0
            target.transform.position = new Vector3(target.transform.position.x, Mathf.Clamp(target.transform.position.y, 0, 100), target.transform.position.z);
        }
        unit.RemoveAirBorne();
        //close animation
        if (anim != null && Animation != ModifierAnimation.none)
        {
            anim.SetBool(Animation.ToString(), false);
        }
        EventParameter e = new EventParameter { target = target, point = target.transform.position };
        OnFinishMove(ability.Owner, e, ability, modifier);
    }

    //結束後的行動
    private void OnFinishMove(Transform owner,EventParameter e, Ability ability, Modifier modifier)
    {
        foreach (var act in OnFinishMoveActions)
        {
            act.Apply(owner, e, ability, modifier);
        }
    }


}
