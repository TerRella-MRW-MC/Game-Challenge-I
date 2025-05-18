using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerMovement))]
public class PlayerMovementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerMovement playerMovement = (PlayerMovement)target;

        if (GUILayout.Button("Reset")) {
            playerMovement.ResetToStart();
        }
    }
}
