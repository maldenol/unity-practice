using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MandelbrotSetScript : MonoBehaviour {
    public uint resolution   = 1024;
    public uint detalization = 255;

    public ComputeShader computeShader;

    private RenderTexture outputTexture;
    private ComputeBuffer bordersBuffer;
    private ComputeBuffer colorsBuffer;

    private int kernelIndex;

    void OnRenderImage(RenderTexture src, RenderTexture dst) {
        if (outputTexture == null) {
            outputTexture = new RenderTexture((int)resolution, (int)resolution, 32);
            outputTexture.enableRandomWrite = true;
            outputTexture.Create();

            float[] bordersArray = new float[]{-2f, -1f, 0.5f, 1f};
            bordersBuffer = new ComputeBuffer(bordersArray.Length, 4);
            bordersBuffer.SetData(bordersArray);

            Vector4[] colorsArray = new Vector4[detalization];
            for (int i = 0; i < detalization; ++i) {
                float r = (float)i / detalization;
                float g = (float)i / detalization;
                float b = (float)i / detalization;

                colorsArray[i] = new Vector4(r, g, b, 1f);
            }
            colorsBuffer = new ComputeBuffer(colorsArray.Length, 4 * 4);
            colorsBuffer.SetData(colorsArray);

            kernelIndex = computeShader.FindKernel("csMain");
            computeShader.SetTexture(kernelIndex, "textureOut", outputTexture);
            computeShader.SetBuffer(kernelIndex, "borders", bordersBuffer);
            computeShader.SetBuffer(kernelIndex, "colors", colorsBuffer);
            computeShader.SetInt("iterCount", (int)detalization);
        }

        computeShader.Dispatch(kernelIndex, (int)resolution / 32, (int)resolution / 32, 1);

        Graphics.Blit(outputTexture, dst);
    }
}
