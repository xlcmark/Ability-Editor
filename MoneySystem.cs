using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneySystem : MonoBehaviour
{
    public static MoneySystem instance;
    public int Money { private set; get; }
    public int BeginMoney;
    public MoneyNumFadeOut MoneyNumUIPrefab;

    public delegate void OnMoneyChanged(int amount);
    public OnMoneyChanged onMoneyChanged;

    private void Awake()
    {
        if (instance == null) instance = this;
        Init();
    }

    public void GainMoney(int amount)
    {
        Money += amount;
        onMoneyChanged?.Invoke(Money);
    }
    public void LoseMoney(int amount)
    {
        Money -= amount;
        onMoneyChanged?.Invoke(Money);
    }
    private void Init()
    {
        GainMoney(BeginMoney);
        StartCoroutine( GainMoneyByTime());
    }
    private IEnumerator GainMoneyByTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(8);
            GainMoney(5);
        }
    }
}
