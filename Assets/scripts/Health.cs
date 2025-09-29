using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 100;
    private int currentHealth;

    private Animator animator;

    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " tomou " + damage + " de dano. Vida restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("GetHit");
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " morreu.");
        animator.SetTrigger("Die");

        GetComponent<PlayerController>().enabled = false;
        GetComponent<PlayerCombat>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
    }
}