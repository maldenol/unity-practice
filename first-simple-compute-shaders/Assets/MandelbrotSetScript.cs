using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MandelbrotSetScript : MonoBehaviour {
    public uint resolution   = 1024;
    public uint detalization = 255;

    private ComputeShader computeShader;

    private RenderTexture outputTexture;
    private ComputeBuffer bordersBuffer;
    private ComputeBuffer colorsBuffer;

    private int kernelIndex;

    private Material material;

    void Awake() {
        // Creating texture buffer
        outputTexture                   = new RenderTexture(
            (int)resolution,
            (int)resolution,
            32
        );
        outputTexture.enableRandomWrite = true;
        outputTexture.Create();

        // Setting drawing borders (min x, min y, max x, max y)
        float[] bordersArray = new float[]{-2f, -1f, 0.5f, 1f};
        bordersBuffer        = new ComputeBuffer(bordersArray.Length, 4);
        bordersBuffer.SetData(bordersArray);

        // Setting color palette
        Vector4[] colorsArray = new Vector4[detalization];
        for (int i = 0; i < detalization; ++i) {
            float r = (float)i / detalization;
            float g = (float)i / detalization;
            float b = (float)i / detalization;

            colorsArray[i] = new Vector4(r, g, b, 1f);
        }
        colorsBuffer = new ComputeBuffer(colorsArray.Length, 4 * 4);
        colorsBuffer.SetData(colorsArray);

        // Loading compute shader
        computeShader = Resources.Load<ComputeShader>("MandelbrotSetCS");

        // Finding compute shader kernel (optionally)
        kernelIndex = computeShader.FindKernel("csMain");

        // Passing buffers and variables to compute shader
        computeShader.SetTexture(kernelIndex, "textureOut", outputTexture);
        computeShader.SetBuffer(kernelIndex, "borders", bordersBuffer);
        computeShader.SetBuffer(kernelIndex, "colors", colorsBuffer);
        computeShader.SetInt("iterCount", (int)detalization);

        // Getting material
        material = GetComponent<Renderer>().material;
    }

    void Update() {
        // Running compute shader
        computeShader.Dispatch(kernelIndex, (int)resolution / 32, (int)resolution / 32, 1);

        // Replacing material texture with ours
        material.SetTexture("_MainTex", outputTexture);
    }
}
