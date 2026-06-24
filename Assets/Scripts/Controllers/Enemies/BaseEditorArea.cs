using UnityEngine;

public class BaseEditorArea : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        // Set the color of the gizmo
        Gizmos.color = Color.yellow;

        // Calculate world space position based on the object's position
        Vector3 worldPos = transform.position;

        // Draw a wireframe box
        Gizmos.DrawWireCube(worldPos, transform.localScale);

        // Optional: Draw a solid box
        // Gizmos.color = new Color(1f, 1f, 0f, 0.2f); // Semi-transparent
        // Gizmos.DrawCube(worldPos, boxSize);
    }
}
