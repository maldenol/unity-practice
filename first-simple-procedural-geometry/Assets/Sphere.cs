using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour {
    public uint RADIUS = 1;
    public uint LOD    = 2;

    void Awake() {
        Mesh mesh = new Mesh();
        mesh.name = "Procedural Sphere";

        // Level-Of-Details (count of quads along one side)
        float step  = 1.0f / LOD;       // discrete quad's side size (optimization)
        int quadLOD = (int)(LOD * LOD); // LOD^2 (optimization)

        int quadsPerSideCount     = quadLOD;
        int verticesPerSideCount  = 4 * quadLOD;
        int verticesCount         = 6 * verticesPerSideCount;
        int trianglesPerSideCount = 2 * quadLOD * 3; // indexes actually, 3 indexes for each triangle
        int trianglesCount        = 6 * trianglesPerSideCount;

        Vector3[] vertices = new Vector3[verticesCount];
        Vector3[] normals  = new Vector3[verticesCount];
        int[] triangles    = new int[trianglesCount];

        // making cube for further transformation
        for (int j = 0; j < 6; ++j) { // the whole cube
            for (int i = 0; i < quadsPerSideCount; ++i) { // single side
                int row    = i / (int)LOD;
                int column = i % (int)LOD;
                float lx   = column * step - 0.5f;
                float rx   = (column + 1) * step - 0.5f;
                float uy   = (row + 1) * step - 0.5f;
                float dy   = row * step - 0.5f;

                Vector3 lu = new Vector3(lx, uy, 0);
                Vector3 ru = new Vector3(rx, uy, 0);
                Vector3 ld = new Vector3(lx, dy, 0);
                Vector3 rd = new Vector3(rx, dy, 0);

                // placing the side on its place
                switch (j) {
                case 0: // x-
                    lu = Quaternion.AngleAxis(90, Vector3.up) * lu;
                    ru = Quaternion.AngleAxis(90, Vector3.up) * ru;
                    ld = Quaternion.AngleAxis(90, Vector3.up) * ld;
                    rd = Quaternion.AngleAxis(90, Vector3.up) * rd;

                    lu.x -= 0.5f;
                    ru.x -= 0.5f;
                    ld.x -= 0.5f;
                    rd.x -= 0.5f;
                    break;
                case 1: // x+
                    lu = Quaternion.AngleAxis(-90, Vector3.up) * lu;
                    ru = Quaternion.AngleAxis(-90, Vector3.up) * ru;
                    ld = Quaternion.AngleAxis(-90, Vector3.up) * ld;
                    rd = Quaternion.AngleAxis(-90, Vector3.up) * rd;

                    lu.x += 0.5f;
                    ru.x += 0.5f;
                    ld.x += 0.5f;
                    rd.x += 0.5f;
                    break;
                case 2: // y-
                    lu = Quaternion.AngleAxis(-90, Vector3.right) * lu;
                    ru = Quaternion.AngleAxis(-90, Vector3.right) * ru;
                    ld = Quaternion.AngleAxis(-90, Vector3.right) * ld;
                    rd = Quaternion.AngleAxis(-90, Vector3.right) * rd;

                    lu.y -= 0.5f;
                    ru.y -= 0.5f;
                    ld.y -= 0.5f;
                    rd.y -= 0.5f;
                    break;
                case 3: // y+
                    lu = Quaternion.AngleAxis(90, Vector3.right) * lu;
                    ru = Quaternion.AngleAxis(90, Vector3.right) * ru;
                    ld = Quaternion.AngleAxis(90, Vector3.right) * ld;
                    rd = Quaternion.AngleAxis(90, Vector3.right) * rd;

                    lu.y += 0.5f;
                    ru.y += 0.5f;
                    ld.y += 0.5f;
                    rd.y += 0.5f;
                    break;
                case 4: // z-
                    lu.z -= 0.5f;
                    ru.z -= 0.5f;
                    ld.z -= 0.5f;
                    rd.z -= 0.5f;
                    break;
                case 5: // z+
                    lu = Quaternion.AngleAxis(180, Vector3.up) * lu;
                    ru = Quaternion.AngleAxis(180, Vector3.up) * ru;
                    ld = Quaternion.AngleAxis(180, Vector3.up) * ld;
                    rd = Quaternion.AngleAxis(180, Vector3.up) * rd;

                    lu.z += 0.5f;
                    ru.z += 0.5f;
                    ld.z += 0.5f;
                    rd.z += 0.5f;
                    break;
                }

                int vertexOffset   = j * verticesPerSideCount + i * 4;
                int triangleOffset = j * quadLOD * 6 + i * 6;

                vertices[vertexOffset]     = lu;
                vertices[vertexOffset + 1] = ru;
                vertices[vertexOffset + 2] = ld;
                vertices[vertexOffset + 3] = rd;

                triangles[triangleOffset]     = vertexOffset;     // left-up quad
                triangles[triangleOffset + 1] = vertexOffset + 1; // left-up quad
                triangles[triangleOffset + 2] = vertexOffset + 2; // left-up quad
                triangles[triangleOffset + 3] = vertexOffset + 1; // right-down quad
                triangles[triangleOffset + 4] = vertexOffset + 3; // right-down quad
                triangles[triangleOffset + 5] = vertexOffset + 2; // right-down quad
            }
        }

        // projecting cube on sphere
        for (int i = 0; i < verticesCount; ++i) {
            vertices[i] = vertices[i].normalized * RADIUS;
            normals[i]  = vertices[i].normalized;
        }

        mesh.vertices  = vertices;
        mesh.normals   = normals;
        mesh.triangles = triangles;

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
