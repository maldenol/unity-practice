using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour {
    public uint SIZE = 2;
    public uint LOD = 2;

    void Awake() {
        Mesh mesh = new Mesh();
        mesh.name = "Procedural Square";

        // 4 vertices
        // List<Vector3> vertices = new List<Vector3>(){
        //     new Vector3(-1, 1, 0),  // left-up
        //     new Vector3(1, 1, 0),   // right-up
        //     new Vector3(-1, -1, 0), // left-down
        //     new Vector3(1, -1, 0),  // right-down
        // };
        //
        // int[] triangles = new int[]{
        //     0, 1, 2, // left-up
        //     1, 3, 2, // right-down
        // };

        // Level-Of-Details
        float step = (float)SIZE / LOD; // discrete quad side size
        float halfSize = SIZE / 2f;
        List<Vector3> vertices = new List<Vector3>();
        int[] triangles = new int[LOD * LOD * 6];
        for (int i = 0; i < LOD * LOD; ++i) {
            float lux = i % LOD * step - halfSize;
            float rux = (i % LOD + 1) * step - halfSize;
            float ldx = lux;
            float rdx = rux;
            float luy = (i / LOD + 1) * step - halfSize;
            float ruy = luy;
            float ldy = i / LOD * step - halfSize;
            float rdy = ldy;

            vertices.Add(new Vector3(lux, luy, 0)); // left-up
            vertices.Add(new Vector3(rux, ruy, 0)); // right-up
            vertices.Add(new Vector3(ldx, ldy, 0)); // left-down
            vertices.Add(new Vector3(rdx, rdy, 0)); // right-down

            triangles[i * 6] = i * 4;         // left-up
            triangles[i * 6 + 1] = i * 4 + 1; // left-up
            triangles[i * 6 + 2] = i * 4 + 2; // left-up
            triangles[i * 6 + 3] = i * 4 + 1; // right-down
            triangles[i * 6 + 4] = i * 4 + 3; // right-down
            triangles[i * 6 + 5] = i * 4 + 2; // right-down
        }

        mesh.SetVertices(vertices);
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
