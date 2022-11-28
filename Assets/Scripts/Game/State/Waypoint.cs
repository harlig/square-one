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

    public static Waypoint Of(Vector2Int pos, WaypointOptions options)
    {
        return new Waypoint(pos, options);
    }

    public static Waypoint Of(int x, int y)
    {
        return Of(new Vector2Int(x, y));
    }

    public static Waypoint Of(int x, int y, WaypointOptions options)
    {
        return Of(new Vector2Int(x, y), options);
    }

    private Waypoint(Vector2Int position)
    {
        Position = position;
        // default size for waypoints
        Options = WaypointOptions.Of(0.3f);
    }

    private Waypoint(Vector2Int position, WaypointOptions options)
    {
        Position = position;
        Options = options;
    }

    public class WaypointOptions
    {
        public float? Size { get; private set; }
        public Color? WaypointColor { get; private set; }
        public WaypointController.OnTriggeredAction OnTriggeredAction { get; set; }

        public static WaypointOptions Of(float size)
        {
            return new WaypointOptions(size);
        }

        public static WaypointOptions Of(float size, Color color)
        {
            return new WaypointOptions(size, color);
        }

        public static WaypointOptions Of(WaypointController.OnTriggeredAction onTriggeredAction)
        {
            return new WaypointOptions(onTriggeredAction);
        }

        public WaypointOptions WithOnTriggeredAction(WaypointController.OnTriggeredAction onTriggeredAction)
        {
            OnTriggeredAction = onTriggeredAction;
            return this;
        }

        private WaypointOptions(float size)
        {
            Size = size;
        }

        private WaypointOptions(float size, Color color)
        {
            Size = size;
            WaypointColor = color;
        }

        private WaypointOptions(WaypointController.OnTriggeredAction onTriggeredAction)
        {
            OnTriggeredAction = onTriggeredAction;
        }
    }
}