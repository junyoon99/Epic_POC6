using UnityEngine;

public class Interact : MonoBehaviour
{
    public float interactRange = 1.0f;

    Core targetCore;
    Rigidbody2D rb2d;
    public Move move;

    private void Awake()
    {
        move = GetComponent<Move>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (targetCore == null)
        {
            return;
        }
        if (Vector2.Distance(rb2d.position, targetCore.transform.position) < interactRange)
        {
            if (move)
            {
                move.ComponentDisable();
            }
            targetCore.Interact(GetComponent<CanSelectObject>());
        }
        else
        {
            if (move)
            {
                move.enabled = true;
            }
        }
    }

    public void SetTarget(Core targetCore)
    {
        this.targetCore = targetCore;
        this.enabled = true;
    }
    public void ComponentDisable()
    {
        targetCore = null;
        this.enabled = false;
    }
}
