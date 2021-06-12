using DG.Tweening;

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

    public Tweener ExecuteMove()
    {
        Tweener tween = null;

        lastMove = nextMove;

        if (nextMove != MoveDirection.None)
        {
            var dir = nextMove.ToVector3();

            if (CanMove(dir)) tween = DoMove(nextMove.ToVector3());
        }

        nextMove = MoveDirection.None;

        return tween;
    }

    public override void OnTurnEnd()
    {
        ExecuteMove();
    }
}
