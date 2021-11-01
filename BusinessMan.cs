using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BusinessMan : MonoBehaviour
{
    public float dis;
    public Texture2D sprite;
    public Shop shop;
    bool IsMouseEnter;
    Transform player;

    private void Awake()
    {
        UIController.onUpdateUI += SetPlayer;
    }
    private void SetPlayer(Unit unit)
    {
        player = unit.transform;
    }

    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;//ui阻擋
        if (CursorControler.instance.cursorSelectMode == CursorControler.CursorSelectMode.Normal)
        {
            Cursor.SetCursor(sprite, new Vector2(sprite.width/2,sprite.height/2), CursorMode.Auto);
            IsMouseEnter = true;
        }
    }
    private void OnMouseExit()
    {
        if (CursorControler.instance.cursorSelectMode == CursorControler.CursorSelectMode.Normal)
            CursorControler.instance.ChangeMode(CursorControler.CursorSelectMode.Normal);
        IsMouseEnter = false;
    }

    private void Update()
    {
        if (player==null || !player.gameObject.activeInHierarchy) return;
        if (Vector3.Distance(player.position, transform.position) <= dis)
        {
            if (Input.GetMouseButtonDown(0) && IsMouseEnter)
            {
                shop.OpenShop();
            }
        }
        else
        {
            shop.CloseShop();
        }
    }
}
