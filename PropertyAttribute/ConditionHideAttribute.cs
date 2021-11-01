
using UnityEngine;

public class ConditionHideAttribute : PropertyAttribute
{
    /// <summary>
    /// 為變數ConditionValueName（enum or bool）設立一個條件，決定此欄位是否隱藏。
    /// 若為enum請用belongNumber(舉例：若想在ConditionValueName為1時顯示，belongNumber設為1)
    /// 若為bool請用belongBool
    /// </summary>
    public string ConditionValueName;
    public int BelongNumber;
    public bool BelongBool;
    public ConditionHideAttribute(string name, int belongNumber)
    {
        ConditionValueName = name;
        BelongNumber = belongNumber;
    }
    public ConditionHideAttribute(string name, bool belongbool)
    {
        ConditionValueName = name;
        BelongBool = belongbool;
    }
}
