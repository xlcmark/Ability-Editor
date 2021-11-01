using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventControler : MonoBehaviour
{
    private Dictionary<EventType, List<GameEvent>> Listeners=new Dictionary<EventType, List<GameEvent>>();//全部的監聽者

    public void OnEvent(EventType eventType,EventParameter e)
    {
        List<GameEvent> listenList = null;
        if (!Listeners.TryGetValue(eventType, out listenList))//搜尋有沒有符合的類
            return;

        if (listenList == null)
            return;

        for(int i=0;i<listenList.Count;i++)
        {
            if (e!=null && e.RefModifier != null)
            {
                if (listenList[i].RefModifier == e.RefModifier)//確認是同個modifier的事件
                {
                    listenList[i].OnTrigger(transform, e);
                }
            }
            else
            {
                listenList[i].OnTrigger(transform, e);
            }
        }

    }

    public void AddListener(EventType eventType,GameEvent NewListener)//新增監聽事件
    {
        List<GameEvent> listenList = null;
        if (Listeners.TryGetValue(eventType, out listenList))
        {
            listenList.Add(NewListener);
        }
        else
        {
            listenList = new List<GameEvent>();
            listenList.Add(NewListener);
            Listeners.Add(eventType, listenList);
        }
    }

    public void RemovedListener(EventType eventType,GameEvent e)
    {
        List<GameEvent> listenList = null;
        if (Listeners.TryGetValue(eventType,out listenList))
        {
            if(listenList!=null)
                listenList.Remove(e);
        }
    }

}
public enum EventType
{
    OnOwnerDeath,
    OnAttackStart,
    OnAttackLanded,//擊中
    OnAbilityStart,//使用技能
    OnAttacked,//被普攻擊
   
    modifeir_OnThinker,//持續行觸發
    modifier_OnCreated,//當modifier被建立
    modifier_OnOverlay,//當modifier疊加
    modifier_OnDestory,//當modifier被移除

    OnDamaged,//被傷害，不一定扣血
    OnHurt,//損血
}
public class EventParameter
{
    public Transform target;
    public Vector3 point;
    public Vector3 projectilePos;

    public Modifier RefModifier;//modifier事件裡使用

}