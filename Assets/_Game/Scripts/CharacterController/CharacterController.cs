using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    public Camera head;

    [Header("Player Settings")]
    [Range(0, 1)] 
    public float mouseSensitivaty = 0.5f;

    [Range(0, 1)] 
    public float mouseYSensitivaty = 0.9f;

    [Range(1, 5)] 
    public float speed = 3;
    public bool invertMouse = true;

    [Header("Player Locks")]
    public bool enableMovement = true;
    public bool enableMouse = true;


    // Internal
    private InputAction movement;
    private InputAction cursor;

    private Vector3 HeadRotation;
    private Rigidbody rb;
    void Start()
    {
        movement = InputSystem.actions.FindAction("Move");
        cursor = InputSystem.actions.FindAction("Look");

        HeadRotation = new Vector3(0, 0, 0);

        Cursor.visible = true;
        rb = GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        #region Rotation
        if(enableMouse)
        {
            Vector2 headMovement = cursor.ReadValue<Vector2>();

            headMovement *= mouseSensitivaty;
            headMovement.y *= mouseYSensitivaty;
            if(!invertMouse) headMovement.y *= -1;

            HeadRotation += new Vector3(headMovement.y, headMovement.x, 0);

            if(HeadRotation.x < -89) HeadRotation.x = -89;
            if(HeadRotation.x > 89) HeadRotation.x = 89;
            head.transform.rotation = Quaternion.Euler(HeadRotation);
        }
        #endregion

    }

    void FixedUpdate()
    {
        #region Movement
        if(enableMovement)
        {
            Vector2 moveAxis = movement.ReadValue<Vector2>();
            Vector3 moveDirection = Quaternion.Euler(new Vector3(0, HeadRotation.y, 0)) * new Vector3(moveAxis.x, 0, moveAxis.y);
            moveDirection *= speed;
            rb.linearVelocity = moveDirection;
        }
        #endregion
    }

    
}
