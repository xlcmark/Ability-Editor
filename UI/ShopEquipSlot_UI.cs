using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopEquipSlot_UI : MonoBehaviour, IPointerDownHandler,IPointerEnterHandler, IPointerExitHandler
{
    public Image image;
    public Text consumableCount;
    public Image frame;
    public Ability ability { private set; get; }

    public void OnPointerDown(PointerEventData eventData)
    {
        Shop.instance.SelectedEquipSlot(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShopToolTips_UI.ShowTooltip_static(ability);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ShopToolTips_UI.HideTooltip_static();
    }

    public void SetEquip(Ability _ability)
    {
        ability = _ability;
        if (ability == null)
        {
            image.gameObject.SetActive(false);
            return;
        }

        image.gameObject.SetActive(true);
        image.sprite = ability.sprite;
        if((ability.behavior & Ability.Behavior.Consumable) != 0)
        {
            consumableCount.enabled = true;
            consumableCount.text = ability.ConsumablesCount.ToString();
        }
        else
        {
            consumableCount.enabled = false;
        }
    }

}
