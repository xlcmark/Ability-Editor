using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ShopItem_UI :MonoBehaviour,IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image image;
    public Text Price;
    public Image frame;
    public Ability ability { private set; get; }
    Coroutine coroutine;

    public void SetItem(Ability _ability)
    {
        ability = _ability;
        image.sprite = _ability.sprite;
        Price.text = _ability.Price.ToString();
    }
  

    public void OnPointerDown(PointerEventData eventData)
    {
        Shop.instance.SelectedShopItem(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine =StartCoroutine(OpenShop());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(coroutine!=null)
            StopCoroutine(coroutine);
        ShopToolTips_UI.HideTooltip_static();
    }
    private IEnumerator OpenShop()
    {
        yield return new WaitForSeconds(0.3f);
        ShopToolTips_UI.ShowTooltip_static(ability);
    }
}
