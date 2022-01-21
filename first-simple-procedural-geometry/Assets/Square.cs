using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour {
    public uint SIZE = 2;
    public uint LOD  = 2;

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

        // Level-Of-Details (count of quads along one side)
        float xyQuadSize = (float)SIZE / LOD; // discrete quad's side xy size (optimization)
        float uvQuadSize = 1f / LOD;          // discrete quad's side uv size (optimization)
        float halfSize   = SIZE / 2f;         // hald of the SIZE (optimization)
        int quadLOD      = (int)(LOD * LOD);  // LOD^2 (optimization)

        Vector3[] vertices = new Vector3[4 * quadLOD]; // 4*LOD^2 vertices
        Vector3[] normals  = new Vector3[4 * quadLOD]; // 4*LOD^2 normals
        Vector2[] uvs      = new Vector2[4 * quadLOD]; // UV coordinates for each vertex
        int[] triangles    = new int[2 * quadLOD * 3]; // 2*LOD^2 triangles (3 indexes each) each

        for (int i = 0; i < quadLOD; ++i) {
            int row    = i / (int)LOD;
            int column = i % (int)LOD;
            float lux  = column * xyQuadSize - halfSize;
            float rux  = (column + 1) * xyQuadSize - halfSize;
            float ldx  = lux;
            float rdx  = rux;
            float luy  = (row + 1) * xyQuadSize - halfSize;
            float ruy  = luy;
            float ldy  = row * xyQuadSize - halfSize;
            float rdy  = ldy;
            float luu  = column * uvQuadSize;
            float ruu  = (column + 1) * uvQuadSize;
            float ldu  = luu;
            float rdu  = ruu;
            float luv  = (row + 1) * uvQuadSize;
            float ruv  = luv;
            float ldv  = row * uvQuadSize;
            float rdv  = ldv;

            vertices[i * 4]     = new Vector3(lux, luy, 0); // left-up
            vertices[i * 4 + 1] = new Vector3(rux, ruy, 0); // right-up
            vertices[i * 4 + 2] = new Vector3(ldx, ldy, 0); // left-down
            vertices[i * 4 + 3] = new Vector3(rdx, rdy, 0); // right-down

            normals[i * 4]     = Vector3.back; // left-up
            normals[i * 4 + 1] = Vector3.back; // right-up
            normals[i * 4 + 2] = Vector3.back; // left-down
            normals[i * 4 + 3] = Vector3.back; // right-down

            uvs[i * 4]     = new Vector2(luu, luv); // left-up
            uvs[i * 4 + 1] = new Vector2(ruu, ruv); // right-up
            uvs[i * 4 + 2] = new Vector2(ldu, ldv); // left-down
            uvs[i * 4 + 3] = new Vector2(rdu, rdv); // right-down

            triangles[i * 6]     = i * 4;     // left-up quad
            triangles[i * 6 + 1] = i * 4 + 1; // left-up quad
            triangles[i * 6 + 2] = i * 4 + 2; // left-up quad
            triangles[i * 6 + 3] = i * 4 + 1; // right-down quad
            triangles[i * 6 + 4] = i * 4 + 3; // right-down quad
            triangles[i * 6 + 5] = i * 4 + 2; // right-down quad
        }

        mesh.vertices  = vertices;
        mesh.normals   = normals;
        mesh.uv        = uvs;
        mesh.triangles = triangles;

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
