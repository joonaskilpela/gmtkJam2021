using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Destroys all cracked bricks in the scene when it explodes
/// </summary>
public class TNT : GridObject
{
    public UnityEvent OnExplode;

    // Always allow pushing (for interaction)
    public override bool CanPush(Vector3 dir)
    {
        return true;
    }

    public override void Push(Vector3 dir)
    {
        // Find and destroy all cracked bricks
        var crackedBricks = FindObjectsOfType<CrackedBrick>();
        foreach (var brick in crackedBricks) brick.Destroy();

        OnExplode?.Invoke();
        Destroy();
    }
}
