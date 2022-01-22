using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVSphere : MonoBehaviour {
    public uint RADIUS = 1;
    public uint LOD    = 2;

    void Awake() {
        Mesh mesh = new Mesh();
        mesh.name = "Procedural UV-Sphere";

        // Level-Of-Details (count of rectangles per longitude/latitude)
        int columns        = 2 * (int)LOD;            // longitude
        int rows           = (int)LOD;                // latitude
        float halfRows     = rows / 2f;               // half of the rows count
        int trapezesCount  = columns * rows;          // count of discrete trapezes
        int verticesCount  = 4 * trapezesCount;       // count of vertices
        int trianglesCount = 2 * trapezesCount * 3;   // indexes actually, 3 indexes for each triangle
        float trapezeXDim  = 2f * Mathf.PI / columns; // maximal discrete trapeze's side x dimension
        float trapezeYDim  = Mathf.PI / rows;         // discrete trapeze's side y dimension
        float trapezeUDim  = 1f / columns;            // discrete trapeze's side u dimension
        float trapezeVDim  = 1f / rows;               // discrete trapeze's side v dimension

        Vector3[] vertices = new Vector3[verticesCount];
        Vector3[] normals  = new Vector3[verticesCount];
        Vector2[] uvs      = new Vector2[verticesCount];
        int[] triangles    = new int[trianglesCount];

        for (int c = 0; c < columns; ++c) {
            for (int r = 0; r < rows; ++r) {
                float luX = c * trapezeXDim;
                float luY = (r + 1) * trapezeYDim;
                float ruX = (c + 1) * trapezeXDim;
                float ruY = (r + 1) * trapezeYDim;
                float ldX = c * trapezeXDim;
                float ldY = r * trapezeYDim;
                float rdX = (c + 1) * trapezeXDim;
                float rdY = r * trapezeYDim;

                float luU = 1f - c * trapezeUDim;
                float luV = 1f - (r + 1) * trapezeVDim;
                float ruU = 1f - (c + 1) * trapezeUDim;
                float ruV = 1f - (r + 1) * trapezeVDim;
                float ldU = 1f - c * trapezeUDim;
                float ldV = 1f - r * trapezeVDim;
                float rdU = 1f - (c + 1) * trapezeUDim;
                float rdV = 1f - r * trapezeVDim;

                // Spherical coordinates to Cartesian (formulas from Wikipedia)
                Vector3 lu = new Vector3(
                    -Mathf.Cos(luX) * Mathf.Sin(luY),
                    Mathf.Cos(luY),
                    Mathf.Sin(luX) * Mathf.Sin(luY)
                );
                Vector3 ru = new Vector3(
                    -Mathf.Cos(ruX) * Mathf.Sin(ruY),
                    Mathf.Cos(ruY),
                    Mathf.Sin(ruX) * Mathf.Sin(ruY)
                );
                Vector3 ld = new Vector3(
                    -Mathf.Cos(ldX) * Mathf.Sin(ldY),
                    Mathf.Cos(ldY),
                    Mathf.Sin(ldX) * Mathf.Sin(ldY)
                );
                Vector3 rd = new Vector3(
                    -Mathf.Cos(rdX) * Mathf.Sin(rdY),
                    Mathf.Cos(rdY),
                    Mathf.Sin(rdX) * Mathf.Sin(rdY)
                );

                Vector2 luUV = new Vector2(luU, luV);
                Vector2 ruUV = new Vector2(ruU, ruV);
                Vector2 ldUV = new Vector2(ldU, ldV);
                Vector2 rdUV = new Vector2(rdU, rdV);

                int index          = c * rows + r;
                int vertexOffset   = index * 4;
                int triangleOffset = index * 6;

                vertices[vertexOffset]     = lu * RADIUS;
                vertices[vertexOffset + 1] = ru * RADIUS;
                vertices[vertexOffset + 2] = ld * RADIUS;
                vertices[vertexOffset + 3] = rd * RADIUS;

                normals[vertexOffset]     = lu;
                normals[vertexOffset + 1] = ru;
                normals[vertexOffset + 2] = ld;
                normals[vertexOffset + 3] = rd;

                uvs[vertexOffset]     = luUV;
                uvs[vertexOffset + 1] = ruUV;
                uvs[vertexOffset + 2] = ldUV;
                uvs[vertexOffset + 3] = rdUV;

                triangles[triangleOffset]     = vertexOffset;     // left-up triangle
                triangles[triangleOffset + 1] = vertexOffset + 1; // left-up triangle
                triangles[triangleOffset + 2] = vertexOffset + 2; // left-up triangle
                triangles[triangleOffset + 3] = vertexOffset + 1; // right-down triangle
                triangles[triangleOffset + 4] = vertexOffset + 3; // right-down triangle
                triangles[triangleOffset + 5] = vertexOffset + 2; // right-down triangle
            }
        }

        mesh.vertices  = vertices;
        mesh.normals   = normals;
        mesh.uv        = uvs;
        mesh.triangles = triangles;

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
