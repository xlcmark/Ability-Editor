using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEffectManager : MonoBehaviour
{
    public static GameEffectManager instance;
    public TextAsset effectInfoTxt;

    List<GameEffectInfo> infos;
    public Dictionary<string, List<GameEffect>> effectPool = new Dictionary<string, List<GameEffect>>();

    private void Awake()
    {
        if (instance == null) instance = this;
        infos = EffectInfoLoad.ReadInfo(effectInfoTxt);//把txt轉成info
        //遊戲初始加入特效近池裡
        for (int i = 0; i < infos.Count; i++)
        {
            for (int j = 0; j < infos[i].repeatCount; j++)
            {
                CreateEffect(infos[i]);
            }
        }
    }
    //增加一個特效進特效池裡
    GameEffect CreateEffect(GameEffectInfo info)
    {
        GameObject prefab = (GameObject)Resources.Load("Effect/" + info.filePath);
        if (prefab == null)
        {
            Debug.Log("沒有" + "Effect/" + info.filePath);
            return null;
        }
        GameObject clone = Instantiate(prefab);
        GameEffect effect = clone.AddComponent<GameEffect>();
        effect.Init(info);
        if (!effectPool.ContainsKey(info.VfxName))
        {
            effectPool.Add(info.VfxName, new List<GameEffect>());
        }
        effectPool[info.VfxName].Add(effect);
        return effect;
    }

    //添加特效到位置
    public GameEffect AddWorldEffect(string _effName,Vector3 _pos,Quaternion _rot)
    {
        GameEffect effect = GetEffect(_effName);
        if (effect == null) return null;
        effect.transform.position = _pos;
        effect.transform.rotation = _rot;
        effect.Play();
        return effect;
    }
    //添加特效到目標物
    public GameEffect AddTansformEffect(string _effName,Transform _Point,Quaternion _rot)
    {
        GameEffect effect = GetEffect(_effName);
        if (effect == null) return null;
        effect.transform.parent = _Point;
        effect.transform.localPosition = Vector3.zero;
        effect.transform.localScale = Vector3.one;
        if (_rot == Quaternion.identity)
        {
            effect.transform.localRotation = Quaternion.identity;
        }
        else
        {
            effect.transform.rotation = _rot;
        }
        //layer
        effect.gameObject.layer = _Point.root.gameObject.layer;

        effect.Play();
        return effect;
    }

    GameEffect GetEffect(string _effectName)
    {
        List<GameEffect> pool = new List<GameEffect>();
        if (!effectPool.TryGetValue(_effectName,out pool))
        {
            Debug.Log("找不到特效"+ _effectName);
            return null;
        }
        GameEffect effect=null;
        //找未使用的
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i].gameObject.activeInHierarchy)
            {
                continue;
            }
            effect = pool[i];
            break;
        }
        //如果未有可用 創建一個新的
        if (effect == null)
        {
            GameEffectInfo info = infos.Find(x => x.VfxName == _effectName);
            effect = CreateEffect(info);
        }
        return effect;
    }

}
