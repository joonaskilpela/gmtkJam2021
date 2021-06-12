using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerPlayer : Player
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow)) SetNextMove(MoveDirection.Up);
        else if (Input.GetKey(KeyCode.RightArrow)) SetNextMove(MoveDirection.Right);
        else if (Input.GetKey(KeyCode.DownArrow)) SetNextMove(MoveDirection.Down);
        else if (Input.GetKey(KeyCode.LeftArrow)) SetNextMove(MoveDirection.Left);
    }

    public override void OnTurnEnd()
    {
        var tween = ExecuteMove();

        // If we moved, do gravity after moving
        if (tween != null)
        {
            // If we moved up, dont do gravity
            if (lastMove != MoveDirection.Up)
            {
                tween.OnComplete(() => DoGravity());
            }
        }
        else
        {
            DoGravity();
        }
    }
}
