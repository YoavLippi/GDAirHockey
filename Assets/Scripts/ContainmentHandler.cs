using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainmentHandler : MonoBehaviour
{
    public Collider2D gameBoard;

    void Start()
    {
    }

    private void FixedUpdate()
    {
        if (!gameBoard.bounds.Contains(transform.position))
        {
            Vector2 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, gameBoard.bounds.min.x, gameBoard.bounds.max.x);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, gameBoard.bounds.min.y, gameBoard.bounds.max.y);
            transform.position = clampedPosition;
        }
    }
}