using UnityEngine;

public class Crate : GridObject
{
    private void OnEnable()
    {
        // Check that newly spawned box doesnt overlap with a player
        var players = FindObjectsOfType<Player>();

        foreach (var player in players) player.OverlapCheck();
    }

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
