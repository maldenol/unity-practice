using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadSphere : MonoBehaviour {
    public uint RADIUS = 1;
    public uint LOD    = 2;

    void Awake() {
        Mesh mesh = new Mesh();
        mesh.name = "Procedural Quad Sphere";

        // Level-Of-Details (count of quads along one side)
        float xyQuadSize = 1f / (int)LOD;      // discrete quad's side xy size
        float uQuadSize  = 1f / 4f / (int)LOD; // discrete quad's side u size
        float vQuadSize  = 1f / 3f / (int)LOD; // discrete quad's side v size
        float halfSize   = 1f / 2f;            // half of the SIZE
        int quadLOD      = (int)(LOD * LOD);   // LOD^2

        int quadsPerSideCount     = quadLOD;                   // discrete quads per side count
        int verticesPerSideCount  = 4 * quadsPerSideCount;     // vertices per side count
        int verticesCount         = 6 * verticesPerSideCount;  // vertices count
        int trianglesPerSideCount = 2 * quadsPerSideCount * 3; // indexes actually, 3 indexes for each triangle
        int trianglesCount        = 6 * trianglesPerSideCount; // indexes actually, 3 indexes for each triangle

        Vector3[] vertices = new Vector3[verticesCount];
        Vector3[] normals  = new Vector3[verticesCount];
        Vector2[] uvs      = new Vector2[verticesCount];
        int[] triangles    = new int[trianglesCount];

        // making cube for further transformation
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

        // projecting cube on sphere
        for (int v = 0; v < verticesCount; ++v) {
            vertices[v] = vertices[v].normalized * RADIUS;
            normals[v]  = vertices[v].normalized;
        }

        mesh.vertices  = vertices;
        mesh.normals   = normals;
        mesh.uv        = uvs;
        mesh.triangles = triangles;

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
