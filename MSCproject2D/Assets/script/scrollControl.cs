using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class scrollControl : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset scrollcontrols;

    [Header("Action Map Name References")] 
    [SerializeField] private string actionMapName = "scroll";
    [Header("Action Name References")]
    [SerializeField] private string rotate = "mouseScroll"; 

    private InputAction rotateAction; 
    public float rotateValue { get; private set; }

    public static scrollControl Instance { get; private set; }

    public float scrollSpeed = 0.5f;

    public float torqueAmount = 10f;

    public float dampingFactor = 0.95f; // Factor to gradually reduce angular velocity
    private Rigidbody rb;

    public MapRotation mapRotation = MapRotation.None;

    private GameObject player;
    private RollJump playerRJ;
    private int width;

    private Vector3 rotationAS;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        
        width = this.GetComponent<Gridgen>().width;
        rotateAction = scrollcontrols.FindActionMap (actionMapName).FindAction (rotate); 
        RegisterInputActions();
        
        
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerRJ = player.GetComponent<RollJump>();
        if (player == null || playerRJ == null)
        {
            Debug.LogError("Player not found. Make sure the player has the 'Player' tag.");
            return;
        }
    }

    private void Update()
    {
        /*if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerRJ = player.GetComponent<RollJump>();
        }*/
        if (mapRotation == MapRotation.ScrollTransform)
        {
            ScrollWheelTransform();
        }
        /*
        if (rotateValue > 0) {
            transform.Rotate(Vector3.forward * scrollSpeed , Space.Self);
        }
        if (rotateValue < 0) {
            transform.Rotate(Vector3.back * scrollSpeed , Space.Self);
        }*/
    }

    private void FixedUpdate()
    {
        switch (mapRotation) 
        {
            case (MapRotation.None):
                break;
            case (MapRotation.ScrollTransform):
                //ScrollWheelTransform();
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
            default:
                break;
        }
    }

    void ScrollWheelTransform()
    {
        if (rotateValue > 0  && Time.timeScale != 0) 
        {
            transform.Rotate(Vector3.forward * scrollSpeed , Space.Self);
            
        }
        if (rotateValue < 0 && Time.timeScale != 0) 
        {
            transform.Rotate(Vector3.back * scrollSpeed , Space.Self);
            
        }

    }

    void ScrollWheelForce()
    {
        if (rotateValue > 0)
        {
            rb.AddTorque(Vector3.forward * torqueAmount, ForceMode.Acceleration);
        }
        else if (rotateValue < 0)
        {
            rb.AddTorque(Vector3.back * torqueAmount, ForceMode.Acceleration);
        }
        else
        {
            //rb.angularVelocity = Vector3.zero; // Stop rotation when the scroll wheel is not used
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, dampingFactor * Time.fixedDeltaTime);
        }
    }

    void GravityMode()
    {
        if (playerRJ.isFalling == false && player.transform.position.x < -0.5f)
        {
            rb.AddTorque(Vector3.forward * torqueAmount, ForceMode.Acceleration);
            //rb.angularVelocity = Vector3.forward * torqueAmount*(player.transform.position.x/width*-1);
            ScrollWheelForce();
        }
        else if (playerRJ.isFalling == false && player.transform.position.x > 0.5f)
        {
            rb.AddTorque(Vector3.back * torqueAmount, ForceMode.Acceleration);
            //rb.angularVelocity = Vector3.back * torqueAmount*(player.transform.position.x/width);
            ScrollWheelForce();
        }
        else
        {
            rb.angularVelocity = Vector3.zero; // Stop rotation when the scroll wheel is not used
            ScrollWheelForce();
        }
    }

    void AutoRotationMode()
    {
        if (rb.angularVelocity.magnitude < 0.1)
        {
            rb.AddTorque(Vector3.forward * torqueAmount, ForceMode.Acceleration);
        }
        
        //print(rb.angularVelocity);
    }

    private void RegisterInputActions()
    {
        rotateAction.performed += context => rotateValue = context.ReadValue<float>(); 
        rotateAction.canceled += context => rotateValue = 0.0f;
    }

    private void OnEnable()
    {
        rotateAction.Enable();
    }
    private void Disable()
    {
        rotateAction.Disable();
    }
}
