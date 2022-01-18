using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour {
    public uint RADIUS = 1;
    public uint LOD = 2;

    void Awake() {
        Mesh mesh = new Mesh();
        mesh.name = "Procedural Sphere";

        float step = 1.0f / LOD; // discrete quad side size (optimization)
        int quadLOD = (int)(LOD * LOD); // LOD^2 (optimization)

        List<Vector3> vertices = new List<Vector3>();
        int[] triangles = new int[quadLOD * 6 * 6]; // 6 sides of 2*LOD^2 triangles (3 indexes each) each

        // making cube for further transformation
        for (int j = 0; j < 6; ++j) { // the whole cube
            for (int i = 0; i < quadLOD; ++i) { // single side
                float lx = i % LOD * step - 0.5f;
                float rx = (i % LOD + 1) * step - 0.5f;
                float uy = (i / LOD + 1) * step - 0.5f;
                float dy = i / LOD * step - 0.5f;

                Vector3 lu = new Vector3(lx, uy, 0);
                Vector3 ru = new Vector3(rx, uy, 0);
                Vector3 ld = new Vector3(lx, dy, 0);
                Vector3 rd = new Vector3(rx, dy, 0);

                int sideTriangleIndexOffset = j * quadLOD * 6; // offset for triangles index according to side index (j)
                int sideVertexIndexOffset = j * quadLOD * 4;   // offset for vertices index according to side index (j)
                triangles[sideTriangleIndexOffset + i * 6] = sideVertexIndexOffset + i * 4;         // left-up
                triangles[sideTriangleIndexOffset + i * 6 + 1] = sideVertexIndexOffset + i * 4 + 1; // left-up
                triangles[sideTriangleIndexOffset + i * 6 + 2] = sideVertexIndexOffset + i * 4 + 2; // left-up
                triangles[sideTriangleIndexOffset + i * 6 + 3] = sideVertexIndexOffset + i * 4 + 1; // right-down
                triangles[sideTriangleIndexOffset + i * 6 + 4] = sideVertexIndexOffset + i * 4 + 3; // right-down
                triangles[sideTriangleIndexOffset + i * 6 + 5] = sideVertexIndexOffset + i * 4 + 2; // right-down

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

                vertices.Add(lu);
                vertices.Add(ru);
                vertices.Add(ld);
                vertices.Add(rd);
            }
        }

        for (int i = 0; i < vertices.Count; ++i) {
            vertices[i] = vertices[i].normalized * RADIUS;
        }

        mesh.SetVertices(vertices);
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
