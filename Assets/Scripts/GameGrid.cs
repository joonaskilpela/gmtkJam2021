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

    public bool FlagReached;

    private bool GameIsOver;

    /// <summary>
    /// Get all grid objects inside this grid
    /// </summary>
    public GridObject[] GridObjects => gameObject.GetComponentsInChildren<GridObject>();

    /// <summary>
    /// Can the grid end turn next frame
    /// </summary>
    public bool CanEndTurn
    {
        get
        {
            if (WillEndTurn) return false;
            if (GameIsOver) return false;

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
            EndTurn();
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

        GameIsOver = true;
        GameOver?.Invoke();
    }

    /// <summary>
    /// Process enemies, gravity etc
    /// </summary>
    public virtual void EndTurn()
    {
        Debug.Log($"{name}: Turn ending");
        foreach (var obj in GridObjects)
        {
            obj.OnTurnEnd();
        }
    }
}
