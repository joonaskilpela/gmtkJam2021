using UnityEngine;

public class TopDownPlayer : Player
{
    protected override void Update()
    {
        base.Update();

        if (Input.GetKey(KeyCode.W)) SetNextMove(MoveDirection.Up);
        else if (Input.GetKey(KeyCode.D)) SetNextMove(MoveDirection.Right);
        else if (Input.GetKey(KeyCode.S)) SetNextMove(MoveDirection.Down);
        else if (Input.GetKey(KeyCode.A)) SetNextMove(MoveDirection.Left);
    }
}
