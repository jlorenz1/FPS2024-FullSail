using UnityEngine;
using Unity.AI.Navigation;

public class NavMeshSurfaceBuilder : MonoBehaviour
{
    private NavMeshSurface navMeshSurface;

    void Start()
    {
        // Get the NavMeshSurface component on the current GameObject
        navMeshSurface = GetComponent<NavMeshSurface>();

        if (navMeshSurface != null)
        {
            // Build the NavMesh using the NavMeshSurface component
            navMeshSurface.BuildNavMesh();
        }
        else
        {
            Debug.LogError("NavMeshSurface component not found on this GameObject.");
        }
    }
}