using UnityEngine;

public class Crate : GridObject
{
    private bool destroyAfterMove = false;
    private SpikeTrap destroySpikeAfterMove = null;
    public GameObject deadPot;

    protected override void OnEnable()
    {
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, Vector3.one * 0.1f, Vector3.forward, transform.rotation, 1f);

        foreach (var hit in hits)
        {
            var obj = hit.collider.GetComponent<GridObject>();

            // Falling crate smashes angry pot
            if (obj is AngryPot)
            {
                obj.Destroy(DestroyedBy.Crushed);
                deadPot.SetActive(true);
            }
        }
    }

    public override void RestoreState(GridObjectState state)
    {
        base.RestoreState(state);

        // Set destroy after move to false when state was rewinded
        destroyAfterMove = false;
        destroySpikeAfterMove = null;
    }

    public override bool CanMove(Vector3 direction)
    {
        var ray = new Ray(transform.position, direction);

        if (Physics.Raycast(ray, out var hit, direction.magnitude))
        {
            var obj = hit.collider.GetComponent<GridObject>();

            // If there was an object, and object was spiketrap
            if (obj is SpikeTrap spike)
            {
                destroyAfterMove = true;
                destroySpikeAfterMove = spike;

                return true;
            }

            return false;
        }

        return true;
    }

    protected override void MoveFinished()
    {
        base.MoveFinished();

        if (destroyAfterMove)
        {
            // Destroy spiketrap
            destroySpikeAfterMove.Destroy(DestroyedBy.Removal);

            // Destroy crate
            Destroy(DestroyedBy.Removal);
        }
    }
}
