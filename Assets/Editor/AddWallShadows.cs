
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;

public static class AddWallShadows
{
    [MenuItem("Tools/Blacked Out/Add Shadows To Walls")]
    public static void AddShadows()
    {
        int wallLayer = LayerMask.NameToLayer("Walls");

        if (wallLayer == -1)
        {
            Debug.LogError("Walls layer was not found!");
            return;
        }

        int added = 0;

        // Find all BoxCollider2D components in the open scene.
        BoxCollider2D[] colliders =
            Object.FindObjectsByType<BoxCollider2D>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (BoxCollider2D collider in colliders)
        {
            GameObject wall = collider.gameObject;

            // Only modify objects assigned to the Walls layer.
            if (wall.layer != wallLayer)
                continue;

            // Avoid adding duplicate shadow casters.
            if (wall.GetComponent<ShadowCaster2D>() != null)
                continue;

            // Add the component with Undo support.
            Undo.AddComponent<ShadowCaster2D>(wall);
            added++;
        }

        Debug.Log(
            "Blacked Out: Added Shadow Caster 2D to " +
            added + " walls."
        );
    }
}
