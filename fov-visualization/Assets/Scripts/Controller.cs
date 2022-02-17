using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
    public float moveSpeed = 1f;

    Rigidbody personRigidbody;
    Camera    viewCamera;

    Vector3 velocity;

    void Start() {
        personRigidbody = GetComponent<Rigidbody>();
        viewCamera      = Camera.main;
    }

    void FixedUpdate() {
        // Moving player's rigidbody in physics update event
        personRigidbody.MovePosition(personRigidbody.position + velocity * Time.deltaTime);
    }

    void Update() {
        // Getting world mouse position
        Vector3 mouseWorldPos = viewCamera.ScreenToWorldPoint(
            new Vector3(
                Input.mousePosition.x,
                Input.mousePosition.y,
                viewCamera.transform.position.y - personRigidbody.position.y
            )
        );

        // Making player face at mouse direction
        transform.LookAt(mouseWorldPos);

        // Getting velocity from keyboard input
        velocity = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0f,
            Input.GetAxisRaw("Vertical")
        ).normalized * moveSpeed;
    }
}
