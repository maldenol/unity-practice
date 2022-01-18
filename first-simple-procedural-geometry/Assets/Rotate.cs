using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {
    Transform tr;

    void Start() {
        tr = GetComponent<Transform>();
    }

    void Update() {
        tr.Rotate(0f, 1f, 0f);
    }
}
