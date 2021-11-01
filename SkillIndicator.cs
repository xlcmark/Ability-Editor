using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillIndicator : MonoBehaviour
{
    #region instance
    public static SkillIndicator instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

    public GameObject root;
    [Header("扇形")]
    public Image Sector;
    public Transform SectorEdgeR;
    public Transform SectorEdgeL;
    [Header("圓")]
    public Image OuterCircle;
    public Image InnerCircle;


    public Ability CurAbility { get; private set; }

    void Update()
    {
        if (CurAbility == null) { return; }

        transform.position = new Vector3(CurAbility.Owner.position.x, .1f, CurAbility.Owner.position.z);
        //射線與平面的交點
        Plane plane = new Plane(Vector3.up, new Vector3(0, 0.1f, 0));
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 hitPoint = new Vector3(0, 0.1f, 0);//目標點
        if (plane.Raycast(ray, out float enter))
        {
            hitPoint = ray.GetPoint(enter);
        }
        Vector3 dir = hitPoint - CurAbility.Owner.position;//到目標向量
        dir.y = 0;

        ChoiceType();//setActive

        switch (CurAbility.SkillIndicatorInfo.colliderType)
        {
            case MultipleTarget.ColliderType.Sector:
                Sector.rectTransform.localScale = new Vector3(CurAbility.SkillIndicatorInfo.Radius * 2, CurAbility.SkillIndicatorInfo.Radius * 2, 1);
                Sector.fillAmount = (float)CurAbility.SkillIndicatorInfo.CircleAngle / 360;//扇形角度

                //偏移角使扇形中線偏移
                dir = Quaternion.Euler(0, -CurAbility.SkillIndicatorInfo.CircleAngle / 2, 0) * dir;
                Sector.rectTransform.localRotation = Quaternion.LookRotation(dir) * Quaternion.FromToRotation(Vector3.up, Vector3.forward);

                //調整扇形邊的位置
                SectorEdgeR.localRotation = Quaternion.Euler(0, 0, -CurAbility.SkillIndicatorInfo.CircleAngle);
                SectorEdgeL.localRotation = Quaternion.Euler(0, 0, 0);

                break;
            case MultipleTarget.ColliderType.Circle:
                OuterCircle.rectTransform.localScale = new Vector3(CurAbility.SpellRange * 2, CurAbility.SpellRange * 2, 1);
                InnerCircle.rectTransform.localScale = CurAbility.SkillIndicatorInfo.Radius / CurAbility.SpellRange * Vector3.one;//除掉外圓距離

                //內圓向量跟距離
                float dis = Mathf.Clamp((hitPoint - transform.position).magnitude, 0, CurAbility.SpellRange);
                InnerCircle.transform.position = transform.position + (dir.normalized * dis);

                break;
            default:
                break;
        }

    }
    private void ChoiceType()
    {
        Sector.gameObject.SetActive(false);
        OuterCircle.gameObject.SetActive(false);

        if (CurAbility.SkillIndicatorInfo.colliderType == MultipleTarget.ColliderType.Sector)
        {
            Sector.gameObject.SetActive(true);
        }
        if (CurAbility.SkillIndicatorInfo.colliderType == MultipleTarget.ColliderType.Circle)
        {
            OuterCircle.gameObject.SetActive(true);
        }
    }


    public void EnterSkillIndicatorInfo(Ability ability)
    {
        CurAbility = ability;
        root.SetActive(true);
    }
    public void CancelSkillIndicator()
    {
        CurAbility = null;
        root.SetActive(false);
    }
}
