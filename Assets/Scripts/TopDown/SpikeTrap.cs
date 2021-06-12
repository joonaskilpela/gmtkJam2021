using UnityEngine;

public class SpikeTrap : GridObject
{
    // Can always push spiketraps (for interaction)
    public override bool CanPush(Vector3 dir)
    {
        return true;
    }

    public override void Push(Vector3 dir, GridObject pusher)
    {
        // Destroy the pushers
        pusher.Destroy();
    }

    public override void Destroy()
    {
        OnObjectDestroyed.Invoke();
    }
}
