using UnityEngine;

public class AngryPot : GridObject
{
    public Renderer potRenderer;
    public Vector3 direction = Vector3.left;
    private MaterialPropertyBlock block;

    protected override void Start()
    {
        base.Start();

        block = new MaterialPropertyBlock();
        potRenderer.GetPropertyBlock(block);

        block.SetVector("_BaseMap_ST", new Vector4(-direction.x, 1, 1, 1));
        potRenderer.SetPropertyBlock(block);
    }

    public override void OnTurnEnd()
    {
        CheckAndMove();

        if (block != null)
        {
            block.SetVector("_BaseMap_ST", new Vector4(-direction.x, 1, 1, 1));
            potRenderer.SetPropertyBlock(block);
        }

        OOBCheck();
    }

    private void CheckAndMove()
    {
        if (CanMove(direction, out var blocker))
        {
            DoMove(direction);
            return;
        }
        else
        {
            if (blocker is Player)
            {
                blocker.Destroy(DestroyedBy.Enemy);
                DoMove(direction);
                return;
            }

            direction = direction * -1;
            CheckAndMove();

            return;
        }
    }
}
