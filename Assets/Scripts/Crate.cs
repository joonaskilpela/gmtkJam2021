using UnityEngine;

public class Crate : GridObject
{
    public override bool CanMove(Vector3 direction)
    {
        var ray = new Ray(transform.position, direction);

        if (Physics.Raycast(ray, out var hit, direction.magnitude))
        {
            var obj = hit.collider.GetComponent<GridObject>();

            // If there was an object, and object was spiketrap
            if (obj is SpikeTrap)
            {
                // Destroy spiketrap
                obj.Destroy();

                // Destroy crate
                Destroy();

                return true;
            }

            return false;
        }

        return true;
    }
}
