using DG.Tweening;
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

    public override void ReachedFlag()
    {
        base.ReachedFlag();

        SetAnimationRow(2);

        // Set flag reached
        FindObjectOfType<TopDownGrid>().FlagReached = true;
    }

    protected override void SetAnimationRow(int row)
    {
        // Force layer 2 (cheer) if flag has been reached
        if (FindObjectOfType<TopDownGrid>().FlagReached) row = 2;
        base.SetAnimationRow(row);
    }

    public override void SetNextMove(MoveDirection dir)
    {
        base.SetNextMove(dir);

        if (nextMove != MoveDirection.None)
        {
            // Reset flag reached when player moves
            FindObjectOfType<TopDownGrid>().FlagReached = false;
        }

        switch (nextMove)
        {
            case MoveDirection.None:
                break;
            case MoveDirection.Up:
                quadRenderer.transform.DORotateQuaternion(Quaternion.Euler(0, 0, 270), 0.1f);
                break;
            case MoveDirection.Right:
                quadRenderer.transform.DORotateQuaternion(Quaternion.Euler(0, 0, 180), 0.1f);
                break;
            case MoveDirection.Down:
                quadRenderer.transform.DORotateQuaternion(Quaternion.Euler(0, 0, 90), 0.1f);
                break;
            case MoveDirection.Left:
                quadRenderer.transform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), 0.1f);
                break;
        }
    }
}
