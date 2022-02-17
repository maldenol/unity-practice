using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour {
    [Range(0f, 10f)]
    public float viewRadius = 1f;
    [Range(0f, 360f)]
    public float viewAngle = 90f;
    [Range(0f, 10f)]
    public float meshResolution = 1f;
    [Range(0, 100)]
    public int edgeResolveIterations = 1;
    [Range(0f, 10f)]
    public float edgeDistanceThreshhold = 1f;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    public MeshFilter viewMeshFilter;

    Mesh viewMesh;

    void Start() {
        // Initializing mesh for FOV visulation
        viewMesh            = new Mesh();
        viewMesh.name       = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

        // Starting FindTargetsWithDelay coroutine each 0.1 seconds
        StartCoroutine("FindTargetsWithDelay", 0.1f);
    }

    // Coroutine function for running FindVisibleTargets each <delay> seconds
    IEnumerator FindTargetsWithDelay(float delay) {
        while (true) {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    // Function for finding visible by player targets
    void FindVisibleTargets() {
        // Clear the list of visible targets
        visibleTargets.Clear();

        // Getting targets in view radius
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; ++i) {
            Transform target = targetsInViewRadius[i].transform;

            Vector3 dirToTarget = (target.position - transform.position).normalized;

            // Checking if target is inside view angle sector
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2f) {
                float distToTarget = Vector3.Distance(transform.position, target.position);

                // Checking if target is not overlapped by obstacle
                if (!Physics.Raycast(
                        transform.position,
                        dirToTarget,
                        distToTarget,
                        obstacleMask
                    )
                ) {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    // Function for updating FOV visualization mesh
    public void DrawFieldOfView() {
        int stepCount   = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngle = viewAngle / stepCount;

        List<Vector3> viewPoints = new List<Vector3>();

        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i <= stepCount; ++i) {
            float angle = -viewAngle / 2f + i * stepAngle;

            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0) {
                // Checking if old and new casts could hit different obstacles
                bool edgeDistanceThreshholdExceeded =
                    Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshhold;

                // If one of casts hits and another one doesn't
                // or if both of them hit but different obstacles
                if (
                    oldViewCast.hit != newViewCast.hit ||
                    oldViewCast.hit && newViewCast.hit && edgeDistanceThreshholdExceeded
                ) {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero) {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero) {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        // Constructing mesh
        int vertexCount    = viewPoints.Count + 1;
        int triangleCount  = (vertexCount - 2) * 3;

        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles    = new int[triangleCount];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; ++i) {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2) {
                triangles[i * 3]     = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices  = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    // Function for casting a ray by specified angle
    ViewCastInfo ViewCast(float angle) {
        Vector3 dir = DirFromAngle(angle);
        RaycastHit hit;

        // If ray hits
        if (Physics.Raycast(
                transform.position,
                dir,
                out hit,
                viewRadius,
                obstacleMask
            )
        ) {
            return new ViewCastInfo(
                true,
                hit.point,
                hit.distance,
                transform.eulerAngles.y + angle
            );
        }

        // Else
        return new ViewCastInfo(
            false,
            transform.position + dir * viewRadius,
            viewRadius,
            transform.eulerAngles.y + angle
        );
    }

    // Function for finding an edge between 2 rays
    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast) {
        float minAngle = minViewCast.angle - transform.eulerAngles.y;
        float maxAngle = maxViewCast.angle - transform.eulerAngles.y;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        // Finding edge with specific precision (edgeResolveIterations)
        for (int i = 0; i < edgeResolveIterations; ++i) {
            float angle = (minAngle + maxAngle) / 2f;
            ViewCastInfo newViewCast = ViewCast(angle);

            // Checking if min and new casts could hit different obstacles
            bool edgeDistanceThreshholdExceeded =
                Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDistanceThreshhold;

            // If both of casts hit same obstalce or don't hit at all
            if (newViewCast.hit == minViewCast.hit && !edgeDistanceThreshholdExceeded) {
                minAngle = angle;
                minPoint = newViewCast.point;
            } else {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    // Function for getting direction vector from local angle in degrees
    public Vector3 DirFromAngle(float angle) {
        // Converting given local angle to global
        angle += transform.eulerAngles.y;

        // Unity x = 90 - mathematical x

        // 1st method: calculate Unity x
        // float unityAngle = 90f - angle;
        // return new Vector3(
        //     Mathf.Cos(unityAngle * Mathf.Deg2Rad),
        //     0f,
        //     Mathf.Sin(unityAngle * Mathf.Deg2Rad)
        // );

        // 2nd method: swap sin and cos
        // because sin(90 - x) = cos(x) and cos(90 - x) = sin(x)
        return new Vector3(
            Mathf.Sin(angle * Mathf.Deg2Rad),
            0f,
            Mathf.Cos(angle * Mathf.Deg2Rad)
        );
    }

    // Structure for holding information about results of raycast in specific direction
    public struct ViewCastInfo {
        public bool    hit;
        public Vector3 point;
        public float   distance;
        public float   angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle) {
            hit      = _hit;
            point    = _point;
            distance = _distance;
            angle    = _angle;
        }
    };

    // Structure for holding information about found edge between 2 rays
    public struct EdgeInfo {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB) {
            pointA = _pointA;
            pointB = _pointB;
        }
    };
}
