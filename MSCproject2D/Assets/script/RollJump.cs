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
	[SerializeField] private string pause = "Pause"; 

	private InputAction jumpAction; 
	private InputAction pauseAction;
    public bool jumpValue { get; private set; }
	public bool pauseValue { get; private set;}

    public static RollJump Instance { get; private set; }


	public float rotationSpeed = 25.0F;
	public float jumpHeight = 5.0F;

	private float gravity= 9.81f;

	public bool isFalling = false;
	public bool isInSlime = false;

	public float slimeSpeed = 0.7f;

	private Rigidbody2D rigid;

	private CharacterController characterController; 

	private Vector3 currentMovement;

	private GM gm;

	private void Awake()
    {
		gm = FindFirstObjectByType<GM>();
		characterController = GetComponent<CharacterController>();
		currentMovement = new Vector3(0, jumpHeight, 0);
        jumpAction = jumpControl.FindActionMap (actionMapName).FindAction (rotate); 
        jumpAction.performed += context => jumpValue = true; 
        jumpAction.canceled += context => jumpValue = false;

		pauseAction = jumpControl.FindActionMap (actionMapName).FindAction (pause);
		pauseAction.performed += context => pauseValue = true;
		pauseAction.canceled += context => pauseValue = false;
    }

	void Start () 
	{
		rigid = GetComponent<Rigidbody2D> ();
		isFalling = true;
	}
		
	void FixedUpdate () {
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
		//if (UpKey()) {print("up");}
		
        if (jumpValue && !isFalling) 
		{
			//Jump
			rigid.AddForce (Vector3.up * jumpHeight, (ForceMode2D)ForceMode.Impulse);
			jumpValue = false;
			//rigid. Move (currentMovement*Time.deltaTime);
			//rigid.MovePosition(currentMovement*Time.deltaTime);
			
		}
		if (jumpValue && isInSlime) 
		{
			//Jump
			rigid.AddForce (Vector3.up * jumpHeight , (ForceMode2D)ForceMode.Impulse);
			jumpValue = false;
			//rigid. Move (currentMovement*Time.deltaTime);
			//rigid.MovePosition(currentMovement*Time.deltaTime);
			
		}
		if (isInSlime)
		{
			ModifySpeed(slimeSpeed);
		}
		if (pauseValue)
		{
			gm.Pause();
			pauseValue = false;
		}
		
		/*else
		{
			currentMovement.y -= gravity*Time.deltaTime;
			rigid.MovePosition(currentMovement*Time.deltaTime);
		}*/

		

	}

	public void OnCollisionStay2D (Collision2D col) 
	{ //Takes parameter of Collision so unity doesn't complain
		isFalling = false;
	}

	public void OnCollisionExit2D() 
	{
		isFalling = true;
	}

	void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Slime"))
        {
            isInSlime = true;
        }
    }

	void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Slime"))
        {
            isInSlime = false;
        }
    }


	public void ModifySpeed(float factor)
    {
        rigid.velocity *= factor;
    }

	private bool UpKey() 
	{
		return (Input.GetKeyDown (KeyCode.W) || Input.GetKeyDown (KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space));
		//return (Input.GetKeyDown(KeyCode.Keypad8));
	}

	private void OnEnable()
    {
        jumpAction.Enable();
		pauseAction.Enable();
    }
    private void Disable()
    {
        jumpAction.Disable();
		pauseAction.Disable();
    }

}