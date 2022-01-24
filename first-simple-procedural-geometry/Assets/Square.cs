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
        float xyQuadSize = (float)SIZE / (int)LOD; // discrete quad's side xy size
        float uvQuadSize = 1f / (int)LOD;          // discrete quad's side uv size
        float halfSize   = SIZE / 2f;              // half of the SIZE
        int quadLOD      = (int)(LOD * LOD);       // LOD^2

        int quadsCount     = quadLOD;            // count of discrete quads
        int verticesCount  = 4 * quadsCount;     // count of vertices
        int trianglesCount = 2 * quadsCount * 3; // indexes actually, 3 indexes for each triangle

        if (verticesCount > 65535) mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        Vector3[] vertices = new Vector3[verticesCount]; // 4*LOD^2 vertices
        Vector3[] normals  = new Vector3[verticesCount]; // 4*LOD^2 normals
        Vector2[] uvs      = new Vector2[verticesCount]; // UV coordinates for each vertex
        int[] triangles    = new int[trianglesCount];    // 2*LOD^2 triangles (3 indexes each) each

        for (int v = 0; v < quadsCount; ++v) {
            int row    = v / (int)LOD;
            int column = v % (int)LOD;
            float lux = column * xyQuadSize - halfSize;
            float rux = (column + 1) * xyQuadSize - halfSize;
            float ldx = lux;
            float rdx = rux;
            float luy = (row + 1) * xyQuadSize - halfSize;
            float ruy = luy;
            float ldy = row * xyQuadSize - halfSize;
            float rdy = ldy;
            float luu = column * uvQuadSize;
            float ruu = (column + 1) * uvQuadSize;
            float ldu = luu;
            float rdu = ruu;
            float luv = (row + 1) * uvQuadSize;
            float ruv = luv;
            float ldv = row * uvQuadSize;
            float rdv = ldv;

            vertices[v * 4]     = new Vector3(lux, luy, 0); // left-up
            vertices[v * 4 + 1] = new Vector3(rux, ruy, 0); // right-up
            vertices[v * 4 + 2] = new Vector3(ldx, ldy, 0); // left-down
            vertices[v * 4 + 3] = new Vector3(rdx, rdy, 0); // right-down

            normals[v * 4]     = Vector3.back; // left-up
            normals[v * 4 + 1] = Vector3.back; // right-up
            normals[v * 4 + 2] = Vector3.back; // left-down
            normals[v * 4 + 3] = Vector3.back; // right-down

            uvs[v * 4]     = new Vector2(luu, luv); // left-up
            uvs[v * 4 + 1] = new Vector2(ruu, ruv); // right-up
            uvs[v * 4 + 2] = new Vector2(ldu, ldv); // left-down
            uvs[v * 4 + 3] = new Vector2(rdu, rdv); // right-down

            triangles[v * 6]     = v * 4;     // left-up triangle
            triangles[v * 6 + 1] = v * 4 + 1; // left-up triangle
            triangles[v * 6 + 2] = v * 4 + 2; // left-up triangle
            triangles[v * 6 + 3] = v * 4 + 1; // right-down triangle
            triangles[v * 6 + 4] = v * 4 + 3; // right-down triangle
            triangles[v * 6 + 5] = v * 4 + 2; // right-down triangle
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
