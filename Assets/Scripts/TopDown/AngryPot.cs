using System.Linq;
using UnityEngine;

public class AngryPot : GridObject
{
    /// <summary>
    /// State of the player object
    /// </summary>
    public class AngryPotState : GridObjectState
    {
        public MoveDirection direction;
    }

    public override GridObjectState GetState()
    {
        // Base state
        var state = new AngryPotState();
        base.GetState().CopyTo(state);

        // Pot properties
        state.direction = direction;

        return state;
    }

    public override void RestoreState(GridObjectState state)
    {
        // Base state
        base.RestoreState(state);

        // Pot properties
        var potState = (AngryPotState)state;

        direction = potState.direction;

        UpdateFacing();
    }

    public Renderer potRenderer;
    public MoveDirection direction = MoveDirection.Left;
    private MaterialPropertyBlock block;

    protected override void Start()
    {
        base.Start();

        block = new MaterialPropertyBlock();
        potRenderer.GetPropertyBlock(block);

        UpdateFacing();
    }

    public override void OnTurnEnd()
    {
        CheckAndMove();

        UpdateFacing();

        OOBCheck();
    }

    private void UpdateFacing()
    {
        if (block != null)
        {
            block.SetVector("_BaseMap_ST", new Vector4(-direction.ToVector3().x, 1, 1, 1));
            potRenderer.SetPropertyBlock(block);
        }
    }

    private void CheckAndMove()
    {
        Vector3 dir = direction.ToVector3();

        if (CanMove(dir, out var blockers))
        {
            DoMove(dir);
            return;
        }
        else
        {
            // If there was a player blocking
            if (blockers.Any(o => o is Player))
            {
                // Pick out the player
                Player player = (Player)blockers.First(o => o is Player);

                // Destroy player if theyre not escaping
                if (!player.WillEscapeEnemy(this, direction)) player.Destroy(DestroyedBy.Enemy);

                DoMove(dir);
                return;
            }

            direction = direction.Opposite();
            CheckAndMove();

            return;
        }
    }

    public override bool IsMovingOutOfWay(GridObject other, MoveDirection moveDirection)
    {
        // If pot is not moving
        if (direction == MoveDirection.None) return false;

        // If its a head-on collision
        if (direction.Opposite() == moveDirection) return false;

        // If pot is changing direction
        if(!IsDirectionAllowed(direction.ToVector3()))
        {
            // If move was in the same direction (towards the obstacle that we are going to change direction on)
            if (direction == moveDirection) return false;
        }

        // If other has gravity, and is above (riding)
        if (other.HasGravity && other.transform.position.y > transform.position.y) return false;

        return true;
    }
}
