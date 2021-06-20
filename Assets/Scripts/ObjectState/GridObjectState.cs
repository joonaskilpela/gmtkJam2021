using System;
using UnityEngine;

/// <summary>
/// Represents a state of object
/// </summary>
public class GridObjectState
{
    /// <summary>
    /// The object this state belongs to
    /// </summary>
    public GridObject gridObject;

    /// <summary>
    /// Is the object active
    /// </summary>
    public bool active;

    /// <summary>
    /// The position of the object
    /// </summary>
    public Vector3 position;

    public virtual void CopyTo(GridObjectState other)
    {
        other.gridObject = gridObject;
        other.active = active;
        other.position = position;
    }

    /// <summary>
    /// Restore all object properties
    /// </summary>
    public void Restore()
    {
        gridObject.RestoreState(this);
    }

    /// <summary>
    /// Restore object active state
    /// </summary>
    public void RestoreActive()
    {
        if (active != gridObject.gameObject.activeSelf) gridObject.gameObject.SetActive(active);
    }
}

