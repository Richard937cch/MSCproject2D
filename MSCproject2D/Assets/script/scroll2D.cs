using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class scrollControl2D : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset scrollcontrols;

    [Header("Action Map Name References")] 
    [SerializeField] private string actionMapName = "scroll";
    [Header("Action Name References")]
    [SerializeField] private string rotate = "mouseScroll"; 
    [SerializeField] private string left = "left"; 
    [SerializeField] private string right = "right";

    private InputAction rotateAction; 
    private InputAction leftAction;
    private InputAction rightAction;
    public float rotateValue { get; private set; }
    public float leftValue { get; private set;}
    public float rightValue { get; private set;}

    public static scrollControl2D Instance { get; private set; }

    public float scrollSpeed = 0.5f;

    public float torqueAmount = 10f;

    public float dampingFactor = 0.95f; // Factor to gradually reduce angular velocity
    private Rigidbody2D rb;
    
    public MapRotation mapRotation = MapRotation.None;

    private GameObject player;
    private RollJump playerRJ;
    private int width;

    private Vector3 rotationAS;
    private bool rotateClockwise = true;

    public float changeDirectionInterval = 2f;

    public bool key = false;

    public MapSettings mapSettings;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        width = this.GetComponent<Gridgen>().width;
        rotateAction = scrollcontrols.FindActionMap(actionMapName).FindAction(rotate); 
        leftAction  = scrollcontrols.FindActionMap(actionMapName).FindAction(left);
        rightAction = scrollcontrols.FindActionMap(actionMapName).FindAction(right);
        
        RegisterInputActions();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerRJ = player.GetComponent<RollJump>();
        MenuParameter(); //receive setup from main menu
        if (player == null || playerRJ == null)
        {
            Debug.LogError("Player not found. Make sure the player has the 'Player' tag.");
            return;
        }
        if (mapRotation == MapRotation.RandomRotation)
        {
            StartCoroutine(ChangeDirectionRoutine());
        }
    }
        
    /*private void Update()
    {
        if (mapRotation == MapRotation.ScrollTransform)
        {
            ScrollWheelTransform();
        }
    }*/

    private void FixedUpdate()
    {
        switch (mapRotation) 
        {
            case (MapRotation.None):
                break;
            case (MapRotation.ScrollTransform):
                ScrollWheelTransform();
                break;
            case (MapRotation.ScrollForce):
                ScrollWheelForce();
                break;
            case (MapRotation.Gravity):
                GravityMode();
                break;
            case (MapRotation.AutoRotation):
                AutoRotationMode();
                break;
            case (MapRotation.RandomRotation):
                RandomRotationMode();
                break;
            default:
                break;
        }
    }

    void ScrollWheelTransform()
    {
        if (Time.timeScale != 0)
        {
            if (rotateValue > 0 || leftValue > 0) 
            {
                transform.Rotate(Vector3.forward * scrollSpeed, Space.Self);
            }
            if (rotateValue < 0 || rightValue > 0) 
            {
                transform.Rotate(Vector3.back * scrollSpeed, Space.Self);
            }
        }
    }

    void ScrollWheelForce()
    {
        print(rb.angularDrag);
        if ((rotateValue > 0 || leftValue > 0))// && rb.angularVelocity < 0.3f)
        {
            rb.AddTorque(torqueAmount, ForceMode2D.Force);
        }
        else if ((rotateValue < 0 || rightValue > 0))// && rb.angularVelocity > -0.3f)
        {
            rb.AddTorque(-torqueAmount, ForceMode2D.Force);
        }
        else
        {
            // Smoothly reduce angular velocity to zero when the scroll wheel is not used
            rb.angularVelocity = Mathf.Lerp(rb.angularVelocity, 0f, dampingFactor * Time.fixedDeltaTime);
        }
    }

    void GravityMode()
    {
        if (playerRJ.isFalling == false && player.transform.position.x < -0.5f)
        {
            rb.AddTorque(torqueAmount, ForceMode2D.Force);
            ScrollWheelForce();
        }
        else if (playerRJ.isFalling == false && player.transform.position.x > 0.5f)
        {
            rb.AddTorque(-torqueAmount, ForceMode2D.Force);
            ScrollWheelForce();
        }
        else
        {
            rb.angularVelocity = 0f; // Stop rotation when the scroll wheel is not used
            ScrollWheelForce();
        }
    }

    void AutoRotationMode()
    {
        if (Mathf.Abs(rb.angularVelocity) < 0.1f)
        {
            rb.AddTorque(torqueAmount, ForceMode2D.Force);
        }
    }

    void RandomRotationMode()
    {
        if (rotateClockwise)
        {
            if (rb.angularVelocity < 0.1f)
            {
                rb.AddTorque(torqueAmount * Time.fixedDeltaTime, ForceMode2D.Force);
            }
        }
        else
        {
            if (rb.angularVelocity > -0.1f)
            {
                rb.AddTorque(-torqueAmount * Time.fixedDeltaTime, ForceMode2D.Force);
            }
        }
    }

    IEnumerator ChangeDirectionRoutine()
    {
        while (mapRotation == MapRotation.RandomRotation)
        {
            yield return new WaitForSeconds(changeDirectionInterval);
            rotateClockwise = Random.value > 0.5f;
        }
    }

    void MenuParameter() //if not using editor setup, use setup from main menu
    {
        if (!this.GetComponent<Gridgen>().editorValue)
        {
            torqueAmount = mapSettings.rotationSpeed;
        }
    }

    private void RegisterInputActions()
    {
        rotateAction.performed += context => rotateValue = context.ReadValue<float>(); 
        rotateAction.canceled += context => rotateValue = 0.0f;
        if (key)
        {
            leftAction.performed  += context => leftValue = context.ReadValue<float>();
            rightAction.performed += context => rightValue = context.ReadValue<float>();
            leftAction.canceled += context => leftValue = 0.0f;
            rightAction.canceled += context => rightValue = 0.0f;
        }
        else
        {
            leftValue = 0.0f;
            rightValue = 0.0f;
        }
    }

    private void OnEnable()
    {
        rotateAction.Enable();
        leftAction.Enable();
        rightAction.Enable();
    }

    private void OnDisable()
    {
        rotateAction.Disable();
        leftAction.Disable();
        rightAction.Disable();
    }
}
