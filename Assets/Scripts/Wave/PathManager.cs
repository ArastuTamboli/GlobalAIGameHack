using UnityEngine;

public class PathManager : MonoBehaviour
{
    public static PathManager instance;
    [Header("Path Settings")]
    public Transform[] path1Waypoints;
    public Transform[] path2Waypoints;

    [Header("Visualization")]
    public Color path1Color = Color.green;
    public Color path2Color = Color.blue;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public Transform[] GetPath(int pathIndex)
    {
        switch (pathIndex)
        {
            case 0:
                return path1Waypoints;
            case 1:
                return path2Waypoints;
            default:
                Debug.LogWarning($"Invalid path index: {pathIndex}");
                return path1Waypoints;
        }
    }

    public int GetRandomPathIndex()
    {
        return Random.Range(0, 2);
    }

    void OnDrawGizmos()
    {
        DrawPath(path1Waypoints, path1Color);
        DrawPath(path2Waypoints, path2Color);
    }

    void DrawPath(Transform[] waypoints, Color color)
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = color;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                Gizmos.DrawSphere(waypoints[i].position, 0.3f);
            }
        }

        if (waypoints[waypoints.Length - 1] != null)
        {
            Gizmos.DrawSphere(waypoints[waypoints.Length - 1].position, 0.5f);
        }
    }
}
