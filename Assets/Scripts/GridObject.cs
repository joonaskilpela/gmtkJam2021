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

    public UnityEvent OnMoveFinished;

    private Tweener currentTween;

    public bool IsMoving
    {
        get
        {
            if (currentTween != null) return currentTween.IsActive();

            return false;
        }
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {

    }

    /// <summary>
    /// Grid has just ended a turn
    /// </summary>
    public virtual void OnTurnEnd()
    {
        if (HasGravity) DoGravity();

        OOBCheck();
    }

    /// <summary>
    /// Check if this object is out of bounds
    /// </summary>
    public void OOBCheck()
    {
        if (transform.position.y <= -7)
        {
            Debug.Log($"{name} left the world");

            Destroy();
        }
    }

    /// <summary>
    /// Destroy this object
    /// </summary>
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
    public virtual void Push(Vector3 dir, GridObject pusher)
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
    public virtual void DoGravity()
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
    public virtual bool CanMove(Vector3 direction) => CanMove(direction, out _);

    /// <summary>
    /// Returns true if there are no colliders in the way, also gives the object that is blocking the way
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="blocker"></param>
    /// <returns></returns>
    public virtual bool CanMove(Vector3 direction, out GridObject blocker)
    {
        blocker = null;
        var ray = new Ray(transform.position, direction);

        if (Physics.Raycast(ray, out var hit, direction.magnitude))
        {
            var obj = hit.collider.GetComponent<GridObject>();

            // If there is an object in the way
            if (obj)
            {
                blocker = obj;

                // And object can move in this direction
                if (obj.CanPush(direction))
                {
                    // Push object and allow movement
                    obj.Push(direction, this);
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
    public virtual void DoMove(Vector3 direction, float duration = 0.2f, bool callback = true)
    {
        if (IsMoving) currentTween.Complete();

        currentTween = transform.DOMove(transform.position + direction, duration);
        currentTween.OnComplete(MoveFinished);
    }

    /// <summary>
    /// Called when move tween completed
    /// </summary>
    protected virtual void MoveFinished()
    {
        if (HasGravity) DoGravity();

        OOBCheck();
    }
}
