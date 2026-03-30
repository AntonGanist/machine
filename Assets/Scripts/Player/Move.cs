using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] public CharacterController controller;
    [SerializeField] public float speed = 12f;
    [SerializeField] public float gravity = -9f;
    [SerializeField] public float jumpHeight = 3f;
    [SerializeField] public Transform groundCheck;
    [SerializeField] public float groundDistance;
    [SerializeField] public LayerMask groundMask;

    Vector3 velocity;
    private bool isGrounded;

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        controller.Move(velocity * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
    }

}
