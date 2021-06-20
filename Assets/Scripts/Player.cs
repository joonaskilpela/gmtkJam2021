using System.Linq;
using UnityEngine;

public class Player : GridObject
{
    /// <summary>
    /// State of the player object
    /// </summary>
    public class PlayerState : GridObjectState
    {
        public MoveDirection facingDirection;
        public MoveDirection lastMove;
        public MoveDirection nextMove;
        public int animationRow;
        public bool flagReached;

        public override void CopyTo(GridObjectState other)
        {
            base.CopyTo(other);

            if (other is PlayerState playerState)
            {
                playerState.facingDirection = facingDirection;
                playerState.lastMove = lastMove;
                playerState.nextMove = nextMove;
                playerState.animationRow = animationRow;
                playerState.flagReached = flagReached;
            }
        }
    }

    public override GridObjectState GetState()
    {
        // Base state
        var state = new PlayerState();
        base.GetState().CopyTo(state);

        // Player properties
        state.animationRow = GetAnimationRow();
        state.facingDirection = lastMove;
        state.lastMove = lastMove;
        state.nextMove = nextMove;
        state.flagReached = FlagReached;

        return state;
    }

    public override void RestoreState(GridObjectState state)
    {
        // Base state
        base.RestoreState(state);

        // Player properties
        PlayerState playerState = (PlayerState)state;

        lastMove = playerState.lastMove;
        FlagReached = playerState.flagReached;
        SetAnimationRow(playerState.animationRow);

        // Set nextmove to none to allow moving anywhere after restore
        nextMove = MoveDirection.None;
    }

    public Renderer quadRenderer;
    public MoveDirection nextMove;
    public MoveDirection lastMove;

    public virtual bool FlagReached { get; set; }

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
    /// Get the array animation row property
    /// </summary>
    /// <returns></returns>
    protected virtual int GetAnimationRow()
    {
        return (int)block.GetFloat("_Row");
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
    /// Will the player escape an enemy collision
    /// </summary>
    /// <param name="angryPot"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public bool WillEscapeEnemy(AngryPot angryPot, MoveDirection direction)
    {
        // Judge by lastMove because the move should already be processed
        // If not moving anywhere
        if (lastMove == MoveDirection.None) return false;

        return true;
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
        if (reason == DestroyedBy.Fall) AudioPlayer.PlaySoundClip(AudioPlayer.SoundClip.PlayerDie, 0.9f);
        else AudioPlayer.PlaySoundClip(AudioPlayer.SoundClip.PlayerDie);
    }

    /// <summary>
    /// Check if player is overlapping with unwanted objects
    /// </summary>
    public virtual void OverlapCheck()
    {
        if (!gameObject.activeSelf) return;

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
            // Goals are for winning
            else if (obj is Goal) ReachedFlag();
        }
    }
}
