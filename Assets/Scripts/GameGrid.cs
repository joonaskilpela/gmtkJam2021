using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public abstract class GameGrid : MonoBehaviour
{
    /// <summary>
    /// Bool used to delay turn ending by 1 frame
    /// </summary>
    public bool WillEndTurn = false;

    public UnityEvent GameOver;
    public UnityEvent GameOverRewind;

    public bool FlagReached = false;

    private bool GameIsOver;
    private bool TurnEnding;

    /// <summary>
    /// List of stored grid object states, for rewind mechanic
    /// </summary>
    public Stack<List<GridObjectState>> previousStateStack = new Stack<List<GridObjectState>>();

    /// <summary>
    /// Get all grid objects inside this grid
    /// </summary>
    public GridObject[] GridObjects => gameObject.GetComponentsInChildren<GridObject>();
    
    public GridObject[] GridObjectsIncludingDisabled => gameObject.GetComponentsInChildren<GridObject>(true);

    /// <summary>
    /// Can the grid end turn next frame
    /// </summary>
    public bool CanEndTurn
    {
        get
        {
            if (WillEndTurn) return false;
            if (GameIsOver) return false;
            if (TurnEnding) return false;

            // If all grids have reached win condition
            var allGrids = FindObjectsOfType<GameGrid>();

            // If there are no grids that have not reached the flag, disable turns
            if (!allGrids.Any(g => !g.FlagReached)) return false;

            var allObjects = FindObjectsOfType<GridObject>();

            // If any object is moving, dont allow ending turn
            return !allObjects.Any(o => o.IsMoving);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (WillEndTurn)
        {
            StartCoroutine(EndTurn());
            WillEndTurn = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // If any object is moving, dont allow ending turn
            if (!CanEndTurn) return;

            WillEndTurn = true;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            StartCoroutine(RewindState());
        }
    }

    private void FixedUpdate()
    {
        // Check for gameover state (player is gone)
        if (GameIsOver) return;

        if (!GridObjects.Any(o => o is Player))
        {
            DoGameOver();
        }
    }

    /// <summary>
    /// Set the game as over
    /// </summary>
    public void DoGameOver()
    {
        if (GameIsOver) return;

        Debug.Log($"{name} Game is Over");

        GameIsOver = true;
        GameOver?.Invoke();
    }

    public IEnumerator RewindState()
    {
        // If stack is empty, dont try to restore state
        if (previousStateStack.Count == 0) yield break;

        // Get the latest state
        var states = previousStateStack.Pop();

        // If there were no previous states
        if (states == null) yield break;

        // Restore all states
        foreach (var state in states)
        {
            state.Restore();
        }

        yield return new WaitForFixedUpdate();

        foreach (var state in states)
        {
            state.RestoreActive();
        }

        // Remove gameover after rewind
        if (GameIsOver)
        {
            GameIsOver = false;
            GameOverRewind?.Invoke();
        }

    }

    /// <summary>
    /// Process enemies, gravity etc
    /// </summary>
    public virtual IEnumerator EndTurn()
    {
        Debug.Log($"{name}: Turn ending");

        TurnEnding = true;

        // Store state from all objects, including disabled
        previousStateStack.Push(GridObjectsIncludingDisabled.Select(o => o.GetState()).ToList());

        yield return new WaitForEndOfFrame();

        var objects = GridObjects;

        // Do players first
        foreach (var obj in objects.Where(o => o is Player))
        {
            obj.OnTurnEnd();
        }

        // Do non players
        foreach (var obj in objects.Where(o => !(o is Player)))
        {
            obj.OnTurnEnd();
        }

        TurnEnding = false;
    }
}
