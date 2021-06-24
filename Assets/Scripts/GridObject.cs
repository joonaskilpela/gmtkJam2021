using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
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

    protected virtual void OnEnable()
    {
        // Check that newly spawned object doesnt overlap with a player
        var players = FindObjectsOfType<Player>();

        foreach (var player in players) player.OverlapCheck();
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
    /// Check if this object is out of bounds, returns true if object is out of bounds
    /// </summary>
    /// <returns></returns>
    public bool OOBCheck()
    {
        // Skip OOB Check when object is not active
        if (!gameObject.activeSelf) return false;

        if (transform.position.y <= -7)
        {
            Debug.Log($"{name} left the world");

            Destroy(DestroyedBy.Fall);

            return true;
        }

        return false;
    }

    public enum DestroyedBy
    {
        /// <summary>
        /// Removed from game
        /// </summary>
        Removal,
        /// <summary>
        /// Fell out of the level
        /// </summary>
        Fall,
        /// <summary>
        /// Collision with enemy
        /// </summary>
        Enemy,
        /// <summary>
        /// Collision with spikes
        /// </summary>
        Spike,
        /// <summary>
        /// Overlap with crate/girder
        /// </summary>
        Crushed
    }

    /// <summary>
    /// Destroy this object
    /// </summary>
    public virtual void Destroy(DestroyedBy reason)
    {
        //if (IsMoving) currentTween.Complete();

        Debug.Log($"{name} was destroyed by {reason}");

        // Play falling sound
        if (reason == DestroyedBy.Fall) AudioPlayer.PlaySoundClip(AudioPlayer.SoundClip.FallCrash);

        //Destroy(gameObject);
        gameObject.SetActive(false);

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
    /// Get the state of this object
    /// </summary>
    /// <returns></returns>
    public virtual GridObjectState GetState()
    {
        return new GridObjectState
        {
            active = gameObject.activeSelf,
            gridObject = this,
            position = transform.position
        };
    }

    public virtual void RestoreState(GridObjectState state)
    {
        // Complete tween if it exists
        if (IsMoving) currentTween.Complete();

        // First set position to avoid overlap check in OnEnable
        transform.position = state.position;

        // If this gameobject needs to deactivate, do it in the restore step, before other object activate
        // To give colliders time to realise they dont overlap with this
        if (gameObject.activeSelf && !state.active) gameObject.SetActive(false);
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
        // If object is not active, dont do gravity
        if (!gameObject.activeSelf) return;

        // If object cant move down, dont do gravity
        if (!IsDirectionAllowed(Vector3.down)) return;

        // Calculate maximum fall height to do it all in a single move
        int fallLength;
        for (fallLength = 1; fallLength < 10; fallLength++)
        {
            if (!IsDirectionAllowed(Vector3.down * fallLength, out List<GridObject> blockers))
            {
                fallLength--;
                break;
            }

            // If we ran into any spikes
            if (blockers.Any(o => o is SpikeTrap))
            {
                // Stop fall short here
                break;
            }
            if (blockers.Any(o => o is Goal))
                break;
            // If we ran into anything
            if(blockers.Count > 0)
            {
                // Stop fall short here
                fallLength--;
                break;
            }
        }

        DoMove(Vector3.down * fallLength, 0.1f * fallLength);
    }


    /// <summary>
    /// Returns true if there are no colliders in the way of given movement
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public virtual bool CanMove(Vector3 direction) => CanMove(direction, out _);

    /// <summary>
    /// Returns true if there are no colliders in the way, also gives the objects that are blocking the way (in the same cell)
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="blockers"></param>
    /// <returns></returns>
    public virtual bool CanMove(Vector3 direction, out List<GridObject> blockers)
    {
        blockers = new List<GridObject>();

        var ray = new Ray(transform.position, direction);
        var hits = Physics.RaycastAll(ray, direction.magnitude);

        var canmove = true;

        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                var obj = hit.collider.GetComponent<GridObject>();

                // If there is an object in the way
                if (obj)
                {
                    blockers.Add(obj);

                    // And object can move in this direction
                    if (obj.CanPush(direction))
                    {
                        // Push object and allow movement
                        obj.Push(direction, this);
                    }
                    else
                    {
                        // If object cannot be pushed, check if its moving out of the way
                        if (!obj.IsMovingOutOfWay(this, direction.ToMoveDirection())) canmove = false;
                    }
                }
                else
                {
                    // Collider wasnt a grid object, it always blocks movement (outer walls etc)
                    canmove = false;
                }
            }
        }

        return canmove;
    }

    public virtual bool IsDirectionAllowed(Vector3 direction) => IsDirectionAllowed(direction, out _);

    /// <summary>
    /// Same as CanMove, but does not push objects, also gives the objects that are blocking the way (in the same cell)
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="blocker"></param>
    /// <returns></returns>
    public virtual bool IsDirectionAllowed(Vector3 direction, out List<GridObject> blockers)
    {
        blockers = new List<GridObject>();

        var ray = new Ray(transform.position, direction);
        var hits = Physics.RaycastAll(ray, direction.magnitude);

        var canmove = true;

        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                var obj = hit.collider.GetComponent<GridObject>();

                // If there is an object in the way
                if (obj)
                {
                    blockers.Add(obj);

                    // And object can move in this direction
                    if (obj.CanPush(direction))
                    {
                        // No pushing
                    }
                    else
                    {
                        // If object cannot be pushed, check if its moving out of the way
                        if(!obj.IsMovingOutOfWay(this, direction.ToMoveDirection())) canmove = false;
                    }
                }
                else
                {
                    // Collider wasnt a grid object, it always blocks movement (outer walls etc)
                    canmove = false;
                }
            }
        }

        return canmove;
    }

    /// <summary>
    /// Returns true if the object is going to move out of the way, called when something is trying to move into this object
    /// </summary>
    /// <param name="moveDirection"></param>
    /// <returns></returns>
    public virtual bool IsMovingOutOfWay(GridObject other, MoveDirection moveDirection)
    {
        return false;
    }

    /// <summary>
    /// Moves this object by direction vector
    /// </summary>
    /// <param name="direction"></param>
    public virtual void DoMove(Vector3 direction, float duration = 0.2f, bool callback = true)
    {
        if (IsMoving) currentTween.Complete();

        currentTween = transform.DOMove(transform.position + direction, duration);

        if(callback) currentTween.OnComplete(MoveFinished);

        // Check if we are out of the world every frame, and stop tween early if so
        currentTween.OnUpdate(() =>
        {
            if (OOBCheck()) currentTween.Complete();
        });
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
