using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultScript : MonoBehaviour {
    public int resolutionX = 256;
    public int resolutionY = 256;
    public int colorDepth  = 24;

    private ComputeShader computeShader;

    private RenderTexture renderTexture;

    private Material material;

    void Awake() {
        // Creating texture buffer
        renderTexture                   = new RenderTexture(
            resolutionX,
            resolutionY,
            colorDepth
        );
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        // Loading compute shader
        computeShader = Resources.Load<ComputeShader>("DefaultCS");

        // Passing buffers and variables to compute shader
        computeShader.SetTexture(0, "renderTexture", renderTexture);
        computeShader.SetInt("resolutionX", renderTexture.width);
        computeShader.SetInt("resolutionY", renderTexture.height);

        // Getting material
        material = GetComponent<Renderer>().material;
    }

    void Update() {
        // Running compute shader
        computeShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);

        // Replacing material texture with ours
        material.SetTexture("_MainTex", renderTexture);
    }
}
