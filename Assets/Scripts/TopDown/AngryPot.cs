using UnityEngine;

public class AngryPot : GridObject
{
    public Renderer potRenderer;
    private Vector3 direction = Vector3.left;
    private MaterialPropertyBlock block;

    private void Start()
    {
        block = new MaterialPropertyBlock();
        potRenderer.GetPropertyBlock(block);
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

            block.SetVector("_BaseMap_ST", new Vector4(direction.x, 1, 1, 1));
            potRenderer.SetPropertyBlock(block);
            
            direction = direction * -1;
            DoMove(direction);
        }

        OOBCheck();
    }
}
