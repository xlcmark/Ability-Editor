using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PicHero : MonoBehaviour
{
    public static PicHero instance;
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    public HeroButton prefab;
    public Image Player;
    public List<Unit> heros=new List<Unit>();
    Unit selectOne;
    private void Start()
    {
        for (int i = 0; i < heros.Count; i++)
        {
            var clone = Instantiate(prefab, transform);
            clone.SetHero( heros[i]);
        }
    }
    public void SelectHero(Unit _unit)
    {
        Player.sprite = _unit.UnitPic;
        selectOne = _unit;
    }
    public void OkeyToSelect()
    {
        if (selectOne == null) return;
        selectOne.team = Unit.Team.blue;
        GM.Instance.player = selectOne;
        SceneManager.LoadScene("SampleScene");
    }
}
