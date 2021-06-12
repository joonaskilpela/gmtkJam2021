using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Anything that lives in the grid and contains logic for it
/// </summary>
public abstract class GridObject : MonoBehaviour
{
    public bool HasGravity = false;
    public bool IsPushable = false;

    [Header("When this object leaves playable area")]
    public UnityEvent OnObjectDestroyed;

    private Tweener currentTween;

    public bool IsMoving
    {
        get
        {
            if (currentTween != null) return currentTween.IsActive();

            return false;
        }
    }

    /// <summary>
    /// Grid has just ended a turn
    /// </summary>
    public virtual void OnTurnEnd()
    {
        if (HasGravity) DoGravity();

        if (transform.position.y <= -7)
        {
            Debug.Log($"{name} left the world");

            Destroy();
        }
    }

    public virtual void Destroy()
    {
        if (IsMoving) currentTween.Complete();

        Destroy(gameObject);
        OnObjectDestroyed.Invoke();
    }

    /// <summary>
    /// Push this object in a direction
    /// </summary>
    /// <param name="dir"></param>
    public virtual void Push(Vector3 dir)
    {
        DoMove(dir);
    }

    /// <summary>
    /// Returns true if this object can be pushed in a direction
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public virtual bool CanPush(Vector3 dir)
    {
        // Object needs to be pushable
        if (!IsPushable) return false;

        // Return true if object can move in the direction
        return CanMove(dir);
    }

    /// <summary>
    /// Does one iteration of gravity, moves down if possible
    /// </summary>
    public void DoGravity()
    {
        if (CanMove(Vector3.down))
        {
            DoMove(Vector3.down);
        }
    }

    /// <summary>
    /// Returns true if there are no colliders in the way of given movement
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public virtual bool CanMove(Vector3 direction)
    {
        var ray = new Ray(transform.position, direction);

        if (Physics.Raycast(ray, out var hit, direction.magnitude))
        {
            var obj = hit.collider.GetComponent<GridObject>();

            // If there is an object in the way
            if (obj)
            {
                // And object can move in this direction
                if (obj.CanPush(direction))
                {
                    // Push object and allow movement
                    obj.Push(direction);
                    return true;
                }
            }

            return false;
        }

        return true;
    }

    /// <summary>
    /// Moves this object by direction vector
    /// </summary>
    /// <param name="direction"></param>
    public Tweener DoMove(Vector3 direction, float duration = 0.2f)
    {
        if (IsMoving) currentTween.Complete();

        currentTween = transform.DOMove(transform.position + direction, duration);

        return currentTween;
    }
}
