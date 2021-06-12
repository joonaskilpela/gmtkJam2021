using UnityEngine;

public class PlatformerPlayer : Player
{
    public int maxJumps = 2;
    public int jumpHeigth = 3;

    private int jumps = 0;

    private void Start()
    {
        jumps = maxJumps;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow) && jumps > 0) SetNextMove(MoveDirection.Up);
        else if (Input.GetKey(KeyCode.RightArrow)) SetNextMove(MoveDirection.Right);
        else if (Input.GetKey(KeyCode.DownArrow)) SetNextMove(MoveDirection.Down);
        else if (Input.GetKey(KeyCode.LeftArrow)) SetNextMove(MoveDirection.Left);
    }

    public override void OnTurnEnd()
    {
        // If player is jumping
        if (nextMove == MoveDirection.Up)
        {
            // Player needs to have jumps left
            if (jumps > 0)
            {
                // Do a jump
                Jump();
            }
        }
        else
        {
            ExecuteMove();
        }

        OOBCheck();
    }

    private void Jump()
    {
        jumps--;

        ExecuteMove(jumpHeigth, 0.2f * jumpHeigth);
    }

    public override void DoGravity()
    {
        if (CanMove(Vector3.down))
        {
            DoMove(Vector3.down);
        }
    }

    protected override void MoveFinished()
    {
        if (lastMove != MoveDirection.Up && lastMove != MoveDirection.Down) DoGravity();

        if (!CanMove(Vector3.down)) jumps = maxJumps;

        OOBCheck();
    }
}
