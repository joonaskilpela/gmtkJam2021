using System.Linq;
using UnityEngine;

public class AngryPot : GridObject
{
    public Renderer potRenderer;
    public MoveDirection direction = MoveDirection.Left;
    private MaterialPropertyBlock block;

    protected override void Start()
    {
        base.Start();

        block = new MaterialPropertyBlock();
        potRenderer.GetPropertyBlock(block);

        block.SetVector("_BaseMap_ST", new Vector4(-direction.ToVector3().x, 1, 1, 1));
        potRenderer.SetPropertyBlock(block);
    }

    public override void OnTurnEnd()
    {
        CheckAndMove();

        if (block != null)
        {
            block.SetVector("_BaseMap_ST", new Vector4(-direction.ToVector3().x, 1, 1, 1));
            potRenderer.SetPropertyBlock(block);
        }

        OOBCheck();
    }

    private void CheckAndMove()
    {
        Vector3 dir = direction.ToVector3();

        if (CanMove(dir, out var blockers))
        {
            DoMove(dir);
            return;
        }
        else
        {
            // If there was a player blocking
            if (blockers.Any(o => o is Player))
            {
                // Pick out the player
                Player player = (Player)blockers.First(o => o is Player);

                // Destroy player if theyre not escaping
                if (!player.WillEscapeEnemy(this, direction)) player.Destroy(DestroyedBy.Enemy);

                DoMove(dir);
                return;
            }

            direction = direction.Opposite();
            CheckAndMove();

            return;
        }
    }
}
