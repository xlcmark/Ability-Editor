using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EffectInfoLoad
{
    public static List<GameEffectInfo> ReadInfo(TextAsset _text)
    {
        List<GameEffectInfo> gameEffectInfos=new List<GameEffectInfo>();
        string allInfo = _text.text;
        string[] lineInfo = allInfo.Split('\n');

        if (lineInfo[0] == "特效表")
         {
            Debug.Log("這不是特效表");
            return null;
         }

        for (int i = 2; i < lineInfo.Length; i++)//從i=2開始 o為表的名稱 1為數據名
        {
            string[] unitInfo = lineInfo[i].Split(',');
            if (unitInfo.Length < 3) break;
            int.TryParse(unitInfo[2], out int count);//轉數字
            GameEffectInfo newInfo = new GameEffectInfo(unitInfo[0], unitInfo[1],count);
            gameEffectInfos.Add(newInfo);
        }
        return gameEffectInfos;
    }
}
