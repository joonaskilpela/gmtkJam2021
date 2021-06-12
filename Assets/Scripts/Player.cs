public class Player : GridObject
{
    public MoveDirection nextMove;
    public MoveDirection lastMove;

    public void SetNextMove(MoveDirection dir)
    {
        var grids = FindObjectsOfType<GameGrid>();
        foreach (var grid in grids)
        {
            if (grid.CanEndTurn)
            {
                nextMove = dir;
                grid.WillEndTurn = true;
            }
        }
    }

    public void ExecuteMove(float length = 1f, float duration = 0.2f)
    {
        lastMove = nextMove;

        if (nextMove != MoveDirection.None)
        {
            var dir = nextMove.ToVector3() * length;

            if (CanMove(dir)) DoMove(dir, duration);
        }

        nextMove = MoveDirection.None;
    }

    public override void OnTurnEnd()
    {
        ExecuteMove();

        OOBCheck();
    }
}
