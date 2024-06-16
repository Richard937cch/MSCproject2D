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
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rotateAction = scrollcontrols.FindActionMap (actionMapName).FindAction (rotate); 
        RegisterInputActions();
    }

    private void Update()
    {
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
        }
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
