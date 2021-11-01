using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fow_RendererFog : MonoBehaviour
{
    public Material projectorMaterial;
    public float blendSpeed;

    private Texture2D fogTexture;

    private RenderTexture prevTexture;
    private RenderTexture currTexture;
    private Projector projector;

    private float blendAmount;

    public Fow_WallGenerator wallGenerator;
    private Fow_FOVCalculator fOVCalculator;

    public Shader blurShader;
    private Material blurMaterial;

    public Unit.Team PlayerTeam;

    private void Awake()
    {
        //team
        PlayerTeam = HeroManager.instance.heroes[0].prefab.team;

        //create calculator
        fogTexture = new Texture2D(wallGenerator.map.width, wallGenerator.map.height, TextureFormat.Alpha8,false);
        fOVCalculator = new Fow_FOVCalculator(wallGenerator.map,wallGenerator.BeginPos,wallGenerator.delta);
        //
        projector = GetComponent<Projector>();
        projector.enabled = true;

        prevTexture = GenerateRenderTexture();
        currTexture = GenerateRenderTexture();

        // Projector materials aren't instanced, resulting in the material asset getting changed.
        // Instance it here to prevent us from having to check in or discard these changes manually.
        projector.material = new Material(projectorMaterial);

        projector.material.SetTexture("_PrevTexture", prevTexture);
        projector.material.SetTexture("_CurrTexture", currTexture);

        //blur
        blurMaterial = new Material(blurShader);
        blurMaterial.SetFloat("_Offset", 0.005f);

        StartNewBlend();
    }

    RenderTexture GenerateRenderTexture()
    {
        RenderTexture rt = new RenderTexture(
            fogTexture.width,
            fogTexture.height,
            0,
            RenderTextureFormat.ARGB32
            )
        { filterMode = FilterMode.Bilinear };
        rt.antiAliasing = 8;

        return rt;
    }

    public void StartNewBlend()
    {
        StopCoroutine(BlendFog());
        blendAmount = 0;
        //calculate fov
        Dictionary<Unit.Team, bool[,]> fovs;
        fovs= fOVCalculator.CalculateFOV();
        //draw fogTexture
        if(fovs.TryGetValue(PlayerTeam,out bool[,] fov))
        {
            Fow_FogController.DrawFog(fov, fogTexture, PlayerTeam);
        }

        // Swap the textures
        Graphics.Blit(currTexture, prevTexture);
        Graphics.Blit(fogTexture, currTexture);

        //blur
        RenderTexture temp = RenderTexture.GetTemporary(fogTexture.width, fogTexture.height, 0);
        Graphics.Blit(currTexture, temp, blurMaterial);
        for (int i = 0; i <= 2; i++)
        {
            RenderTexture temp2 = RenderTexture.GetTemporary(fogTexture.width / 2, fogTexture.height / 2, 0);
            Graphics.Blit(temp, temp2, blurMaterial);
            RenderTexture.ReleaseTemporary(temp);
            temp = temp2;
        }
        Graphics.Blit(temp, currTexture, blurMaterial);

        StartCoroutine(BlendFog());
        RenderTexture.ReleaseTemporary(temp);
    }
    //blend
    IEnumerator BlendFog()
    {
        while (blendAmount < 1)
        {
            // increase the interpolation amount
            blendAmount += Time.deltaTime * blendSpeed;
            // Set the blend property so the shader knows how much to lerp
            // by when checking the alpha value
            projector.material.SetFloat("_Blend", blendAmount);
            yield return null;
        }
        // once finished blending, swap the textures and start a new blend
        StartNewBlend();
    }

}

public struct Fow_pos
{
    public int x, y;
    public Fow_pos(int _x,int _y) { x = _x; y = _y; }
}