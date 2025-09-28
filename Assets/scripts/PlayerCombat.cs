using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    private InputSystem_Actions playerInput;
    private Animator animator;

    //  Lógica do Combo 
    private int comboCounter = 0;
    public float comboResetTime = 1.0f; 
    private float lastAttackTime;
  

    private void Awake()
    {
        playerInput = new InputSystem_Actions();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        playerInput.Player.Enable();
        playerInput.Player.Attack.performed += OnAttack;
    }

    private void OnDisable()
    {
        playerInput.Player.Disable();
        playerInput.Player.Attack.performed -= OnAttack;
    }

    void Update()
    {
        if (Time.time - lastAttackTime > comboResetTime)
        {
            comboCounter = 0;
        }
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
    }


}