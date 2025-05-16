using System;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public Action Arrived;
    public float moveSpeed = 5f;
    Rigidbody2D rb2d;

    Vector2? targetPosition;

    CanSelectObject targetObject;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (targetObject != null)
        {
            Vector2 direction = ((Vector2)targetObject.transform.position - rb2d.position).normalized;
            rb2d.linearVelocity = direction * moveSpeed;
        }
        else if (targetPosition != null)
        {
            Vector2 direction = ((Vector2)targetPosition - rb2d.position).normalized;
            rb2d.linearVelocity = direction * moveSpeed;
            if (Vector2.Distance(rb2d.position, (Vector2)targetPosition) < 0.1f) 
            {
                Arrived?.Invoke();
            }
        }

    }

    public void SetTarget(CanSelectObject targetObject, Vector2? targetPosition) 
    {
        this.enabled = true;
        this.targetObject = targetObject;
        this.targetPosition = targetPosition;
    }

    public void ComponentDisable() 
    {
        this.enabled = false;
    }
}
