using UnityEngine;

public class Goal : GridObject
{
    public override bool CanPush(Vector3 dir)
    {
        return true;
    }

    public override void Push(Vector3 dir, GridObject pusher)
    {
        // Win condition
        if(pusher is Player player)
        {
            player.ReachedFlag();
        }
    }
}
