using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SilverlightShadowCasting;

public class Fow_FOVCalculator
{
    private Texture2D IsOpaque;
    private bool[,] FOV;
    private Vector3 FowBeginPos;
    private float delta;
    private List<Color> Brushes = new List<Color>();//不同顏色代表不同草叢

    public struct UnitPosInfo
    {
        public Unit unit;
        public Fow_pos pos;
        public UnitPosInfo(Unit _unit,Fow_pos _pos) { unit = _unit; pos = _pos; }
    }
    private Dictionary<Unit.Team,List<UnitPosInfo>> TeamsPos;

    public Fow_FOVCalculator(Texture2D _IsOpaque,Vector3 _fowBeginPos,float _delta)
    {
        IsOpaque = _IsOpaque;
        FowBeginPos = _fowBeginPos;
        delta = _delta;
    }

    private bool GetOpaque(int x,int y)
    {
        Color pixColor = IsOpaque.GetPixel(x, y);
        if (pixColor.a == 0) return false;
        if (Brushes.Contains(pixColor)) return false;//草叢有人就不是障礙物
        return true;
    }
    private void SetFOV(int x,int y)
    {
        if(x<FOV.GetLength(0) && y< FOV.GetLength(1) && x>=0 && y>=0)
            FOV[x ,y] = true;
    }
    private void UpdateTeamsPos()
    {
        TeamsPos = new Dictionary<Unit.Team, List<UnitPosInfo>>();
        foreach (var _team in UnitManeger.instance.Teams)
        {
            List<UnitPosInfo> unitPosInfos = new List<UnitPosInfo>();
            foreach (var _unit in _team.Value)
            {
                unitPosInfos.Add(new UnitPosInfo(_unit, WorldPosToFOWMap(_unit.transform.position)));
            }
            TeamsPos.Add(_team.Key, unitPosInfos);
        }
    }

    public Dictionary<Unit.Team,bool[,]> CalculateFOV()
    {
        UpdateTeamsPos();

        Dictionary<Unit.Team, bool[,]> fovs = new Dictionary<Unit.Team, bool[,]>();

        foreach (var _team in TeamsPos)
        {
            CalculateBrush(_team.Key);

            bool[,]fov= CalculateTeamVision(_team.Key);
            fovs.Add(_team.Key, fov);
        }
        return fovs;
    }
    //計算完草叢才能計算視野
    private bool[,] CalculateTeamVision(Unit.Team _team)
    {
        FOV = new bool[IsOpaque.width, IsOpaque.height];
        List<UnitPosInfo> unitPosInfos;
        if (TeamsPos.TryGetValue(_team, out unitPosInfos))//同隊 先做視野運算
        {
            if (unitPosInfos == null) return null;
            for (int i = 0; i < unitPosInfos.Count; i++)
            {
                Fow_pos fowPos = unitPosInfos[i].pos;
                //set fov
                ShadowCaster.ComputeFieldOfViewWithShadowCasting(fowPos.x, fowPos.y, unitPosInfos[i].unit.visionRange, GetOpaque, SetFOV);
            }
        }
        //設定完fov  設定敵隊單位裡的Isvision
        foreach (var team in TeamsPos)
        {
            if (team.Value == null) continue;
            if(team.Key!=_team)//敵隊
            {
                for (int i = 0; i < team.Value.Count; i++)
                {
                    Fow_pos fowPos = team.Value[i].pos;
                    bool isVisible = FOV[fowPos.x, fowPos.y];
                    team.Value[i].unit.SetTeamVision(_team, isVisible);
                }
            }
        }
        return FOV;
    }
    //計算哪幾個草叢有人
    private void CalculateBrush(Unit.Team _team)
    {
        Brushes.Clear();
        List<UnitPosInfo> unitPosInfos;
        if (TeamsPos.TryGetValue(_team, out unitPosInfos))
        {
            if (unitPosInfos == null) return;
            for (int i = 0; i < unitPosInfos.Count; i++)
            {
                //check pos if in brush
                Fow_pos fowPos = unitPosInfos[i].pos;
                Color pixColor = IsOpaque.GetPixel(fowPos.x, fowPos.y);
                if (pixColor.a > 0)//在草叢
                {
                    Brushes.Add(pixColor);
                    unitPosInfos[i].unit.SetIsInBrush(true);
                }
                else//不在草叢
                {
                    unitPosInfos[i].unit.SetIsInBrush(false);
                }
            }
        }
    }

    private Fow_pos WorldPosToFOWMap(Vector3 pos)
    {
        int x = Mathf.FloorToInt((pos.x - FowBeginPos.x) / delta);
        int y = Mathf.FloorToInt((pos.z - FowBeginPos.z) / delta);
        return new Fow_pos(x, y);
    }
}
