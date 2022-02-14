using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesScript : MonoBehaviour {
    // Cannot be changed while running
    public uint particlesCount = 1000;
    public float particleSize  = 0.01f;

    // Can be changed while running
    public float minimalDistance = 2f;
    public float maximalDistance = 5f;

    private ComputeShader computeShader;

    private int  kernelIndex;
    private uint kernelThreadGroupSizeX;

    private ComputeBuffer particlesPositionsBuffer;
    private ComputeBuffer particlesOffsetsBuffer;

    private Vector3[] particlesPositionsArray;

    private Transform[] particles;

    void Awake() {
        // Loading compute shader
        computeShader = Resources.Load<ComputeShader>("ParticlesCS");

        // Finding compute shader kernel and its numthreads (optionally)
        kernelIndex = computeShader.FindKernel("calcParticle");
        computeShader.GetKernelThreadGroupSizes(
            kernelIndex,
            out kernelThreadGroupSizeX,
            out _,
            out _
        );

        // Initializing particlesPositionsArray and particlesPositionsBuffer
        particlesPositionsArray = new Vector3[particlesCount];
        for (uint i = 0; i < particlesCount; ++i) {
            particlesPositionsArray[i] = Random.onUnitSphere;
        }
        particlesPositionsBuffer = new ComputeBuffer(
            (int)particlesCount,
            3 * sizeof(float)
        );
        particlesPositionsBuffer.SetData(particlesPositionsArray);

        // Initializing particlesOffsetsBuffer
        float[] particlesOffsetsArray = new float[particlesCount];
        for (uint i = 0; i < particlesCount; ++i) {
            particlesOffsetsArray[i] = Random.Range(-Mathf.PI, Mathf.PI);
        }
        particlesOffsetsBuffer = new ComputeBuffer(
            (int)particlesCount,
            sizeof(float)
        );
        particlesOffsetsBuffer.SetData(particlesOffsetsArray);

        // Creating particles
        // Initializing particles array
        particles = new Transform[particlesCount];
        // Getting and creating particle components
        Transform thisTransform = GetComponent<Transform>();
        Mesh pointMesh = new Mesh();
        pointMesh.vertices = new Vector3[]{Vector3.zero};
        pointMesh.SetIndices(new int[]{0}, MeshTopology.Points, 0);
        Material quadBillboardMaterial = Resources.Load<Material>("ParticlesM");
        quadBillboardMaterial.SetFloat("_Size", particleSize);
        // Filling array with particles
        for (uint i = 0; i < particlesCount; ++i) {
            // Initializing new particle
            GameObject newParticle = new GameObject(
                "Particle" + i,
                typeof(MeshFilter),
                typeof(MeshRenderer)
            );
            newParticle.GetComponent<MeshFilter>().sharedMesh = pointMesh;
            newParticle.GetComponent<MeshRenderer>().material = quadBillboardMaterial;
            // Making blank object a child of this object
            newParticle.transform.parent = thisTransform;
            // Adding particle to array
            particles[i] = newParticle.transform;
        }

        // Passing buffers to compute shader
        computeShader.SetBuffer(
            kernelIndex,
            "particlesPositions",
            particlesPositionsBuffer
        );
        computeShader.SetBuffer(
            kernelIndex,
            "particlesOffsets",
            particlesOffsetsBuffer
        );
    }

    void Update() {
        // Passing variables
        computeShader.SetFloat("minDistance", minimalDistance);
        computeShader.SetFloat("maxDistance", maximalDistance);
        computeShader.SetFloat("currTime", Time.time);

        // Running compute shader
        computeShader.Dispatch(
            kernelIndex,
            (int)Mathf.Ceil((float)particlesCount / (float)kernelThreadGroupSizeX),
            1,
            1
        );

        // Passing compute shader results to arrays
        particlesPositionsBuffer.GetData(particlesPositionsArray);

        // Setting particles their positions
        for (uint i = 0; i < particlesCount; ++i) {
            particles[i].localPosition = particlesPositionsArray[i];
        }
    }

    void OnDestroy() {
        // Releasing buffers
        particlesPositionsBuffer.Dispose();
        particlesOffsetsBuffer.Dispose();
    }
}
