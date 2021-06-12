using UnityEngine;

public class Girder : GridObject
{
    public override bool CanMove(Vector3 direction)
    {
        if(Physics.BoxCast(transform.position, transform.localScale * 0.48f, direction, transform.rotation, direction.magnitude))
        {
            return false;
        }

        return true;
    }
}
