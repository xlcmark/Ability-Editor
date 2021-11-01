using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorControler : MonoBehaviour
{
    #region instance
    public static CursorControler instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        UIController.onUpdateUI += SetUser;
    }
    #endregion

    Unit user;
    public Camera CursorCamera;
    public enum CursorSelectMode
    {
        Normal,
        Attack,
        UnitTarget,
        Point,
        none,
        Useless//沒有功能
    }
    public CursorSelectMode cursorSelectMode { private set; get; }

    public Texture2D normalCursor;
    public Texture2D crosshair;
    private Vector2 crosshairHotspot;
    [HideInInspector]
    public SingleTarget TargetFilter;

    public delegate void OnLeftMouseButtonDown(Vector3 point,Transform target);
    public OnLeftMouseButtonDown onLeftMouseButtonDown;

    private void SetUser(Unit unit)
    {
        user = unit;
    }
    private void Start()
    {
        crosshairHotspot = new Vector2(crosshair.width / 2, crosshair.height / 2);
        ChangeMode(CursorSelectMode.Normal);
    }

    void Update()
    {
        Ray ray = CursorCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        switch (cursorSelectMode)
        {
            case CursorSelectMode.Normal:

               break;

            case CursorSelectMode.Attack:

                break;

            case CursorSelectMode.UnitTarget:
                if (Input.GetMouseButtonDown(0))
                {
                    if(Physics.Raycast(ray,out hit, 100))
                    {
                        Unit hitUnit = hit.transform.GetComponent<Unit>();
                        if (hitUnit != null)
                        {
                            if (ConditionTargetFilter(hitUnit))
                            {
                                onLeftMouseButtonDown.Invoke(Vector3.zero, hit.transform);
                                ChangeMode(CursorSelectMode.Normal);
                            }
                        }
                    }
                }

                break;
            case CursorSelectMode.Point:

                if (Input.GetMouseButtonDown(0))
                {
                    //射線與平面的交點
                    Plane plane = new Plane(Vector3.up, new Vector3(0, 0, 0));
                    Vector3 hitPoint = new Vector3(0, 0, 0);//目標點
                    if (plane.Raycast(ray, out float enter))
                    {
                        hitPoint = ray.GetPoint(enter);
                    }

                    onLeftMouseButtonDown.Invoke(hitPoint,null);
                    ChangeMode(CursorSelectMode.Normal);

                }

                break;
            case CursorSelectMode.none:

                if (Input.GetMouseButtonDown(0))
                {
                    if (SkillIndicator.instance.CurAbility != null)
                    {
                        if (SkillIndicator.instance.CurAbility.SkillIndicatorInfo.colliderType == MultipleTarget.ColliderType.Circle)
                        {
                            onLeftMouseButtonDown(SkillIndicator.instance.InnerCircle.transform.position, null);
                            ChangeMode(CursorSelectMode.Normal);
                        }
                    }
                }
                break;
            default:
                break;
        }
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeMode(CursorSelectMode.Normal);
        }
    }



    public void ChangeMode(CursorSelectMode mode)
    {
        if (cursorSelectMode == CursorSelectMode.Useless) return;//useless不能隨意切換到其他狀態
        cursorSelectMode = mode;
         switch (mode)
         {
             case CursorSelectMode.Normal:
                 Cursor.visible = true;
                 Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);

                SkillIndicator.instance.CancelSkillIndicator();//取消技能指示計
                break;
             case CursorSelectMode.Attack:
                 Cursor.visible = true;
                 Cursor.SetCursor(crosshair, crosshairHotspot, CursorMode.Auto);

                SkillIndicator.instance.CancelSkillIndicator();//取消技能指示計
                break;
             case CursorSelectMode.UnitTarget:
                 Cursor.visible = true;
                 Cursor.SetCursor(crosshair, crosshairHotspot, CursorMode.Auto);

                SkillIndicator.instance.CancelSkillIndicator();//取消技能指示計
                break;
             case CursorSelectMode.Point:
                 Cursor.visible = true;
                 Cursor.SetCursor(crosshair, crosshairHotspot, CursorMode.Auto);
                 break;
            case CursorSelectMode.none:
                Cursor.visible = false;
                Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
                break;
            case CursorSelectMode.Useless:
                Cursor.visible = true;
                Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
                SkillIndicator.instance.CancelSkillIndicator();//取消技能指示計
                break;
             default:
                 break;
         }
    }
    //從useless中恢復
    public void RecoverUsed()
    {
        if (cursorSelectMode != CursorSelectMode.Useless) return;

        cursorSelectMode = CursorSelectMode.Normal;
        Cursor.visible = true;
        Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
        SkillIndicator.instance.CancelSkillIndicator();//取消技能指示計
    }

    private bool ConditionTargetFilter(Unit unitTar)
    {
        if (TargetFilter != null)
        {
            if (TargetFilter.team == UnitTeam.Enemy)
            {
                if (unitTar.team == user.team) return false;
            }
            else if(TargetFilter.team == UnitTeam.Friendly)
            {
                if (unitTar.team != user.team) return false;
            }

            if ((unitTar.type & TargetFilter.type) == 0)
                return false;
            if ((unitTar.flag != 0) && ((unitTar.flag & TargetFilter.flag) == 0))
                return false;

            return true;
        }
        return true;
    }

}
