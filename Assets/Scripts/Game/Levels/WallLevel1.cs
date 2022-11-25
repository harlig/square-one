using System.Collections.Generic;
using UnityEngine;

public class WallLevel1 : LevelManager
{
    private List<GameState> gameStateOrder;
    private GameState currentGameState;

    private List<ObstacleController> obstacles;

    enum GameState
    {
        START,
        GREEN_SETUP,
        GREEN_HIT,
        RED_SETUP,
        RED_HIT,
        BLUE_SETUP,
        BLUE_HIT,
        SUCCESS,
        FAILED,
    };

    void Start()
    {
        gridSizeX = gridSizeY = 6;
        turnLimit = 13;

        gameStateOrder = new List<GameState>
        {
            GameState.START,
            GameState.GREEN_SETUP,
            GameState.GREEN_HIT,
            GameState.RED_SETUP,
            GameState.RED_HIT,
            GameState.BLUE_SETUP,
            GameState.BLUE_HIT,
            GameState.SUCCESS
        };

        SetupLevel(2, 3);

        waypoints = new() {
            new Vector2Int(gridSizeX - 1, gridSizeY - 2),
            // TODO this one shouldn't trigger until the wall finishes moving, so need to figure out how to manage the game state more complexly
            // like I want to be able to have the granularity to call TransitionState() at the times when I do below
            new Vector2Int(gridSizeX - 2, gridSizeY - 1),
            new Vector2Int(squareOne.x, squareOne.y),
        };

        SpawnNextWaypoint(waypoints);

        obstacles = new();
        for (int ndx = 0; ndx < gridSizeY; ndx++)
        {
            obstacles.Add(gridController.AddObstacleAtPosition(1, ndx));
        }

        obstacles[^1].SetAfterRollAction((_, _) => AfterObjectMoves());

        currentGameState = GameState.START;
    }

    bool objectHasMoved;

    void AfterObjectMoves()
    {
        Debug.Log("Object has moved");
        objectHasMoved = true;
    }

    void Update()
    {
        SetMoveCountText();
        if (levelActive)
        {
            ManageGameState();
        }
    }

    override protected void OnPlayerMoveFinish(Vector2Int playerPosition)
    {
        if (levelActive)
        {
            turnsLeft = turnLimit - playerController.GetMoveCount();
        }
    }

    bool greenHit = false;
    bool redSetupStarted = false;

    void ManageGameState()
    {
        Vector2Int playerPos = playerController.GetCurrentPosition();

        // allow devMode to not fall out of map
        if (!gridController.IsWithinGrid(playerPos))
        {
            Debug.Log("Player has exited map.");
            currentGameState = GameState.FAILED;
        }
        switch (currentGameState)
        {
            case GameState.START:
                TransitionState();
                break;
            case GameState.GREEN_SETUP:
                gridController.PaintTileAtLocation(waypoints[0].x, waypoints[0].y, Color.green);
                TransitionState();
                break;
            case GameState.GREEN_HIT:
                if (gridController.TileColorAtLocation(playerPos) == Color.green)
                {
                    greenHit = true;
                    MoveObstacles(Vector3.right);
                }
                if (greenHit && objectHasMoved)
                {
                    TransitionState();
                    objectHasMoved = false;
                }
                break;
            case GameState.RED_SETUP:
                if (!redSetupStarted)
                {
                    MoveObstacles(Vector3.right);
                    redSetupStarted = true;
                }
                if (redSetupStarted && objectHasMoved)
                {
                    objectHasMoved = false;
                    gridController.PaintTileAtLocation(waypoints[1].x, waypoints[1].y, Color.red);
                    gridController.PaintTileAtLocation(waypoints[2].x, waypoints[2].y, Color.blue);
                    TransitionState();
                }
                break;
            case GameState.RED_HIT:
                if (gridController.TileColorAtLocation(playerPos) == Color.red)
                {
                    gridController.SpawnIceTile(gridSizeX - 2, gridSizeY - 2, OnIceTileSteppedOn);
                    gridController.SpawnIceTile(gridSizeX - 2, gridSizeY - 3, OnIceTileSteppedOn);
                    gridController.SpawnIceTile(gridSizeX - 2, gridSizeY - 4, OnIceTileSteppedOn);
                    TransitionState();
                }
                break;
            case GameState.BLUE_SETUP:
                // last step is back to square one
                TransitionState();
                break;
            case GameState.BLUE_HIT:
                if (gridController.TileColorAtLocation(playerPos) == Color.blue)
                {
                    TransitionState();
                    // you must manage game state here before falling through, otherwise you could be transitioning
                    // into a success game state when you're at zero turns!
                    ManageGameState();
                }
                break;
            case GameState.SUCCESS:
                Debug.Log("Player has won!");
                SetTerminalGameState(successElements);
                break;
            case GameState.FAILED:
                Debug.Log("Player has failed.");
                SetTerminalGameState(failedElements);
                break;
            default:
                Debug.LogErrorFormat("Encountered unexpected game state: {0}", currentGameState);
                break;
        }

        if (turnsLeft <= 0)
        {
            Debug.Log("Player exceeded move count");
            currentGameState = GameState.FAILED;
        }

        void TransitionState()
        {
            // could probably use a better data structure as the state machine that allows a failure state as defined by the state machine
            currentGameState = gameStateOrder[gameStateOrder.IndexOf(currentGameState) + 1];
        }
    }

    void MoveObstacles(Vector3 direction)
    {
        if (objectHasMoved) return;
        foreach (ObstacleController obstacle in obstacles)
        {
            if (Mathf.RoundToInt(obstacle.transform.position.z) == 0) continue;

            obstacle.MoveInDirectionIfNotMovingAndDontEnqueue(direction);
        }
    }
}