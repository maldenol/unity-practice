using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circling : MonoBehaviour
{
    Vector3 originPosition;
    const float RADIUS = 6f;
    const float HEIGHT = 1.5f;
    const float VELOCITY = 0.1f;

    // Start is called before the first frame update
    void Start() {
        originPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            originPosition.x + RADIUS * Mathf.Cos(Time.time * VELOCITY),
            originPosition.y + HEIGHT,
            originPosition.z + RADIUS * Mathf.Sin(Time.time * VELOCITY)
        );
        transform.LookAt(originPosition);
    }
}
