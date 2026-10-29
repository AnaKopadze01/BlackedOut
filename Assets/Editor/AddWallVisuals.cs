
using UnityEngine;
using UnityEditor;

public static class AddWallVisuals
{
    [MenuItem("Tools/Blacked Out/Add Visuals To Selected Walls")]
    public static void AddVisuals()
    {
        // Find the WallVisual we already created.
        GameObject template = null;

        Transform[] transforms =
            Object.FindObjectsByType<Transform>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (Transform t in transforms)
        {
            if (t.name == "WallVisual" &&
                t.GetComponent<SpriteRenderer>() != null)
            {
                template = t.gameObject;
                break;
            }
        }

        if (template == null)
        {
            Debug.LogError(
                "Create one WallVisual first!"
            );
            return;
        }

        int wallLayer = LayerMask.NameToLayer("Walls");
        int added = 0;

        foreach (GameObject selected in Selection.gameObjects)
        {
            BoxCollider2D[] colliders =
                selected.GetComponentsInChildren<BoxCollider2D>(true);

            foreach (BoxCollider2D collider in colliders)
            {
                GameObject wall = collider.gameObject;

                if (wall.layer != wallLayer)
                    continue;

                if (wall.transform.Find("WallVisual") != null)
                    continue;

                GameObject visual =
                    Object.Instantiate(template, wall.transform);

                visual.name = "WallVisual";

                visual.transform.localPosition = Vector3.zero;
                visual.transform.localRotation = Quaternion.identity;
                visual.transform.localScale = Vector3.one;

                // Only the original wall handles collisions and shadows.
                foreach (Collider2D c in
                    visual.GetComponents<Collider2D>())
                    Object.DestroyImmediate(c);

                foreach (var caster in visual.GetComponents<
                    UnityEngine.Rendering.Universal.ShadowCaster2D>())
                    Object.DestroyImmediate(caster);

                Undo.RegisterCreatedObjectUndo(
                    visual, "Add Wall Visual"
                );

                added++;
            }
        }

        Debug.Log("Added visuals to " + added + " walls.");
    }
}
