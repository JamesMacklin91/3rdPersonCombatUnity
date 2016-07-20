using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {

    Rigidbody rigidbody;
    Animator anim;
    CapsuleCollider capCol;
    Transform cam;

    [HideInInspector]
    public Transform camHolder;

    [SerializeField] float lockSpeed = 0.5f;
    [SerializeField] float normSpeed = 0.8f;
    float speed;

    [SerializeField] float turnSpeed = 5;

    Vector3 directionPos;
    Vector3 storeDir;

    [HideInInspector]
    public float horizontal;
    [HideInInspector]
    public float vertical;
    [HideInInspector]
    public bool rollInput;

    public bool lockTarget;
    int curTarget;
    bool changeTarget;

    float targetTurnAmount;
    float curTurnAmount;
    public bool canMove;
    public List<Transform> Enemies = new List<Transform>();

    public Transform camTarget;
    public float camTargetSpeed = 5;
    Vector3 targetPos;

    public bool attackInput;
    public bool blockInput;

    public bool inCombat;

	// Use this for initialization
	void Start () {
        rigidbody = GetComponent<Rigidbody>();
        cam = Camera.main.transform;
        camHolder = cam.parent.parent;
        capCol = GetComponent<CapsuleCollider>();
        SetupAnimator();

        GetComponent<PlayerAnimations>().enabled = true;
	
	}

    void FixedUpdate()
    {
        HandleInput();
        HandleCameraTarget();

        if(canMove && inCombat)
        {
            if(!lockTarget)
            {
                speed = normSpeed;
                HandleMovementNormal();
            }
            else
            {
                speed = lockSpeed;

                if (Enemies.Count > 0)
                {
                    HandleMovementLockOn();
                    HandleRotationOnLock();
                }
                else
                {
                    lockTarget = false;
                }
            }
        }
        else
        {
            speed = normSpeed;
            HandleMovementNormal();
        }
    }

	// Update is called once per frame
	void Update () {
        //Debug.Log(Enemies);
        camTarget.position = targetPos;
	}

    void SetupAnimator()
    {
        anim = GetComponent<Animator>();

        //use avatar from a child animator component if present
        //this is to enable easy swapping of the character model as a child node
        foreach(var childAnimator in GetComponentsInChildren<Animator>())
        {
            if(childAnimator != anim)
            {
                anim.avatar = childAnimator.avatar;
                Destroy(childAnimator);
                break;//if you find the first animator, stop searching
            }
        }
    }

    void HandleInput()
    {
       // Debug.Log(Input.GetAxis("Horizontal"));
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        rollInput = Input.GetButton("Roll");
        attackInput = Input.GetButton("Attack");
        blockInput = Input.GetButton("Block");
        storeDir = camHolder.right;//get the direction from the camera holder instead of the camera so that we can avoid differences in velocity based on where the camera is looking

        ChangeTargetsLogic();
    }

    void HandleCameraTarget()
    {
        
        if(!lockTarget)
        {
            targetPos = transform.position;
        }
        else
        {
            if(Enemies.Count>0)
            {
                Vector3 direction = Enemies[curTarget].position - transform.position;
                direction.y = 0;

                float distance = Vector3.Distance(transform.position, Enemies[curTarget].position);

                targetPos = direction.normalized * distance / 6;
                targetPos += transform.position;

                if(distance>20)
                {
                    lockTarget = false;
                }
            }
        }
    }

    void HandleMovementNormal()
    {
        canMove = anim.GetBool("CanMove");

        Vector3 dirForward = storeDir * horizontal;
        Vector3 dirSides = camHolder.forward * vertical;

        if(canMove)
        {
            rigidbody.AddForce((dirForward + dirSides).normalized * speed / Time.deltaTime);

            directionPos = transform.position + (storeDir * horizontal) + (cam.forward * vertical);

            Vector3 dir = directionPos - transform.position;
            dir.y = 0;

            float angle = Vector3.Angle(transform.forward, dir);

            float animValue = Mathf.Abs(horizontal) + Mathf.Abs(vertical);

            animValue = Mathf.Clamp01(animValue);

            anim.SetFloat("Forward", animValue);
            anim.SetBool("LockOn", false);

            if(horizontal !=0 || vertical !=0)
            {
                if(angle!=0 && canMove)
                {
                    rigidbody.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);
                }
            }
        }

    }

    void HandleMovementLockOn()
    {
        Transform camHolder = cam.parent.parent;
        Vector3 camForward = Vector3.Scale(camHolder.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = Vector3.Scale(camHolder.right, new Vector3(1, 0, 1)).normalized;
        Vector3 move = vertical * camForward + horizontal * cam.right;

        Vector3 moveForward = camForward * vertical;
        Vector3 moveSideways = camRight * horizontal;

        rigidbody.AddForce((moveForward + moveSideways).normalized * speed / Time.deltaTime);

        ConvertMoveInputAndPassItToAnimator(move);
    }

    void HandleRotationOnLock()
    {
        Vector3 lookPos = Enemies[curTarget].position;

        Vector3 lookDir = lookPos - transform.position;
        lookDir.y = 0;

        Quaternion rot = Quaternion.LookRotation(lookDir);
        rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, rot, Time.deltaTime * turnSpeed);
    }

    void ChangeTargetsLogic()
    {
        if(Input.GetButtonDown("Lockon"))
        {
            lockTarget = !lockTarget;
        }

        if(Input.GetButtonDown("ChangeLockOn"))
        {
            if(curTarget < Enemies.Count -1)
            {
                curTarget++;
            }
            else
            {
                curTarget = 0;
            }
        }
    }

    void ConvertMoveInputAndPassItToAnimator(Vector3 moveInput)
    {
        Vector3 localMove = transform.InverseTransformDirection(moveInput);
        float turnAmount = localMove.x;
        float forwardAmount = localMove.z;

        if(turnAmount !=0)
        {
            turnAmount *= 2;

            anim.SetBool("LockOn", true);
            anim.SetFloat("Forward", vertical);//, 0.0f, Time.deltaTime);
            anim.SetFloat("Sideways", horizontal);//, 0.0f, Time.deltaTime);

        }
    }
}
