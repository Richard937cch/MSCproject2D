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
    private bool rotateClockwise = true;

    public float changeDirectionInterval = 2f;

    public bool key = false;

    public MapSettings mapSettings;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        
        width = this.GetComponent<Gridgen>().width;
        rotateAction = scrollcontrols.FindActionMap (actionMapName).FindAction (rotate); 
        leftAction  = scrollcontrols.FindActionMap (actionMapName).FindAction (left);
        rightAction = scrollcontrols.FindActionMap (actionMapName).FindAction(right);
        
        RegisterInputActions();
        
        
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerRJ = player.GetComponent<RollJump>();
        MenuParameter(); //receive setup from main manu
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
        

    private void Update()
    {
        /*if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerRJ = player.GetComponent<RollJump>();
        }*/
        if (mapRotation == MapRotation.ScrollTransform)
        {
            //ScrollWheelTransform();
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
                //leftValue = false;
                
            }
            if (rotateValue < 0 || rightValue > 0) 
            {
                transform.Rotate(Vector3.back * scrollSpeed, Space.Self);
                //rightValue = false;
                
            }
            print(leftValue+"+"+rightValue);
        }
        

    }

    void ScrollWheelForce()
    {
        
        if ((rotateValue > 0 || leftValue > 0) && rb.angularVelocity.z < 0.3)
        {
            rb.AddTorque(Vector3.forward * torqueAmount, ForceMode.Acceleration);
        }
        else if ((rotateValue < 0 || rightValue > 0) && rb.angularVelocity.z > -0.3)
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
        
    }

    void RandomRotationMode()
    {
        //print(rb.angularVelocity);
        
        if (rotateClockwise)
        {
            if (rb.angularVelocity.z < 0.1)
            {
                rb.AddTorque(Vector3.forward * torqueAmount * Time.fixedDeltaTime, ForceMode.Acceleration);
            }
            
        }
        else
        {
            if (rb.angularVelocity.z > -0.1)
            {
                rb.AddTorque(Vector3.back * torqueAmount * Time.fixedDeltaTime, ForceMode.Acceleration);
            }
        }
        

    }

    IEnumerator ChangeDirectionRoutine()
    {
        while (mapRotation == MapRotation.RandomRotation)
        {
            //changeDirectionInterval = Random.Range(1, changeDirectionInterval);
            yield return new WaitForSeconds(changeDirectionInterval);
            rotateClockwise = Random.value > 0.5f;
            //print(rotateClockwise);
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
    private void Disable()
    {
        rotateAction.Disable();
        leftAction.Disable();
        rightAction.Disable();
    }
}
