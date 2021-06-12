using UnityEngine;

public class Player : GridObject
{
    public Renderer quadRenderer;
    public MoveDirection nextMove;
    public MoveDirection lastMove;

    private MaterialPropertyBlock block;

    protected override void Start()
    {
        base.Start();

        block = new MaterialPropertyBlock();
        quadRenderer.GetPropertyBlock(block);
        Debug.Log(block);
    }

    protected void SetAnimationRow(int row)
    {
        block.SetFloat("_Row", row);
        quadRenderer.SetPropertyBlock(block);
    }

    /// <summary>
    /// Set the move executed at the end of the turn
    /// </summary>
    /// <param name="dir"></param>
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

    public override void DoMove(Vector3 direction, float duration = 0.2F, bool callback = true)
    {
        SetAnimationRow(1);

        base.DoMove(direction, duration, callback);
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

        SetAnimationRow(0);
    }

    /// <summary>
    /// Check if player is overlapping with unwanted objects
    /// </summary>
    public virtual void OverlapCheck()
    {
        // Check if player ended up overlapping with something
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, Vector3.one * 0.4f, Vector3.forward, transform.rotation, 1f);

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
