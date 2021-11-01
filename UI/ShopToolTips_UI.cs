using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class ShopToolTips_UI : MonoBehaviour
{
    private static ShopToolTips_UI instance;

    public GameObject tooltipsPanel;
    public Image image;
    public Text ItemName;
    public Text Price;
    public Text Description;
    public Text Properties;

    Ability LastAbility;
    RectTransform _parent;
    RectTransform rectTransform;

    void Awake()
    {
        if (instance == null)
            instance = this;
        HideTooltip();//初始隱藏
        _parent = transform.parent as RectTransform ;
        rectTransform = tooltipsPanel.GetComponent<RectTransform>();
    }
    void LateUpdate()
    {
        if (!tooltipsPanel.activeInHierarchy) return;
        //讓UI跟著滑鼠
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_parent, Input.mousePosition, Camera.main, out localPoint);
        transform.localPosition = localPoint;

    }

    private void ShowTooltip(Ability ability)
    {
        if (ability == null) return;

        tooltipsPanel.SetActive(true);

        if (LastAbility == ability) return;
        LastAbility = ability;

        //UI隨位置變換錨點
        Vector2 coner = new Vector2(Input.mousePosition.x < (Screen.width / 2) ? 0 : 1, Input.mousePosition.y < (Screen.height / 2) ? 0 : 1);
        rectTransform.pivot = coner;
        rectTransform.anchoredPosition = new Vector2(coner.x == 0 ? 100 : -100, coner.y == 0 ? 100 : -100);

        image.sprite = ability.sprite;
        ItemName.text = ability.AbilityName;
        Price.text = ability.Price.ToString();

        //description
        if (ability.Description.Contains("<") && ability.Description.Contains(">"))
        {
            Regex regex = new Regex("(?<=<)[^>]*(?=>)");//regex，擷取<>內的字串，不包含<>本身
            MatchCollection matches = regex.Matches(ability.Description);//有<>的集合
            for (int i = 0; i < matches.Count; i++)
            {
                value val = ability.ValueList.Find(x => x.valueName == matches[i].ToString());//查找有無匹配valueName
                if (val != null)
                {
                    int level = Mathf.Clamp(ability.Level, 0, val.values.Length - 1);
                    ability.Description = ability.Description.Replace("<" + matches[i] + ">", "<color=#C394FF>" + val.values[level].ToString("0") + "</color>");
                }
            }
        }
        Description.text = ability.Description;

        //property
        Properties.text = null;
        foreach (var mod in ability.passiveModifiers)
        {
            if (mod.properties.Count != 0)
            {
                foreach (var prop in mod.properties)
                {
                    string persent = prop.propertyType.ToString().Contains("Percentage") ? "%" : null;
                    string type = prop.propertyType.ToString();

                    Properties.text += "+" + prop.value.values[0]+persent+" " + type.Split('_')[0] + "\n";
                }
                break;
            }
        }

    }
    private void HideTooltip()
    {
        tooltipsPanel.SetActive(false);
    }

    //給外面用
    public static void ShowTooltip_static(Ability ability)
    {
        instance.ShowTooltip(ability);//static有instance實體化才能用
    }
    public static void HideTooltip_static()
    {
        instance.HideTooltip();
    }

}
