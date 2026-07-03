using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpForce = 15f;
    public float gravityMultiplier = 2f;

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isGrounded;
    private float currentSpeed;

    private float groundCheckDistance = 1f;
    public LayerMask groundLayer;
    private Transform groundCheckOrigin;

    
    public bool IsSprinting { get; private set; }
    private bool sprintState = false;

    void Start()
    {
        groundCheckOrigin = gameObject.transform.Find("GroundCheckOrigin");
        rb = GetComponent<Rigidbody>();
        currentSpeed = moveSpeed;
    }

    void Update()
    {
        RaycastGroundCheck();
        
        // Stop sprinting if aiming or moving not forward
        if (IsSprinting && moveInput.y <= 0.1f)
        {
            IsSprinting = false;
            UpdateMovementSpeed();
        }
    }

    void FixedUpdate()
    {
        Move();

        rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
        
        float targetSpeed = currentSpeed;
        if (IsSprinting && moveInput.y > 0.1f)
            targetSpeed = sprintSpeed;
        else
            targetSpeed = moveSpeed;

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 6f);
    }

    void Move()
    {
        // Move based on input and player's forward direction
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 newVelocity = move * currentSpeed;
        newVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = newVelocity;
    }

    void RaycastGroundCheck()
    {
        isGrounded = Physics.Raycast(groundCheckOrigin.position, Vector3.down, groundCheckDistance, groundLayer);
        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
        Debug.Log(isGrounded);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsSprinting = true;
        }
        else if (context.canceled)
        {
            IsSprinting = false;
        }

        UpdateMovementSpeed();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }
    
    
    private void UpdateMovementSpeed()
    {
        float baseSpeed = moveSpeed;

        // Sprint only when moving forward
        if (IsSprinting && moveInput.y > 0.1f)
            baseSpeed = sprintSpeed;

        currentSpeed = baseSpeed;
    }
}
