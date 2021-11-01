using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroButton : MonoBehaviour
{
    public Unit unit;
    public Image image;
    public void SetHero(Unit _unit)
    {
        unit = _unit;
        image.sprite = _unit.UnitPic;
    }
    public void SeletHero()
    {
        if (unit == null) return;
        PicHero.instance.SelectHero(unit);
    }
}
