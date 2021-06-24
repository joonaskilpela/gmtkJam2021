using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class AngryPot : GridObject
{
    public Texture upTexture;
    public Texture downTexture;
    public bool destroyed = false;
    public UnityEvent RewindDestroyed;

    /// <summary>
    /// State of the player object
    /// </summary>
    public class AngryPotState : GridObjectState
    {
        public bool destroyed;
        public MoveDirection direction;
    }

    public override GridObjectState GetState()
    {
        // Base state
        var state = new AngryPotState();
        base.GetState().CopyTo(state);

        // Pot properties
        state.direction = direction;
        state.destroyed = destroyed;

        return state;
    }

    public override void RestoreState(GridObjectState state)
    {
        // Base state
        base.RestoreState(state);

        // Pot properties
        var potState = (AngryPotState)state;

        // If state destroyed differs from current state
        if (potState.destroyed != destroyed)
        {
            // State is destroyed, remove pot
            if (potState.destroyed)
            {
                Destroy(DestroyedBy.Removal);
            }
            else // State is not destroyed, rewind destroyed state
            {
                RewindDestroyed?.Invoke();
            }
        }

        destroyed = potState.destroyed;

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
        if (destroyed)
            return;

        CheckAndMove();

        UpdateFacing();

        OOBCheck();
    }

    private void UpdateFacing()
    {
        if (block != null)
        {
            if (direction == MoveDirection.Up)
                block.SetTexture("_BaseMap", upTexture);

            else if (direction == MoveDirection.Down)
                block.SetTexture("_BaseMap", downTexture);

            else
                block.SetVector("_BaseMap_ST", new Vector4(-direction.ToVector3().x, 1, 1, 1));

            potRenderer.SetPropertyBlock(block);
        }
    }

    private void CheckAndMove()
    {
        if (destroyed)
            return;

        Vector3 dir = direction.ToVector3();
        List<GridObject> blockers = new List<GridObject>();

        if (CanMove(dir, out blockers))
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

            dir = direction.ToVector3();
            if (CanMove(dir, out blockers))
            {
                CheckAndMove();
            }
            else if (blockers.Any(o => o is Player))
            {
                CheckAndMove();
            }
            else
                DoGravity();

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
        if (!IsDirectionAllowed(direction.ToVector3()))
        {
            // If move was in the same direction (towards the obstacle that we are going to change direction on)
            if (direction == moveDirection) return false;
        }

        // If other has gravity, and is above (riding)
        if (other.HasGravity && other.transform.position.y > transform.position.y) return false;

        // If pot is stuck above player, player can't jump above it
        if(!IsDirectionAllowed(direction.ToVector3()) && !IsDirectionAllowed(direction.Opposite().ToVector3())){
            return false;
        }
        return true;
    }

    public override void Destroy(DestroyedBy reason)
    {
        destroyed = true;

        Debug.Log($"{name} was destroyed by {reason}");

        // Play falling sound
        if (reason == DestroyedBy.Fall) AudioPlayer.PlaySoundClip(AudioPlayer.SoundClip.FallCrash);

        //Destroy(gameObject);
        //gameObject.SetActive(false);        

        OnObjectDestroyed.Invoke();
    }

    public override void DoGravity()
    {
        if (destroyed)
            return;

        base.DoGravity();
    }
}
