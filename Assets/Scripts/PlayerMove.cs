using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float forceFactor;

    public Collider2D gameBoard;

    private float _extentX, _extentY;

    private Bounds _gameBounds;

    private Rigidbody2D _rb;

    private bool _active = false;

    // Start is called before the first frame update
    void Start()
    {
        _extentX = GetComponent<SpriteRenderer>().bounds.extents.x;
        _extentY = GetComponent<SpriteRenderer>().bounds.extents.y;
        _gameBounds = gameBoard.bounds;
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (_active)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 clampedPosition = mousePosition;

            if (!gameBoard.bounds.Contains(clampedPosition))
            {
                clampedPosition.x = Mathf.Clamp(clampedPosition.x, _gameBounds.min.x + _extentX,
                    _gameBounds.max.x - _extentX);
                clampedPosition.y = Mathf.Clamp(clampedPosition.y, _gameBounds.min.y + _extentY,
                    _gameBounds.max.y - _extentY);
            }

            //transform.position = clampedPosition;
            _rb.MovePosition(clampedPosition);
        }
    }

    private void Begin()
    {
        _active = !_active;
        if (!_active)
        {
            transform.position = new Vector3(-4f, 0f, 0f);
        }
    }
}