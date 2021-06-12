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
}