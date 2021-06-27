using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SwipeType = InputTest.SwipeType;

public class PlayerTest : MonoBehaviour
{

    public InputTest inputTest;

    float moveUnit = 12.5f;

    void Awake()
    {
        inputTest.onSwipe.AddListener(Move);
    }

    // TODO : 부드러운 움직임 적용 필요
    void Move(SwipeType swipeType)
    {
        Vector3 direction = Vector3.zero;

        switch (swipeType)
        {
            case SwipeType.LEFT:
                direction = Vector3.left;
                break;
            case SwipeType.RIGHT:
                direction = Vector3.right;
                break;
            case SwipeType.UP:
                direction = Vector3.forward;
                break;
            case SwipeType.DOWN:
                direction = Vector3.back;
                break;
            default:
                break;
        }

        transform.position += direction * moveUnit;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}
