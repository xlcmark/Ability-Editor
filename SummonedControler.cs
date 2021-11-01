using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonedControler : MonoBehaviour
{
    Unit unit;
    MovementController mc;
    private void Awake()
    {
        unit = GetComponent<Unit>();
        mc = GetComponent<MovementController>();
    }
    #region 組合鍵shift+左鍵
    private void OnGUI()
    {
        if (Event.current.rawType == UnityEngine.EventType.MouseDown)
        {
            EventCallBack(Event.current);
        }
    }
    private void EventCallBack(Event e)
    {
        bool eventDown = (e.modifiers & EventModifiers.Shift) != 0;
        if (!eventDown) return;

        e.Use();//標記已使用過

        if (e.button == 0)
        {
            OnRayCastHit();
        }
    }
    #endregion
    private void OnRayCastHit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray,out hit, 50, LayerMask.GetMask("Ground") | LayerMask.GetMask("Unit")))
        {
            Unit TarUnit = hit.transform.GetComponent<Unit>();
            if (TarUnit != null)//點人
            {
                if (TarUnit.team != unit.team)
                {
                    mc.ChangeState(MovementController.ComandState.AttackSomone, Vector3.zero, TarUnit);
                }
            }
            else//點地
            {
                mc.ChangeState(MovementController.ComandState.Move, hit.point, null);
            }
        }
    }
}
