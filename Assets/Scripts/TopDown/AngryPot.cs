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
        if (CanMove(direction, out var blocker))
        {
            DoMove(direction);
        }
        else
        {
            if (blocker is Player)
            {
                blocker.Destroy();
                DoMove(direction);
                return;
            }

            direction = direction * -1;
            DoMove(direction);
        }

        block.SetVector("_BaseMap_ST", new Vector4(-direction.x, 1, 1, 1));
        potRenderer.SetPropertyBlock(block);

        OOBCheck();
    }
}
