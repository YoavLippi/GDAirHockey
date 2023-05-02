using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    [SerializeField] private float forceFactor;

    [SerializeField] private GameObject puck;

    [SerializeField] private float moveSpeed;

    [SerializeField] private PuckMovement puckInfo;

    [SerializeField] private GameObject goal;
    
    [SerializeField] private float minDist, timeToReact;

    [SerializeField] private bool hardMode;

    public Collider2D gameBoard;

    private float _extentX, _extentY;

    private Bounds _gameBounds;

    private Rigidbody2D _rb;

    private bool _active = false;

    private Vector2 currentDesire;
    
    private int moveFrameCount;

    private bool dirSwitch = true;

    // Start is called before the first frame update
    private void Start()
    {
        moveFrameCount = 0;
        _extentX = GetComponent<SpriteRenderer>().bounds.extents.x;
        _extentY = GetComponent<SpriteRenderer>().bounds.extents.y;
        _gameBounds = gameBoard.bounds;
        _rb = GetComponent<Rigidbody2D>();
        StartCoroutine(ReactionTime());
    }

    
    private void FixedUpdate()
    {
        if (_active)
        {
            Vector2 clampedPosition = currentDesire;
            Vector2 currentPos = transform.position;
            
            Rigidbody2D puckBody = puck.GetComponent<Rigidbody2D>();
            Debug.DrawRay(currentPos, getPerpendicular(currentPos-puckBody.position), Color.cyan);
            Debug.DrawRay(puckBody.position, puckBody.velocity.normalized, Color.red);
            
            if (!_gameBounds.Contains(clampedPosition))
            {
                clampedPosition.x = Mathf.Clamp(clampedPosition.x, _gameBounds.min.x + _extentX,
                    _gameBounds.max.x - _extentX);
                clampedPosition.y = Mathf.Clamp(clampedPosition.y, _gameBounds.min.y + _extentY,
                    _gameBounds.max.y - _extentY);
            }
            
            Vector2 newDir = (clampedPosition - currentPos).normalized * (Time.deltaTime * moveSpeed);
            Vector2 newPos = currentPos + newDir;
            
            newPos.x = Mathf.Clamp(newPos.x, _gameBounds.min.x + _extentX, _gameBounds.max.x - _extentX);
            newPos.y = Mathf.Clamp(newPos.y, _gameBounds.min.y + _extentY, _gameBounds.max.y - _extentY);

            if (Vector2.Distance(currentPos, clampedPosition) < minDist)
            {
                _rb.MovePosition(clampedPosition);
            }
            else
            {
                _rb.MovePosition(newPos);
            }

            /*if (newPos == currentPos)
            {
                moveFrameCount++;
                if (moveFrameCount >= 10)
                {
                    dirSwitch = !dirSwitch;
                    moveFrameCount = 0;
                }
            }
            else
            {
                moveFrameCount = 0;
            }*/
        }
    }

    private IEnumerator ReactionTime()
    {
        while (true)
        {
            if (_active)
            {
                currentDesire = AIMove();
            }
            yield return new WaitForSeconds(timeToReact/1000f);
        }
    }

    //Coding AI: Two phases - attack and defense
    private Vector2 AIMove()
    {
        Vector2 currentPos = transform.position;
        Rigidbody2D puckBody = puck.GetComponent<Rigidbody2D>();
        Vector2 midpoint = (goal.transform.position + puck.transform.position) / 2;
        
        //We want it to move between the goal and the puck
        Vector2 desiredMovement = midpoint;

        if (!puck.gameObject.activeSelf)
        {
            desiredMovement = new Vector2(4f, 0f);
            return desiredMovement;
        }
        
        //Always move away if it was the last thing to hit the puck
        if (puckInfo.LastHit == this.gameObject)
        {
            RaycastHit2D hitInfo =
                Physics2D.Raycast(puck.transform.position, puckBody.velocity.normalized, Mathf.Infinity, LayerMask.GetMask("Paddles"));
            if (hitInfo)
            {
                if (hitInfo.collider.gameObject == gameObject)
                {
                    //Debug.Log("It's coming towards me AHHHH!!!");
                    desiredMovement = currentPos + getPerpendicular(currentPos-puckBody.position);
                    if (currentPos.x >= 4)
                    {
                        desiredMovement.x = -Mathf.Abs(desiredMovement.x);
                    }
                    else
                    {
                        desiredMovement.x = Mathf.Abs(desiredMovement.x);
                    }
                    
                    return desiredMovement;
                }
            }
            else
            {
                //This behaviour makes the AI a far better defender -- Turn it off if you are having difficulties
                if (hardMode)
                {
                    if (puckBody.position.x < 0f)
                    {
                        return midpoint;
                    }
                }

                if (puckBody.velocity.magnitude < 0.05f)
                {
                    return puckBody.position;
                }
                desiredMovement = currentPos + (currentPos - puckBody.position);
                //This bit of code makes the AI play much better, but it hits the ball too much
                /*if (puckBody.velocity.magnitude == 0 || (puckBody.velocity.x <0.1f && puckBody.position.x >0))
                {
                    desiredMovement = puckBody.position;
                    Debug.Log("Guess I'll have to do it myself... smhing my head so bad rnn");
                }*/
                return desiredMovement;
            }
        }
        
        //Attack
        Vector2 goalPos = new Vector2(goal.transform.position.x, goal.transform.position.y);
        if (Physics2D.Raycast(puckBody.position, goalPos - puckBody.position, Mathf.Infinity, 1 << 7) && puckBody.position.x > -0.5f)
        {
            Debug.Log("Attack mode");
            desiredMovement = puckBody.position;
            return desiredMovement;
        }

        return desiredMovement;
    }

    private Vector2 getPerpendicular(Vector2 original)
    {
        float x = -original.y;
        float y = original.x;
        return new Vector2(x, y);
    }

    private void Begin()
    {
        _active = !_active;
        transform.position = new Vector3(4f, 0f);
    }
}