using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    private InputSystem_Actions playerInput;
    private Animator animator;

    private int comboCounter = 0;
    public float comboResetTime = 1.0f;
    private float lastAttackTime;

    [Header("Configurações de Ataque")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private LayerMask enemyLayers;

    private void Awake()
    {
        playerInput = new InputSystem_Actions();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        playerInput.Player.Enable();
        playerInput.Player.Attack.performed += OnAttack;
        playerInput.Player.Defend.performed += OnDefend;
        playerInput.Player.Defend.canceled += OnDefendReleased;
    }

    private void OnDisable()
    {
        playerInput.Player.Disable();
        playerInput.Player.Attack.performed -= OnAttack;
        playerInput.Player.Defend.performed -= OnDefend;
        playerInput.Player.Defend.canceled -= OnDefendReleased;
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (Time.time - lastAttackTime > comboResetTime)
        {
            comboCounter = 0;
        }

        comboCounter++;

        if (comboCounter == 1)
        {
            animator.SetInteger("AttackIndex", 1);
        }
        else if (comboCounter == 2)
        {
            animator.SetInteger("AttackIndex", 2);
            comboCounter = 0;
        }

        lastAttackTime = Time.time;
        PerformAttack();
    }

    void PerformAttack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }

    private void OnDefend(InputAction.CallbackContext context)
    {
        animator.SetBool("isDefending", true);
    }

    private void OnDefendReleased(InputAction.CallbackContext context)
    {
        animator.SetBool("isDefending", false);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}