using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public bool isDead;
    GameObject showText;


    private void Awake()
    {
        currentHealth = maxHealth;
        showText = Resources.Load<GameObject>("Prefabs/ShowText");
    }

    public void TakeDamage(Attack attack)
    {
        currentHealth -= attack.attackDamage;

        GameObject spawnedText = Instantiate(showText, transform);
        spawnedText.transform.position = (Vector2)transform.position + new Vector2(0,1);
        spawnedText.GetComponent<ShowText>().SetText(attack.attackDamage.ToString());
        spawnedText.GetComponent<ShowText>().fontSize(18, attack.attackDamage);

        // 모험가 AI일 때
        if (TryGetComponent<AdventurerAI>(out AdventurerAI adventure))
        {
            adventure.SetTarget(attack.GetComponent<CanSelectObject>());
        }
        else if (GetComponent<MonsterAI>())
        {
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }
}
