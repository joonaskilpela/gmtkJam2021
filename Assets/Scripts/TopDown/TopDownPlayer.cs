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

    public override void SetNextMove(MoveDirection dir)
    {
        base.SetNextMove(dir);

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
