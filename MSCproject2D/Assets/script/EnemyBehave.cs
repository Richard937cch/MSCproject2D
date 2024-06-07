using UnityEngine;
using NPBehave;

public class EnemyAI : MonoBehaviour
{
    private Blackboard blackboard;
    private Root behaviorTree;

    public float duration = 10f; // Time interval between spawns
    private float timer;

    private float timer2;
    private float jumptime = 2f;

    public float jumpHeight = 8.0F;

	private bool isFalling = false;

	private Rigidbody2D rigid;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D> ();
        timer = duration;
        timer2 = jumptime;
        // create our behaviour tree and get it's blackboard
        behaviorTree = CreateBehaviourTree();
        blackboard = behaviorTree.Blackboard;

        // attach the debugger component if executed in editor (helps to debug in the inspector) 
#if UNITY_EDITOR
        Debugger debugger = (Debugger)this.gameObject.AddComponent(typeof(Debugger));
        debugger.BehaviorTree = behaviorTree;
#endif

        // start the behaviour tree
        behaviorTree.Start();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        timer2 -= Time.deltaTime;
        
        if (timer <= 0.0f)
        {
            behaviorTree.Stop();
            Destroy(gameObject);
            timer = duration; // Reset the timer
        }

        if (timer2 <= 0.0f)
        {
           if ( !isFalling) {
				//Jump
				rigid.AddForce (Vector3.up * jumpHeight, (ForceMode2D)ForceMode.Impulse);
			}
           timer2 = jumptime; // Reset the timer
        }

        

    }

    private Root CreateBehaviourTree()
    {
        // we always need a root node
        return new Root(

            // kick up our service to update the "playerDistance" and "playerLocalPos" Blackboard values every 125 milliseconds
            new Service(0.125f, UpdatePlayerDistance,

                new Selector(

                    // check the 'playerDistance' blackboard value.
                    // When the condition changes, we want to immediately jump in or out of this path, thus we use IMMEDIATE_RESTART
                    new BlackboardCondition("playerDistance", Operator.IS_SMALLER, 10.5f, Stops.IMMEDIATE_RESTART,

                        // the player is in our range of 7.5f
                        new Sequence(

                            // set color to 'red'
                            new Action(() => SetColor(Color.red)) { Label = "Change to Red" },

                            // go towards player until playerDistance is greater than 7.5 ( in that case, _shouldCancel will get true )
                            new Action((bool _shouldCancel) =>
                            {
                                if (!_shouldCancel)
                                {
                                    MoveTowards(blackboard.Get<Vector3>("playerLocalPos"));
                                    return Action.Result.PROGRESS;
                                }
                                else
                                {
                                    return Action.Result.FAILED;
                                }
                            }) { Label = "Follow" }
                        )
                    ),

                    // park until playerDistance does change
                    new Sequence(
                        new Action(() => SetColor(Color.grey)) { Label = "Change to Gray" },
                        new WaitUntilStopped()
                    )
                )
            )
        );
    }

    private void UpdatePlayerDistance()
    {
        Vector3 playerLocalPos = this.transform.InverseTransformPoint(GameObject.FindGameObjectWithTag("Player").transform.position);
        behaviorTree.Blackboard["playerLocalPos"] = playerLocalPos;
        behaviorTree.Blackboard["playerDistance"] = playerLocalPos.magnitude;
    }

    private void MoveTowards(Vector3 localPosition)
    {
        transform.localPosition += localPosition * 0.5f * Time.deltaTime;
    }

    private void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().material.SetColor("_Color", color);
    }

    public void OnCollisionStay (Collision col) { //Takes parameter of Collision so unity doesn't complain
		isFalling = false;
	}

	public void OnCollisionExit() {
		isFalling = true;
	}
}
