using UnityEngine;

public enum MoveDirection
{
    None,
    Up,
    Right,
    Down,
    Left
}

public static class MoveDirectionExt
{
    /// <summary>
    /// Convert movedirection to the vector it represents
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static Vector3 ToVector3(this MoveDirection d)
    {
        switch (d)
        {
            case MoveDirection.None:
                return Vector3.zero;
            case MoveDirection.Up:
                return Vector3.up;
            case MoveDirection.Right:
                return Vector3.right;
            case MoveDirection.Down:
                return Vector3.down;
            case MoveDirection.Left:
                return Vector3.left;
        }

        return Vector3.zero;
    }

    /// <summary>
    /// Convert vector3 into movedirection
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static MoveDirection ToMoveDirection(this Vector3 v)
    {
        if (v == Vector3.zero) return MoveDirection.None;
        else if (v == Vector3.up) return MoveDirection.Up;
        else if (v == Vector3.right) return MoveDirection.Right;
        else if (v == Vector3.down) return MoveDirection.Down;
        else if (v == Vector3.left) return MoveDirection.Left;

        return MoveDirection.None;
    }

    /// <summary>
    /// Get the opposite movedirection
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static MoveDirection Opposite(this MoveDirection d)
    {
        switch (d)
        {
            case MoveDirection.None:
                return MoveDirection.None;
            case MoveDirection.Up:
                return MoveDirection.Down;
            case MoveDirection.Right:
                return MoveDirection.Left;
            case MoveDirection.Down:
                return MoveDirection.Up;
            case MoveDirection.Left:
                return MoveDirection.Right;
        }

        return MoveDirection.None;
    }
}