using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultScript : MonoBehaviour {
    public int resolutionX = 256;
    public int resolutionY = 256;
    public int colorDepth = 24;

    public ComputeShader computeShader;
    public RenderTexture renderTexture;

    void OnRenderImage(RenderTexture src, RenderTexture dst) {
        if (renderTexture == null) {
            renderTexture = new RenderTexture(resolutionX, resolutionY, colorDepth);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
        }

        computeShader.SetTexture(0, "renderTexture", renderTexture);
        computeShader.SetInt("resolutionX", renderTexture.width);
        computeShader.SetInt("resolutionY", renderTexture.height);
        computeShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);

        Graphics.Blit(renderTexture, dst);
    }
}
