using UnityEngine;

public class Girder : GridObject
{
    public override bool CanMove(Vector3 direction)
    {
        var collider = GetComponent<BoxCollider>();

        if(Physics.BoxCast(transform.position, collider.size * 0.48f, direction, transform.rotation, direction.magnitude))
        {
            return false;
        }

        return true;
    }

    public override bool IsDirectionAllowed(Vector3 direction)
    {
        var collider = GetComponent<BoxCollider>();

        if (Physics.BoxCast(transform.position, collider.size * 0.48f, direction, transform.rotation, direction.magnitude))
        {
            return false;
        }

        return true;
    }
}
