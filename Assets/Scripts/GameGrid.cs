using System.Linq;
using UnityEngine;

public abstract class GameGrid : MonoBehaviour
{
    /// <summary>
    /// Bool used to delay turn ending by 1 frame
    /// </summary>
    public bool WillEndTurn = false;

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
