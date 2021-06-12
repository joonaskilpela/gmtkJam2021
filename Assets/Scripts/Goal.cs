using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : GridObject
{
    public override bool CanPush(Vector3 dir)
    {
        return true;
    }

    public override void Push(Vector3 dir)
    {
        // Win condition
    }
}
