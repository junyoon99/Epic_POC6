using UnityEngine;

public class Attack : MonoBehaviour
{
    public float attackRange = 1.0f;
    public float attackDelay = 1.0f;
    public float attackDamage = 10.0f;

    CanSelectObject target;
    Rigidbody2D rb2d;
    public Move move;

    float attackCooldown = 0f;

    private void Awake()
    {
        move = GetComponent<Move>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (target == null) 
        {
            attackCooldown = 0;
            return;
        } 
        attackCooldown += Time.fixedDeltaTime;
        if (Vector2.Distance(rb2d.position, target.transform.position) < attackRange && target.CheckCurrentRoom() == GetComponent<CanSelectObject>().CheckCurrentRoom())
        {
            if (move)
            {
                move.ComponentDisable();
            }
            if (attackCooldown >= attackDelay)
            {

                AttackTarget();
            }
        }
        else
        {
            if (move)
            {
                move.enabled = true;
            }
        }
    }

    public void AttackTarget() 
    {
        attackCooldown = 0f;

        if (target.TryGetComponent<Health>(out Health targetHealth))
        {
            targetHealth.TakeDamage(this);
        }
    }

    public void SetAttackTarget(CanSelectObject target)
    {
        this.target = target;
        this.enabled = true;
    }

    public void DisableAttack()
    {
        attackCooldown = 0f;
        target = null;
        this.enabled = false;
    }
}
