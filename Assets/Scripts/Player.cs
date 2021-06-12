using UnityEngine;

public class Player : GridObject
{
    public MoveDirection nextMove;
    public MoveDirection lastMove;

    public void SetNextMove(MoveDirection dir)
    {
        var grids = FindObjectsOfType<GameGrid>();
        foreach (var grid in grids)
        {
            if (grid.CanEndTurn)
            {
                nextMove = dir;
                grid.WillEndTurn = true;
            }
        }
    }

    /// <summary>
    /// Move this object, returns true if successful
    /// </summary>
    /// <param name="length"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public bool ExecuteMove(float length = 1f, float duration = 0.2f)
    {
        lastMove = nextMove;

        if (nextMove != MoveDirection.None)
        {
            var dir = nextMove.ToVector3() * length;

            if (CanMove(dir))
            {
                DoMove(dir, duration);
                nextMove = MoveDirection.None;
                return true;
            }
        }

        nextMove = MoveDirection.None;
        return false;
    }

    public override void OnTurnEnd()
    {
        ExecuteMove();

        OOBCheck();
    }

    protected override void MoveFinished()
    {
        base.MoveFinished();

        OverlapCheck();
    }

    /// <summary>
    /// Check if player is overlapping with unwanted objects
    /// </summary>
    public virtual void OverlapCheck()
    {
        // Check if player ended up overlapping with something
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, Vector3.one * 0.48f, Vector3.forward, transform.rotation, 1f);

        foreach (var hit in hits)
        {
            var obj = hit.collider.GetComponent<GridObject>();

            // Angry pots destroy the player
            if (obj is AngryPot) Destroy();
            // Crates falling on the player is also lethal
            else if (obj is Crate) Destroy();
            // Girders falling on the player, also very deadly
            else if (obj is Girder) Destroy();
        }
    }
}
