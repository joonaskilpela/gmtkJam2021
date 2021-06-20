using UnityEngine;

public class PlatformerPlayer : Player
{
    public int maxJumps = 2;
    public int jumpHeight = 3;

    private int jumps = 0;

    protected override void Start()
    {
        base.Start();

        jumps = maxJumps;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (Input.GetKey(KeyCode.UpArrow) && jumps > 0) SetNextMove(MoveDirection.Up);
        else if (Input.GetKey(KeyCode.RightArrow)) SetNextMove(MoveDirection.Right);
        //else if (Input.GetKey(KeyCode.DownArrow)) SetNextMove(MoveDirection.Down);
        else if (Input.GetKey(KeyCode.LeftArrow)) SetNextMove(MoveDirection.Left);
    }

    public override void ReachedFlag()
    {
        base.ReachedFlag();

        SetAnimationRow(3);

        // Set flag reached
        FindObjectOfType<PlatformerGrid>().FlagReached = true;
    }

    protected override void SetAnimationRow(int row)
    {
        // Force layer 3 (cheer) if flag has been reached
        if (FindObjectOfType<PlatformerGrid>().FlagReached) row = 3;

        base.SetAnimationRow(row);
    }

    public override void SetNextMove(MoveDirection dir)
    {
        base.SetNextMove(dir);

        if (nextMove != MoveDirection.None)
        {
            // Reset flag reached when player moves
            FindObjectOfType<PlatformerGrid>().FlagReached = false;
        }

        switch (nextMove)
        {
            case MoveDirection.None:
                break;
            case MoveDirection.Up:
                break;
            case MoveDirection.Right:
                block.SetVector("_Tiling", new Vector2(-1, 1));
                break;
            case MoveDirection.Down:
                break;
            case MoveDirection.Left:
                block.SetVector("_Tiling", new Vector2(1, 1));
                break;
        }

        quadRenderer.SetPropertyBlock(block);
    }

    public override void OnTurnEnd()
    {
        if (nextMove == MoveDirection.None) DoGravity();

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

        // Try to do the tallest jump possible
        for (int i = jumpHeight; i > 0; i--)
        {
            nextMove = MoveDirection.Up;
            if (ExecuteMove(i, 0.1f * i)) break;
        }

        AudioPlayer.PlaySoundClip(AudioPlayer.SoundClip.Jump);

        SetAnimationRow(2);
    }

    protected override void MoveFinished()
    {
        if (lastMove != MoveDirection.Up) DoGravity();

        if (!CanMove(Vector3.down)) jumps = maxJumps;

        OOBCheck();
        OverlapCheck();

        SetAnimationRow(0);
    }
}
