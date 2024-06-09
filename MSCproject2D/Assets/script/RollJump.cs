using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RollJump : MonoBehaviour {

	[Header("Input Action Asset")]
    [SerializeField] private InputActionAsset jumpControl;

    [Header("Action Map Name References")] 
    [SerializeField] private string actionMapName = "PlayerJump";
    [Header("Action Name References")]
    [SerializeField] private string rotate = "Jump"; 

	private InputAction jumpAction; 
    public bool jumpValue { get; private set; }

    public static RollJump Instance { get; private set; }


	public float rotationSpeed = 25.0F;
	public float jumpHeight = 5.0F;

	private float gravity= 9.81f;

	private bool isFalling = false;

	private Rigidbody2D rigid;

	private CharacterController characterController; 

	private Vector3 currentMovement;

	public float respawnDepth = -110;

	private void Awake()
    {
		characterController = GetComponent<CharacterController>();
		currentMovement = new Vector3(0, jumpHeight, 0);
        jumpAction = jumpControl.FindActionMap (actionMapName).FindAction (rotate); 
        jumpAction.performed += context => jumpValue = true; 
        jumpAction.canceled += context => jumpValue = false;
    }

	void Start () 
	{
		rigid = GetComponent<Rigidbody2D> ();
		isFalling = true;
	}
		
	void Update () {
        /*
		//Handles the rotation of the ball
		float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
		rotation *= Time.deltaTime;
		if (rigid != null) {
			//Apply rotation
			Vector3 vector = new Vector3(rotation,0.0F,0.0F);
			rigid.AddForce (vector, ForceMode.Impulse);
			if (UpKey() && !isFalling) {
				//Jump
				rigid.AddForce (Vector3.up * jumpHeight, ForceMode.Impulse);
			}
		}*/
		//if (jumpValue) {print("up");}
		
        if (UpKey() && !isFalling) 
		{
			//Jump
			rigid.AddForce (Vector3.up * jumpHeight, (ForceMode2D)ForceMode.Impulse);
			//rigid. Move (currentMovement*Time.deltaTime);
			//rigid.MovePosition(currentMovement*Time.deltaTime);
			
		}
		/*else
		{
			currentMovement.y -= gravity*Time.deltaTime;
			rigid.MovePosition(currentMovement*Time.deltaTime);
		}*/

		if (transform.position.y < respawnDepth)
		{
			transform.position = new Vector3(0, 40, -0.58f);
		}

	}

	public void OnCollisionStay2D (Collision2D col) 
	{ //Takes parameter of Collision so unity doesn't complain
		isFalling = false;
	}

	public void OnCollisionExit2D() 
	{
		isFalling = true;
	}

	private bool UpKey() 
	{
		return (Input.GetKeyDown (KeyCode.W) || Input.GetKeyDown (KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space));
		//return (Input.GetKeyDown(KeyCode.Keypad8));
	}

	private void OnEnable()
    {
        jumpAction.Enable();
    }
    private void Disable()
    {
        jumpAction.Disable();
    }

}