using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor {
    public void OnSceneGUI() {
        FieldOfView fov = (FieldOfView)target;

        // Drawing view radius circle
        Handles.color = Color.white;
        Handles.DrawWireArc(
            fov.transform.position,
            Vector3.up,
            Vector3.forward,
            360f,
            fov.viewRadius
        );

        Vector3 leftViewBorder  = fov.DirFromAngle(-fov.viewAngle / 2f);
        Vector3 rightViewBorder = fov.DirFromAngle(fov.viewAngle / 2f);

        // Drawing view angle sector
        Handles.DrawLine(
            fov.transform.position,
            fov.transform.position + leftViewBorder * fov.viewRadius
        );
        Handles.DrawLine(
            fov.transform.position,
            fov.transform.position + rightViewBorder * fov.viewRadius
        );

        // Drawing lines to each visible target
        Handles.color = Color.red;
        foreach (Transform visibleTarget in fov.visibleTargets) {
            Handles.DrawLine(fov.transform.position, visibleTarget.position);
        }

        // Updating FOV visualization mesh
        fov.DrawFieldOfView();
    }
}
