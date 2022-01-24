using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcoSphere : MonoBehaviour {
    public uint RADIUS = 1;

    void Awake() {
        Mesh mesh = new Mesh();
        mesh.name = "Procedural IcoSphere";

        int verticesCount   = 12;     // count of vertices
        int trianglesCount  = 20 * 3; // indexes actually, 3 indexes for each triangle

        Vector3[] vertices = new Vector3[verticesCount];
        Vector3[] normals  = new Vector3[verticesCount];
        Vector2[] uvs      = new Vector2[verticesCount];
        int[] triangles    = new int[trianglesCount];

        // generating an icosahedron
        float circumferenceRadius = 1f / Mathf.Sqrt(5f);
        for (int p = 0; p < 2; ++p) { // 2 poles
            for (int v = 0; v < 5; ++v) { // single pole vertices
                float lerpByIndex         = v / 5f;
                float angle               = 2f * Mathf.PI * lerpByIndex;
                int currVertexIndex       = p * 6 + v;

                vertices[currVertexIndex] = new Vector3(
                    circumferenceRadius * Mathf.Cos(angle),
                    -0.5f * (1f - circumferenceRadius),
                    circumferenceRadius * Mathf.Sin(angle)
                );

                if (p == 0) { // upper
                    uvs[currVertexIndex] = new Vector2(
                        lerpByIndex,
                        2f / 3f
                    );
                } else { // lower
                    uvs[currVertexIndex] = new Vector2(
                        1f - lerpByIndex,
                        1f / 3f
                    );
                }
            }

            int poleIndex = p * 6 + 5;

            vertices[poleIndex] = new Vector3( // pole vertex
                0f,
                0f,
                0f
            );

            if (p == 0) { // upper
                uvs[poleIndex] = new Vector2(0f, 1f);
            } else { // lower
                uvs[poleIndex] = new Vector2(0f, 0f);
            }

            for (int t = 0; t < 5; ++t) { // pole triangles
                int currVertexIndex     = p * 6 + t;
                int nextVertexIndex     = p * 6 + (t + 1) % 5;
                int triangleIndexOffset = (p * 5 + t) * 3;

                triangles[triangleIndexOffset]     = currVertexIndex;
                triangles[triangleIndexOffset + 1] = poleIndex;
                triangles[triangleIndexOffset + 2] = nextVertexIndex;
            }

            if (p == 0) { // upper pole transformation
                for (int v = 0; v < 6; ++v) {
                    vertices[v].y += 0.5f;
                    vertices[v]    = Quaternion.AngleAxis(-144, Vector3.up) * vertices[v];
                }
            } else { // lower pole transformation
                for (int v = 0; v < 6; ++v) {
                    vertices[6 + v].y *= -1;
                    vertices[6 + v].y -= 0.5f;
                    vertices[6 + v].x *= -1;
                }
            }
        }

        for (int tp = 0; tp < 5; ++tp) { // side triangles (in pairs)
            int currUpperVertexIndex = tp;
            int nextUpperVertexIndex = (tp + 1) % 5;
            int currLowerVertexIndex = 6 + (5 - tp) % 5;
            int nextLowerVertexIndex = 6 + (10 - (tp + 1)) % 5;
            int triangleIndexOffset  = 30 + tp * 6;

            triangles[triangleIndexOffset]     = currUpperVertexIndex;
            triangles[triangleIndexOffset + 1] = nextUpperVertexIndex;
            triangles[triangleIndexOffset + 2] = currLowerVertexIndex;
            triangles[triangleIndexOffset + 3] = nextUpperVertexIndex;
            triangles[triangleIndexOffset + 4] = nextLowerVertexIndex;
            triangles[triangleIndexOffset + 5] = currLowerVertexIndex;
        }

        for (int v = 0; v < verticesCount; ++v) { // adjusting radius and normals
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
