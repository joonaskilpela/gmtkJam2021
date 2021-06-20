using UnityEngine;
using UnityEngine.Events;

public class SpikeTrap : GridObject
{
    /// <summary>
    /// State of the player object
    /// </summary>
    public class SpikeTrapState : GridObjectState
    {
        public bool destroyed;
    }

    public override GridObjectState GetState()
    {
        // Base state
        var state = new SpikeTrapState();
        base.GetState().CopyTo(state);

        // SpikeTrap properties
        state.destroyed = destroyed;

        return state;
    }

    public override void RestoreState(GridObjectState state)
    {
        // Base state
        base.RestoreState(state);

        // SpikeTrap properties
        var spikeState = (SpikeTrapState)state;

        // If state destroyed differs from current state
        if(spikeState.destroyed != destroyed)
        {
            // State is destroyed, remove spikes
            if(spikeState.destroyed)
            {
                Destroy(DestroyedBy.Removal);
            }
            else // State is not destroyed, rewind destroyed state
            {
                RewindDestroyed?.Invoke();
            }
        }

        destroyed = spikeState.destroyed;
    }

    public UnityEvent RewindDestroyed;

    private bool destroyed;

    // Can always push spiketraps (for interaction)
    public override bool CanPush(Vector3 dir)
    {
        return true;
    }

    public override void Push(Vector3 dir, GridObject pusher)
    {
        // Pots are immune to spikes
        if (pusher is AngryPot) return;

        // Destroy the pushers
        pusher.Destroy(DestroyedBy.Spike);
    }

    public override void Destroy(DestroyedBy reason)
    {
        OnObjectDestroyed.Invoke();
        destroyed = true;
    }
}
