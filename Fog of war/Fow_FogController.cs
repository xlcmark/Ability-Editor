using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Fow_FogController
{

    public static void DrawFog(bool[,] FOV, Texture2D FogTexture,Unit.Team _team)
    {
        UpdateFogTexture(FOV,FogTexture);//更新霧

        foreach (var team in UnitManeger.instance.Teams)
        {
            for (int i = 0; i < team.Value.Count; i++)
            {
                SetUnitIsVisibleInScene(team.Value[i], _team);//更新敵人在友軍視野是否可見
            }
        }
    }
    private static void UpdateFogTexture(bool[,] FOV,Texture2D FogTexture)
    {
        for (int i = 0; i < FOV.GetLength(0); i++)
        {
            for (int j = 0; j < FOV.GetLength(1); j++)
            {
                if (FOV[i, j] == true)
                {
                    FogTexture.SetPixel(i, j, Color.black);
                }
                else
                {
                    FogTexture.SetPixel(i, j, Color.clear);
                }
            }
        }
        FogTexture.Apply();
    }
    private static void SetUnitIsVisibleInScene(Unit unit,Unit.Team team)
    {
        //看得見
        if (unit.GetTeamVision(team))
            ChangedUnitLayer(unit, LayerMask.NameToLayer("Unit"));
        else//看不見
            ChangedUnitLayer(unit, LayerMask.NameToLayer("Invisible"));
    }
    private static void ChangedUnitLayer(Unit unit, int _layer)
    {
        for (int i = 0; i < unit.RendererGOs.Count; i++)
        {
            unit.RendererGOs[i].layer = _layer;
        }
        unit.ChangedOwnerVfxLayer(_layer);
    }

    /* another way
    private void TextureUpdatePlus(bool[,] FOV, Texture2D FogTexture)
    {
        //把FOV放大四倍
        for (int i = 0; i < FOV.GetLength(0) - 1; i++)
        {
            for (int j = 0; j < FOV.GetLength(1) - 1; j++)
            {
                int binary = 0;
                binary += FOV[i, j] ? 1 : 0;
                binary += FOV[i + 1, j] ? 2 : 0;
                binary += FOV[i, j + 1] ? 4 : 0;
                binary += FOV[i + 1, j + 1] ? 8 : 0;

                FogTexture.SetPixels(i * 4, j * 4, 4, 4, GetColorArray(binary));
            }
        }
        FogTexture.Apply();//gpu
    }
    //16種填色情形
    private Color[] GetColorArray(int i)
    {
        Color[] colors;
        Color ColorHalf = new Color(0, 0, 0, 0.25f);
        switch (i)
        {
            case 0:
                colors = new Color[]{
                Color.clear, Color.clear, Color.clear, Color.clear,
                Color.clear, Color.clear, Color.clear, Color.clear,
                Color.clear, Color.clear, Color.clear, Color.clear,
                Color.clear, Color.clear, Color.clear, Color.clear,
                };
                break;
            case 1:
                colors = colors = new Color[]{
                Color.black, ColorHalf, Color.clear, Color.clear,
                ColorHalf, Color.clear, Color.clear, Color.clear,
                Color.clear, Color.clear, Color.clear, Color.clear,
                Color.clear, Color.clear, Color.clear, Color.clear,
                };
                break;
            case 2:
                colors = new Color[]{
                Color.clear, Color.clear, ColorHalf, Color.black,
                Color.clear, Color.clear, Color.clear, ColorHalf,
                Color.clear, Color.clear, Color.clear, Color.clear,
                Color.clear, Color.clear, Color.clear, Color.clear,
                };
                break;
            case 3:
                colors = new Color[]{
                Color.black, Color.black, Color.black, Color.black,
                Color.black, Color.black, Color.black, Color.black,
                Color.clear, Color.clear, Color.clear, Color.clear,
                Color.clear, Color.clear, Color.clear, Color.clear,
                };
                break;
            case 4:
                colors = new Color[]{
                Color.clear, Color.clear, Color.clear, Color.clear,
                Color.clear, Color.clear, Color.clear, Color.clear,
                ColorHalf, Color.clear, Color.clear, Color.clear,
                Color.black, ColorHalf, Color.clear, Color.clear,
                };
                break;
            case 5:
                colors = new Color[]{
                Color.black, Color.black, Color.clear, Color.clear,
                Color.black, Color.black, Color.clear, Color.clear,
                Color.black, Color.black, Color.clear, Color.clear,
                Color.black, Color.black, Color.clear, Color.clear,
                };
                break;
            case 6:
                colors = new Color[]{
                Color.clear, Color.clear, ColorHalf, Color.black,
                Color.clear, Color.clear, Color.clear, ColorHalf,
                ColorHalf, Color.clear, Color.clear, Color.clear,
                Color.black, ColorHalf, Color.clear, Color.clear,
                };
                break;
            case 7:
                colors = new Color[]{
                Color.black, Color.black, Color.black, Color.black,
                Color.black, Color.black, Color.black, Color.black,
                Color.black, Color.black, Color.black, ColorHalf,
                Color.black, Color.black, ColorHalf, Color.clear,
                };
                break;
            case 8:
                colors = new Color[]{
                Color.clear, Color.clear, Color.clear, Color.clear,
                Color.clear, Color.clear, Color.clear, Color.clear,
                Color.clear, Color.clear, Color.clear, ColorHalf,
                Color.clear, Color.clear, ColorHalf, Color.black,
                };
                break;
            case 9:
                colors = new Color[]{
                Color.black, ColorHalf, Color.clear, Color.clear,
                ColorHalf, Color.clear, Color.clear, Color.clear,
                Color.clear, Color.clear, Color.clear, ColorHalf,
                Color.clear, Color.clear, ColorHalf, Color.black,
                };
                break;
            case 10:
                colors = new Color[]{
                Color.clear, Color.clear, Color.black, Color.black,
                Color.clear, Color.clear, Color.black, Color.black,
                Color.clear, Color.clear, Color.black, Color.black,
                Color.clear, Color.clear, Color.black, Color.black,
                };
                break;
            case 11:
                colors = new Color[]{
                Color.black, Color.black, Color.black, Color.black,
                Color.black, Color.black, Color.black, Color.black,
                ColorHalf, Color.black, Color.black, Color.black,
                Color.clear, ColorHalf, Color.black, Color.black,
                };
                break;
            case 12:
                colors = new Color[]{
                Color.clear, Color.clear, Color.clear, Color.clear,
                Color.clear, Color.clear, Color.clear, Color.clear,
                Color.black, Color.black, Color.black, Color.black,
                Color.black, Color.black, Color.black, Color.black,
                };
                break;
            case 13:
                colors = new Color[]{
                Color.black, Color.black, ColorHalf, Color.clear,
                Color.black, Color.black, Color.black, ColorHalf,
                Color.black, Color.black, Color.black, Color.black,
                Color.black, Color.black, Color.black, Color.black,
                };
                break;
            case 14:
                colors = new Color[]{
                Color.clear, ColorHalf, Color.black, Color.black,
                ColorHalf, Color.black, Color.black, Color.black,
                Color.black, Color.black, Color.black, Color.black,
                Color.black, Color.black, Color.black, Color.black,
                };
                break;
            case 15:
                colors = new Color[]{
                Color.black, Color.black, Color.black, Color.black,
                Color.black, Color.black, Color.black, Color.black,
                Color.black, Color.black, Color.black, Color.black,
                Color.black, Color.black, Color.black, Color.black,
                };
                break;
            default:
                colors = new Color[]{
                Color.black, Color.black, Color.black, Color.black,
                Color.black, Color.black, Color.black, Color.black,
                Color.black, Color.black, Color.black, Color.black,
                Color.black, Color.black, Color.black, Color.black,
                };
                break;
        }
        return colors;
    }
    */
}
