using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Character : MonoBehaviour
{
    Rigidbody rb;
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Move Rigidbody
        if (Input.GetKeyDown(KeyCode.Space) && rb.velocity.y == 0)
            rb.AddForce(Vector3.up * 200f);
        if (Input.GetKey(KeyCode.W))
            rb.position += Quaternion.Euler(0f, rb.rotation.eulerAngles.y, 0f) * Vector3.forward / 10f;
        if (Input.GetKey(KeyCode.S))
            rb.position += Quaternion.Euler(0f, rb.rotation.eulerAngles.y + 180f, 0f) * Vector3.forward / 10f;
        if (Input.GetKey(KeyCode.A))
            rb.position += Quaternion.Euler(0f, rb.rotation.eulerAngles.y - 90f, 0f) * Vector3.forward / 10f;
        if (Input.GetKey(KeyCode.D))
            rb.position += Quaternion.Euler(0f, rb.rotation.eulerAngles.y + 90f, 0f) * Vector3.forward / 10f;

        // Rotate Rigidbody around Y-axis
        // X and Z are fixed to zero
        rb.rotation = Quaternion.Euler(
            0f,
            rb.rotation.eulerAngles.y + Input.GetAxis("Mouse X") * 100f * Time.deltaTime,
            0f
        );
        // Rotate Camera around X-axis
        // assign Rigidbody Y-axis rotation to Camera
        cam.transform.rotation = Quaternion.Euler(
            cam.transform.rotation.eulerAngles.x - Input.GetAxis("Mouse Y") * 100f * Time.deltaTime,
            rb.rotation.eulerAngles.y,
            0f
        );
        if (cam.transform.rotation.eulerAngles.x >= 85f && cam.transform.rotation.eulerAngles.x <= 180f)
            cam.transform.rotation = Quaternion.Euler(
                84.5f,
                rb.rotation.eulerAngles.y,
                0f
            );
        if (cam.transform.rotation.eulerAngles.x <= 275f && cam.transform.rotation.eulerAngles.x >= 180f)
            cam.transform.rotation = Quaternion.Euler(
                275.5f,
                rb.rotation.eulerAngles.y,
                0f
            );

        // Load Level2
        if (Input.GetKey(KeyCode.R))
            SceneManager.LoadScene("Level2");
    }
}
