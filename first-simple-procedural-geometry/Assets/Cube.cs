using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour {
    public uint SIZE = 2;
    public uint LOD  = 2;

    void Awake() {
        Mesh mesh = new Mesh();
        mesh.name = "Procedural Cube";

        // 8 vertices
        // List<Vector3> vertices = new List<Vector3>(){
        //     new Vector3(-1, 1, 1),   // z- left-up
        //     new Vector3(1, 1, 1),    // z- right-up
        //     new Vector3(-1, -1, 1),  // z- left-down
        //     new Vector3(1, -1, 1),   // z- right-down
        //     new Vector3(-1, 1, -1),  // z+ left-up
        //     new Vector3(1, 1, -1),   // z+ right-up
        //     new Vector3(-1, -1, -1), // z+ left-down
        //     new Vector3(1, -1, -1),  // z+ right-down
        // };
        //
        // int[] triangles = new int[]{
        //     0, 4, 2, // x- left-up
        //     4, 6, 2, // x- right-down
        //     5, 1, 7, // x+ left-up
        //     1, 3, 7, // x+ right-down
        //     6, 7, 2, // y- left-up
        //     7, 3, 2, // y- right-down
        //     0, 1, 4, // y+ left-up
        //     1, 5, 4, // y+ right-down
        //     4, 5, 6, // z- left-up
        //     5, 7, 6, // z- right-down
        //     1, 0, 3, // z+ left-up
        //     0, 2, 3, // z+ right-down
        // };

        // 24 vertices
        // List<Vector3> vertices = new List<Vector3>(){
        //     new Vector3(-1, 1, 1),   // x- left-up
        //     new Vector3(-1, 1, -1),  // x- right-up
        //     new Vector3(-1, -1, 1),  // x- left-down
        //     new Vector3(-1, -1, -1), // x- right-down
        //     new Vector3(1, 1, -1),   // x+ left-up
        //     new Vector3(1, 1, 1),    // x+ right-up
        //     new Vector3(1, -1, -1),  // x+ left-down
        //     new Vector3(1, -1, 1),   // x+ right-down
        //     new Vector3(-1, -1, -1), // y- left-up
        //     new Vector3(1, -1, -1),  // y- right-up
        //     new Vector3(-1, -1, 1),  // y- left-down
        //     new Vector3(1, -1, 1),   // y- right-down
        //     new Vector3(-1, 1, 1),   // y+ left-up
        //     new Vector3(1, 1, 1),    // y+ right-up
        //     new Vector3(-1, 1, -1),  // y+ left-down
        //     new Vector3(1, 1, -1),   // y+ right-down
        //     new Vector3(-1, 1, -1),  // z- left-up
        //     new Vector3(1, 1, -1),   // z- right-up
        //     new Vector3(-1, -1, -1), // z- left-down
        //     new Vector3(1, -1, -1),  // z- right-down
        //     new Vector3(1, 1, 1),    // z+ left-up
        //     new Vector3(-1, 1, 1),   // z+ right-up
        //     new Vector3(1, -1, 1),   // z+ left-down
        //     new Vector3(-1, -1, 1),  // z+ right-down
        // };
        //
        // int[] triangles = new int[]{
        //     0, 1, 2,    // x- left-up
        //     1, 3, 2,    // x- right-down
        //     4, 5, 6,    // x+ left-up
        //     5, 7, 6,    // x+ right-down
        //     8, 9, 10,   // y- left-up
        //     9, 11, 10,  // y- right-down
        //     12, 13, 14, // y+ left-up
        //     13, 15, 14, // y+ right-down
        //     16, 17, 18, // z- left-up
        //     17, 19, 18, // z- right-down
        //     20, 21, 22, // z+ left-up
        //     21, 23, 22, // z+ right-down
        // };

        // Level-Of-Details (count of quads along one side)
        float xyQuadSize = (float)SIZE / (int)LOD; // discrete quad's side xy size
        float uQuadSize  = 1f / 4f / (int)LOD;     // discrete quad's side u size
        float vQuadSize  = 1f / 3f / (int)LOD;     // discrete quad's side v size
        float halfSize   = SIZE / 2f;              // half of the SIZE
        int quadLOD      = (int)(LOD * LOD);       // LOD^2

        int quadsPerSideCount     = quadLOD;                   // discrete quads per side count
        int verticesPerSideCount  = 4 * quadsPerSideCount;     // vertices per side count
        int verticesCount         = 6 * verticesPerSideCount;  // vertices count
        int trianglesPerSideCount = 2 * quadsPerSideCount * 3; // indexes actually, 3 indexes for each triangle
        int trianglesCount        = 6 * trianglesPerSideCount; // indexes actually, 3 indexes for each triangle

        if (verticesCount > 65535) mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        Vector3[] vertices = new Vector3[verticesCount];
        Vector3[] normals  = new Vector3[verticesCount];
        Vector2[] uvs      = new Vector2[verticesCount];
        int[] triangles    = new int[trianglesCount];

        for (int s = 0; s < 6; ++s) { // the whole cube
            for (int v = 0; v < quadsPerSideCount; ++v) { // single side
                int row      = v / (int)LOD;
                int column   = v % (int)LOD;
                float leftX  = column * xyQuadSize - halfSize;
                float rightX = (column + 1) * xyQuadSize - halfSize;
                float downY  = row * xyQuadSize - halfSize;
                float upY    = (row + 1) * xyQuadSize - halfSize;
                float leftU  = column * uQuadSize;
                float rightU = (column + 1) * uQuadSize;
                float downV  = row * vQuadSize;
                float upV    = (row + 1) * vQuadSize;

                Vector3 lu = new Vector3(leftX, upY, 0);
                Vector3 ru = new Vector3(rightX, upY, 0);
                Vector3 ld = new Vector3(leftX, downY, 0);
                Vector3 rd = new Vector3(rightX, downY, 0);

                Vector3 n = Vector3.back;

                Vector2 luUV = new Vector2(leftU, upV);
                Vector2 ruUV = new Vector2(rightU, upV);
                Vector2 ldUV = new Vector2(leftU, downV);
                Vector2 rdUV = new Vector2(rightU, downV);

                // placing the side on its place
                switch (s) {
                case 0: // x-
                    lu = Quaternion.AngleAxis(90, Vector3.up) * lu;
                    ru = Quaternion.AngleAxis(90, Vector3.up) * ru;
                    ld = Quaternion.AngleAxis(90, Vector3.up) * ld;
                    rd = Quaternion.AngleAxis(90, Vector3.up) * rd;

                    n = Quaternion.AngleAxis(90, Vector3.up) * n;

                    lu.x -= halfSize;
                    ru.x -= halfSize;
                    ld.x -= halfSize;
                    rd.x -= halfSize;

                    luUV.x += 0f / 4f;
                    luUV.y += 1f / 3f;
                    ruUV.x += 0f / 4f;
                    ruUV.y += 1f / 3f;
                    ldUV.x += 0f / 4f;
                    ldUV.y += 1f / 3f;
                    rdUV.x += 0f / 4f;
                    rdUV.y += 1f / 3f;
                    break;
                case 1: // x+
                    lu = Quaternion.AngleAxis(-90, Vector3.up) * lu;
                    ru = Quaternion.AngleAxis(-90, Vector3.up) * ru;
                    ld = Quaternion.AngleAxis(-90, Vector3.up) * ld;
                    rd = Quaternion.AngleAxis(-90, Vector3.up) * rd;

                    n = Quaternion.AngleAxis(-90, Vector3.up) * n;

                    lu.x += halfSize;
                    ru.x += halfSize;
                    ld.x += halfSize;
                    rd.x += halfSize;

                    luUV.x += 2f / 4f;
                    luUV.y += 1f / 3f;
                    ruUV.x += 2f / 4f;
                    ruUV.y += 1f / 3f;
                    ldUV.x += 2f / 4f;
                    ldUV.y += 1f / 3f;
                    rdUV.x += 2f / 4f;
                    rdUV.y += 1f / 3f;
                    break;
                case 2: // y-
                    lu = Quaternion.AngleAxis(-90, Vector3.right) * lu;
                    ru = Quaternion.AngleAxis(-90, Vector3.right) * ru;
                    ld = Quaternion.AngleAxis(-90, Vector3.right) * ld;
                    rd = Quaternion.AngleAxis(-90, Vector3.right) * rd;

                    n = Quaternion.AngleAxis(-90, Vector3.right) * n;

                    lu.y -= halfSize;
                    ru.y -= halfSize;
                    ld.y -= halfSize;
                    rd.y -= halfSize;

                    luUV.x += 1f / 4f;
                    luUV.y += 0f / 3f;
                    ruUV.x += 1f / 4f;
                    ruUV.y += 0f / 3f;
                    ldUV.x += 1f / 4f;
                    ldUV.y += 0f / 3f;
                    rdUV.x += 1f / 4f;
                    rdUV.y += 0f / 3f;
                    break;
                case 3: // y+
                    lu = Quaternion.AngleAxis(90, Vector3.right) * lu;
                    ru = Quaternion.AngleAxis(90, Vector3.right) * ru;
                    ld = Quaternion.AngleAxis(90, Vector3.right) * ld;
                    rd = Quaternion.AngleAxis(90, Vector3.right) * rd;

                    n = Quaternion.AngleAxis(90, Vector3.right) * n;

                    lu.y += halfSize;
                    ru.y += halfSize;
                    ld.y += halfSize;
                    rd.y += halfSize;

                    luUV.x += 1f / 4f;
                    luUV.y += 2f / 3f;
                    ruUV.x += 1f / 4f;
                    ruUV.y += 2f / 3f;
                    ldUV.x += 1f / 4f;
                    ldUV.y += 2f / 3f;
                    rdUV.x += 1f / 4f;
                    rdUV.y += 2f / 3f;
                    break;
                case 4: // z-
                    lu.z -= halfSize;
                    ru.z -= halfSize;
                    ld.z -= halfSize;
                    rd.z -= halfSize;

                    luUV.x += 3f / 4f;
                    luUV.y += 1f / 3f;
                    ruUV.x += 3f / 4f;
                    ruUV.y += 1f / 3f;
                    ldUV.x += 3f / 4f;
                    ldUV.y += 1f / 3f;
                    rdUV.x += 3f / 4f;
                    rdUV.y += 1f / 3f;
                    break;
                case 5: // z+
                    lu = Quaternion.AngleAxis(180, Vector3.up) * lu;
                    ru = Quaternion.AngleAxis(180, Vector3.up) * ru;
                    ld = Quaternion.AngleAxis(180, Vector3.up) * ld;
                    rd = Quaternion.AngleAxis(180, Vector3.up) * rd;

                    n = Quaternion.AngleAxis(180, Vector3.up) * n;

                    lu.z += halfSize;
                    ru.z += halfSize;
                    ld.z += halfSize;
                    rd.z += halfSize;

                    luUV.x += 1f / 4f;
                    luUV.y += 1f / 3f;
                    ruUV.x += 1f / 4f;
                    ruUV.y += 1f / 3f;
                    ldUV.x += 1f / 4f;
                    ldUV.y += 1f / 3f;
                    rdUV.x += 1f / 4f;
                    rdUV.y += 1f / 3f;
                    break;
                }

                int vertexOffset   = s * verticesPerSideCount + v * 4;
                int triangleOffset = s * quadLOD * 6 + v * 6;

                vertices[vertexOffset]     = lu;
                vertices[vertexOffset + 1] = ru;
                vertices[vertexOffset + 2] = ld;
                vertices[vertexOffset + 3] = rd;

                normals[vertexOffset]     = n;
                normals[vertexOffset + 1] = n;
                normals[vertexOffset + 2] = n;
                normals[vertexOffset + 3] = n;

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

        // for (int v = 0; v < verticesCount; ++v) { // Perlin noise
        //     float amplitude = 0.05f;
        //     float frequency = 10f;

        //     for (int l = 0; l < 5; ++l) { // single noise layer (pass)
        //         float noiseValue = Mathf.PerlinNoise(frequency * uvs[v].x, frequency * uvs[v].y) * 2f - 1f;
        //         vertices[v]     += amplitude * normals[v] * noiseValue;
        //         amplitude       /= 2f;
        //         frequency       *= 2f;
        //     }
        // }

        mesh.vertices  = vertices;
        mesh.normals   = normals;
        mesh.uv        = uvs;
        mesh.triangles = triangles;

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
