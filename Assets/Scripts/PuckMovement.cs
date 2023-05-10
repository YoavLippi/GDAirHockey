using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuckMovement : MonoBehaviour
{
    private Rigidbody2D thisBody;

    [SerializeField] private float maxVelocity, friction;

    [SerializeField] private GameObject lastHit;

    [SerializeField] private Material innerMat;
    
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private List<AudioClip> SFX;

    private bool onCooldown = false;
    public GameObject LastHit => lastHit;

    // Start is called before the first frame update
    void Start()
    {
        thisBody = GetComponent<Rigidbody2D>();
        GetComponentsInChildren<SpriteRenderer>()[1].material = innerMat;
    }

    private void FixedUpdate()
    {
        var velocity = thisBody.velocity;
        if (velocity.magnitude > maxVelocity)
        {
            Vector2 currentVal = velocity.normalized;
            velocity = currentVal * maxVelocity;
            thisBody.velocity = velocity;
        }

        if (velocity.magnitude > 0)
        {
            velocity = velocity.normalized * (velocity.magnitude * friction);
            thisBody.velocity = velocity;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Paddle"))
        {
            audioSource.clip = SFX[0];
            audioSource.volume = 0.5f * (thisBody.velocity.magnitude/maxVelocity);
            audioSource.pitch = Mathf.Clamp(0.9f + (col.relativeVelocity.magnitude/maxVelocity), 0f, 1f);
            audioSource.Play(0);
            
            if (col.gameObject == lastHit)
            {
                if (!onCooldown)
                {
                    SendMessageUpwards("DoubleHit", col.gameObject.name);
                    onCooldown = true;
                    StartCoroutine(nameof(cooldown));
                }
            }
            else
            {
                lastHit = col.gameObject;
                GetComponentsInChildren<SpriteRenderer>()[1].material =
                    col.gameObject.GetComponent<SpriteRenderer>().material;
            }
        }

        if (col.gameObject.CompareTag("Board"))
        {
            audioSource.clip = SFX[2];
            audioSource.volume = 0.5f * (thisBody.velocity.magnitude/maxVelocity);
            audioSource.pitch = 1f;
            audioSource.Play(0);
        }
    }

    private void OnEnable()
    {
        onCooldown = false;
        lastHit = null;
        GetComponentsInChildren<SpriteRenderer>()[1].material = innerMat;
    }

    private IEnumerator cooldown()
    {
        yield return new WaitForSeconds(0.4f);
        onCooldown = false;
    }
}