using UnityEngine;

public class Waypoint
{
    public Vector2Int Position { get; private set; }
    public WaypointOptions Options { get; private set; }
    // after move action?

    public static Waypoint Of(Vector2Int pos)
    {
        return new Waypoint(pos);
    }

    public static Waypoint Of(int x, int y)
    {
        return new Waypoint(x, y);
    }

    private Waypoint(int x, int y)
    {
        Position = new Vector2Int(x, y);
    }

    private Waypoint(Vector2Int position)
    {
        Position = position;
    }

    private Waypoint(int x, int y, WaypointOptions options)
    {
        Position = new Vector2Int(x, y);
        Options = options;
    }

    private Waypoint(Vector2Int position, WaypointOptions options)
    {
        Position = position;
        Options = options;
    }

    public class WaypointOptions
    {
        private float _size;
        public WaypointOptions(float size)
        {
            _size = size;
        }
    }
}