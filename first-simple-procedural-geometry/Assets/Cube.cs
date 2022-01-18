using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour {
    public uint SIZE = 2;
    public uint LOD = 2;

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

        // Level-Of-Details
        float step = (float)SIZE / LOD; // discrete quad side size (optimization)
        float halfSize = SIZE / 2f;     // hald of the SIZE (optimization)
        int quadLOD = (int)(LOD * LOD); // LOD^2 (optimization)

        List<Vector3> vertices = new List<Vector3>();
        int[] triangles = new int[quadLOD * 6 * 6]; // 6 sides of 2*LOD^2 triangles (3 indexes each) each

        for (int j = 0; j < 6; ++j) { // the whole cube
            for (int i = 0; i < quadLOD; ++i) { // single side
                float lx = i % LOD * step - halfSize;
                float rx = (i % LOD + 1) * step - halfSize;
                float uy = (i / LOD + 1) * step - halfSize;
                float dy = i / LOD * step - halfSize;

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
                    lu.x -= halfSize;
                    ru.x -= halfSize;
                    ld.x -= halfSize;
                    rd.x -= halfSize;
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
                    break;
                case 4: // z-
                    lu.z -= halfSize;
                    ru.z -= halfSize;
                    ld.z -= halfSize;
                    rd.z -= halfSize;
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
                    break;
                }

                vertices.Add(lu);
                vertices.Add(ru);
                vertices.Add(ld);
                vertices.Add(rd);
            }
        }

        mesh.SetVertices(vertices);
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
