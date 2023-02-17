using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelStateManager
{
    public delegate void StateChangeAction(GameState state);
    public event StateChangeAction OnStateChange;

    public enum GameState
    {
        START,
        PLAYING,
        SUCCESS,
        FAILED
    };

    private readonly GridController gridController;
    private readonly PlayerController playerController;

    private int turnLimit;
    public bool TurnLimitEnabled { get; private set; } = false;

    private List<Waypoint> waypoints;
    private int activeWaypoint;

    private GameState currentState;
    private readonly List<Action> actions;

    /*
        Constructor that takes no default parameters
    */
    public LevelStateManager(PlayerController player, GridController grid)
    {
        playerController = player;
        gridController = grid;

        activeWaypoint = -1;

        waypoints = new();

        PlayerController.OnMoveFullyCompleted += CheckTurnLimit;

        actions = new();

        TransitionState(GameState.START);
    }

    public LevelStateManager(PlayerController player, GridController grid, Waypoint[] waypoints) : this(player, grid)
    {
        this.waypoints = new(waypoints);
    }

    public LevelStateManager(PlayerController player, GridController grid, Vector2Int[] waypointPositions) : this(player, grid)
    {
        waypoints = PositionsToWaypoints(waypointPositions);
    }

    public void SetWaypoints(Waypoint[] waypoints)
    {
        this.waypoints = new(waypoints);
    }

    // DEPRECATED, should use the signature with Waypoint[] instead
    public void SetWaypoints(Vector2Int[] waypointPositions)
    {
        waypoints = PositionsToWaypoints(waypointPositions);
    }

    // Adds a tile to the current active path
    public void AddWaypoint(Waypoint waypoint)
    {
        if (waypoints.Count == 0)
        {
            activeWaypoint = 0;
        }

        waypoints.Add(waypoint);
    }

    public void SetTurnLimit(int turnLimit)
    {
        playerController.ResetMoveCount();
        this.turnLimit = turnLimit;
        TurnLimitEnabled = true;
    }

    private void CheckTurnLimit(Vector2Int positionAfterMove, bool moveShouldCount)
    {
        if (activeWaypoint == waypoints.Count)
        {
            TransitionState(GameState.SUCCESS);
        }

        if (!TurnLimitEnabled)
        {
            return;
        }
        if (moveShouldCount && playerController.GetMoveCount() >= turnLimit)
        {
            TransitionState(GameState.FAILED);
        }
    }
    /**
       * Spawns next waypoint in the waypoints arrray
    */
    public void SpawnNextWaypoint()
    {
        activeWaypoint += 1;

        if (activeWaypoint == waypoints.Count)
        {
            playerController.FinishMovingThenStopMovement();
        }
        else
        {
            gridController.SpawnWaypoint(waypoints[activeWaypoint], () => SpawnNextWaypoint());
        }

    }

    public bool IsTerminalGameState(GameState gameState)
    {
        return terminalGameStates.Contains(gameState);
    }

    private readonly HashSet<GameState> terminalGameStates = new() {
        GameState.SUCCESS,
        GameState.FAILED
    };

    void TransitionState(GameState nextState)
    {
        if (IsTerminalGameState(currentState))
        {
            // do not allow to transition to failed / success states from failed / success states
            return;
        }
        if (IsTerminalGameState(nextState))
        {
            gridController.DespawnAllWaypoints();
        }

        currentState = nextState;
        OnStateChange?.Invoke(currentState);
    }

    public void CheckPlayerState()
    {
        Vector2Int playerPos = playerController.GetCurrentPosition();
        if (!gridController.IsWithinGrid(playerPos))
        {
            TransitionState(GameState.FAILED);
        }
        if (playerController.GetHeight() < -1.0f)
        {
            TransitionState(GameState.FAILED);
        }
    }

    public void SetFailedState()
    {
        TransitionState(GameState.FAILED);
    }


    public void ManageGameState()
    {
        switch (currentState)
        {
            case GameState.START:
                SpawnNextWaypoint();
                TransitionState(GameState.PLAYING);
                break;
            case GameState.PLAYING:
                // player is currently playing the level
                // main checks and mechanics
                foreach (Action func in actions)
                {
                    func();
                }
                break;
            case GameState.SUCCESS:
                // player has met criteria for success on level
                break;
            case GameState.FAILED:
                // player has failed the level
                break;
            default:
                Debug.LogErrorFormat("Encountered unexpected game state: {0}", currentState);
                break;
        }
    }

    List<Waypoint> PositionsToWaypoints(Vector2Int[] waypointPositions)
    {
        List<Waypoint> waypoints = new();
        foreach (Vector2Int pos in waypointPositions)
        {
            waypoints.Add(Waypoint.Of(pos));
        }
        return waypoints;
    }
}
