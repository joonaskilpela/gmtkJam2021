using System.Linq;
using UnityEngine;

public class Player : GridObject
{
    public Renderer quadRenderer;
    public MoveDirection nextMove;
    public MoveDirection lastMove;

    protected MaterialPropertyBlock block;

    protected override void Start()
    {
        base.Start();

        block = new MaterialPropertyBlock();
        quadRenderer.GetPropertyBlock(block);
    }

    /// <summary>
    /// Set the array animation row property
    /// </summary>
    /// <param name="row"></param>
    protected virtual void SetAnimationRow(int row)
    {
        block.SetFloat("_Row", row);
        quadRenderer.SetPropertyBlock(block);
    }

    /// <summary>
    /// Player has reached the flag
    /// </summary>
    public virtual void ReachedFlag()
    {
        Debug.Log($"{name} reached the flag");
    }

    /// <summary>
    /// Set the move executed at the end of the turn
    /// </summary>
    /// <param name="dir"></param>
    public virtual void SetNextMove(MoveDirection dir)
    {
        // Find all grids
        var grids = FindObjectsOfType<GameGrid>();

        // Make sure all grids are ready for turn end
        if (grids.Any(g => !g.CanEndTurn)) return;

        // Do nothing if player cannot move in the direction
        if (!IsDirectionAllowed(dir.ToVector3(), out _)) return;

        // Set next move
        nextMove = dir;

        // Make sure we can end turn now, and advance turn in each grid
        foreach (var grid in grids)
        {
            grid.WillEndTurn = true;
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

    public override void Destroy(DestroyedBy reason)
    {
        base.Destroy(reason);

        // Delay sound
        if(reason == DestroyedBy.Fall) AudioPlayer.PlaySoundClip(AudioPlayer.SoundClip.PlayerDie, 0.9f);
        else AudioPlayer.PlaySoundClip(AudioPlayer.SoundClip.PlayerDie);
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
            if (obj is AngryPot) Destroy(DestroyedBy.Enemy);
            // Crates falling on the player is also lethal
            else if (obj is Crate) Destroy(DestroyedBy.Crushed);
            // Girders falling on the player, also very deadly
            else if (obj is Girder) Destroy(DestroyedBy.Crushed);
        }
    }
}
