using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalConnection : MonoBehaviour
{
    [SerializeField] private GameObject otherPortal;

    private CapsuleCollider2D portalCollider, otherPortalCollider;

    private SpriteRenderer thisSprite, otherSprite;

    // Start is called before the first frame update
    void Start()
    {
        portalCollider = GetComponent<CapsuleCollider2D>();
        otherPortalCollider = otherPortal.GetComponent<CapsuleCollider2D>();
        thisSprite = GetComponent<SpriteRenderer>();
        otherSprite = otherPortal.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Puck"))
        {
            col.gameObject.transform.position = otherPortal.transform.position;
            StartCoroutine(nameof(Cooldown));
        }
    }

    private IEnumerator Cooldown()
    {
        portalCollider.enabled = false;
        otherPortalCollider.enabled = false;
        yield return new WaitForSeconds(0.8f);
        portalCollider.enabled = true;
        otherPortalCollider.enabled = true;
    }
}