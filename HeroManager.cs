using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroManager : MonoBehaviour
{
    public static HeroManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        rebornEfx = Instantiate(rebornEfx);
        Unit _player = GM.Instance.player;
        if (_player != null)
        {
            heroes[0].prefab = _player;
        }
    }
    public Transform spwanPoint;
    public float beginTime;
    public ParticleSystem rebornEfx;
    public CameraController cameraController;
    [System.Serializable]
    public class HeroData
    {
        public Unit prefab;
        public Unit hero;//實體
        public float deathTimer;
    }
    public HeroData[] heroes;

    void Start()
    {
        Invoke( "Spwan",beginTime);
    }
    //出生
    private void Spwan()
    {
        for (int i = 0; i < heroes.Length; i++)
        {
            heroes[i].hero= Instantiate(heroes[i].prefab,spwanPoint.position, spwanPoint.rotation);
            heroes[i].hero.OnDead += OnHeroDie;
            heroes[i].hero.ResetUnit();
            if (i == 0)//控制第一個英雄
                heroes[i].hero.OnInitFinish += SetPlayerUI;
            //efx
            rebornEfx.transform.position = heroes[i].hero.transform.position;
            rebornEfx.Play();
            //camera
            if (cameraController != null)
            {
                if (i == 0)//控制第一個英雄
                    cameraController.CameraMove(new Vector3(cameraController.PanelLimitXmin, 0, cameraController.PanelLimitYmin));
            }
        }
    }
    private void SetPlayerUI()
    {
        UIController.SetUnit(heroes[0].hero);
    }

    //event
    private void OnHeroDie(Unit _unit)
    {
        for (int i = 0; i < heroes.Length; i++)
        {
            if (heroes[i].hero == _unit)
            {
                StartCoroutine(DeadTimer(heroes[i]));
            }
        }
    }
    //timer
    private IEnumerator DeadTimer(HeroData hero)
    {
        UpgradeSystem us = hero.hero.GetComponent<UpgradeSystem>();
        if (us != null)
        {
            hero.deathTimer = HeroDeathTimerData_GM.instance.deathTimer[Mathf.Clamp(us.Level - 1, 0, HeroDeathTimerData_GM.instance.deathTimer.Length - 1)];
            while (hero.deathTimer > 0)
            {
                yield return new WaitForSeconds(.1f);
                hero.deathTimer -= .1f;
            }
            hero.deathTimer = 0;
            Reborn(hero.hero);
        }
    }

    //復活
    private void Reborn(Unit _unit)
    {
        if (_unit.gameObject.activeInHierarchy) return;
        _unit.transform.position = spwanPoint.position;
        _unit.gameObject.SetActive(true);
        _unit.ResetUnit();
        //efx
        rebornEfx.transform.position = _unit.transform.position;
        rebornEfx.Play();
        //camera
        if (cameraController != null)
        {
            if (_unit == heroes[0].hero)//玩家操控
                cameraController.CameraMove(new Vector3( cameraController.PanelLimitXmin,0,cameraController.PanelLimitYmin));
        }
    }


}
