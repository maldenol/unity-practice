using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levitating : MonoBehaviour
{
    float originHeight;
    float frequency;
    float amplitude;
    float rotationVelocityInDeg;

    // Start is called before the first frame update
    void Start()
    {
        originHeight = transform.position.y;
        frequency = Random.Range(1.5f, 2.5f);
        amplitude = Random.Range(0.05f, 0.15f);
        rotationVelocityInDeg = Random.Range(0.15f, 0.25f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, originHeight + Mathf.Cos(Time.time * frequency) * amplitude, transform.position.z);
        transform.Rotate(new Vector3(0f, rotationVelocityInDeg, 0f));
    }
}
