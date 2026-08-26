using UnityEngine;
using NaughtyAttributes;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField, BoxGroup("References"), Required] private Rigidbody rb;
    [SerializeField, BoxGroup("References"), Required] private InputManager input;
    private Camera _camera;
   
    
    [SerializeField, BoxGroup("Settings"), Range(0,10)] private float walkSpeed = 5f;
    [SerializeField, BoxGroup("Settings"), Range(0,10)] private float runSpeed = 5f;
    [SerializeField, BoxGroup("Settings"), Range(0,10)] private float rotationSpeed = 10f;
    [SerializeField, BoxGroup("Settings")] private LayerMask groundMask;
    
    private bool _isGamepadAiming;
    private Vector2 _lastMousePosition;
    private float _speed;
    
    private void Start()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
        if (!input) input = FindAnyObjectByType<InputManager>();
        _lastMousePosition = input.AimPointInput();
        _camera = Camera.main;
    }

    private void FixedUpdate()
    {
        Rotation();
        
        if (input.SprintInput())
        {
            MovementFast();
        }
        else
        {
            Movement();
        }
    }

    private void Movement()
    {
        Vector3 diraction = new Vector3(input.MoveInput().x , 0 , input.MoveInput().y);
        
        diraction = diraction.normalized;
        
        rb.MovePosition(transform.position + diraction * (walkSpeed * Time.fixedDeltaTime));
    }
    
    private void MovementFast()
    {
        float forwardMove = 0;
        
        if (input.MoveInput().y >= 0.1f)
        {
            forwardMove = 1;
        }
        else
        {
            forwardMove = 0;
        }
        
        // rb.MovePosition(transform.position + transform.forward * (forwardMove * ( runSpeed * Time.fixedDeltaTime)));
        rb.MovePosition(transform.position + transform.forward * ( runSpeed * Time.fixedDeltaTime));
    }
    

    private void Rotation()
    {
        Vector2 aim = input.AimDirectionInput();
        Vector2 mousePosition = input.AimPointInput();

        // -------------------------
        // GAMEPAD
        // -------------------------
        
        if (aim.sqrMagnitude > 0.1f)
        {
            _isGamepadAiming = true;
            
            Vector3 dir = new Vector3(aim.x, 0, aim.y);
            
            Quaternion targetRotation = Quaternion.LookRotation(dir);

            Quaternion newRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
                    
            rb.MoveRotation(newRotation);
            
            return;
        }

        // -------------------------
        // MOUSE
        // -------------------------
        
        if (mousePosition != _lastMousePosition)
        {
            _isGamepadAiming = false;
            _lastMousePosition = mousePosition;
        }

        if (_isGamepadAiming == false)
        {
            Ray ray = _camera.ScreenPointToRay(mousePosition);
        
            if (Physics.Raycast(ray, out RaycastHit hitInfo,Mathf.Infinity ,groundMask))
            {
                Vector3 dir = hitInfo.point - transform.position;
                
                dir.y = 0;

                if (dir.sqrMagnitude > 0.1)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(dir);

                    Quaternion newRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
                    
                    rb.MoveRotation(newRotation);
                }
            }
        }
    }
}