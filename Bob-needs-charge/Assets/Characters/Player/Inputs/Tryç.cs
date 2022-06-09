using UnityEngine;
using UnityEngine.InputSystem;

public class Tryç : MonoBehaviour
{

    public float moveSpeed = 1f;

    private Vector2 moveInput;
    private Rigidbody2D rb;
    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void OnJump (InputValue value)
    {
        Debug.Log("Jump");
    }
    private void OnJumpUp(InputValue value)
    {
        Debug.Log("JumpUp");
    }
    private void OnMove(InputValue value)
    {
        Debug.Log("Move");
        Debug.Log(value.Get<Vector2>());
        moveInput = value.Get<Vector2>();
    }
    private void OnShoot(InputValue value)
    {
        Debug.Log("Shoot");
    }


    private void Update()
    {
        Vector2 move = controls.Land.Move.ReadValue<Vector2>();
        controls.Land.Jump.ReadValue<float>();

    }

    void FixedUpdate()
    {
        rb.MovePosition(new Vector2(rb.position.x + moveInput.x * moveSpeed * Time.fixedDeltaTime, rb.position.y));
    }
}
