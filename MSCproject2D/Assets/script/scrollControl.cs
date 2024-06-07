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

    private void Awake()
    {
        rotateAction = scrollcontrols.FindActionMap (actionMapName).FindAction (rotate); 
        RegisterInputActions();
    }

    private void Update()
    {
        if (rotateValue > 0) {
            transform.Rotate(Vector3.forward * scrollSpeed , Space.Self);
        }
        if (rotateValue < 0) {
            transform.Rotate(Vector3.back * scrollSpeed , Space.Self);
        }
    }

    private void RegisterInputActions()
    {
        rotateAction.performed += context => rotateValue = context.ReadValue<float>(); 
        rotateAction.canceled += context => rotateValue = 0.0f;
    }

    private void OnEnable()
    {
        scrollcontrols.Enable();
    }
    private void Disable()
    {
        scrollcontrols.Disable();
    }
}
