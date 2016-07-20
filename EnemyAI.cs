using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour {

    public bool dead;

    [SerializeField]
    float deadZone = 5f;
    [SerializeField]
    float turnSpeed = 3f;

    NavMeshAgent agent;
    Animator anim;

    public Transform targetEnemy;
    public Transform targetDestination;
    public Vector3 targetDesPos;
    Vector3 lookPos;
    public bool AttackEnemy;
    public float distanceThreshold = 3;

    float attackTimer;
    bool interrupted;

    public GameObject counterAttackIndicator;

	// Use this for initialization
	void Start () {

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        SetupAnimator();

        agent.updateRotation = false;
        agent.speed = 1;
        agent.stoppingDistance = distanceThreshold - .5f;
        

        deadZone *= Mathf.Deg2Rad;

        targetDesPos = transform.position;
	}

    void SetupAnimator()
    {
        anim = GetComponent<Animator>();

        //use avatar from a child animator component if present
        //this is to enable easy swapping of the character model as a child node
        foreach (var childAnimator in GetComponentsInChildren<Animator>())
        {
            if (childAnimator != anim)
            {
                anim.avatar = childAnimator.avatar;
                Destroy(childAnimator);
                break;//if you find the first animator, stop searching
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
        if(!dead)
        {
            FindDestination();
            LookAtTheTarget();
            NavigationAI();
            HandleMovementAnimation();
            StateSwitch();
        }
	}

    void FindDestination()
    {
        targetDesPos = targetEnemy.position;

        float distanceFromTarget = Vector3.Distance(transform.position, targetDesPos);

        agent.SetDestination(targetDesPos);

        if(distanceFromTarget<distanceThreshold)
        {
            attackTimer += Time.deltaTime;
            if(attackTimer>1 &&!interrupted)
            {
                //targetEnemy.GetComponent<PlayerControl>().EnableCounterAttack(true);
                counterAttackIndicator.SetActive(true);

                if(attackTimer>2)
                {
                    //targetEnemy.GetComponent<PlayerControl>().EnableCounterAttack(false);
                    counterAttackIndicator.SetActive(false);
                    anim.SetBool("CurrentlyAttacking", true);
                    anim.SetInteger("AttackType", 1);
                    attackTimer = 0;
                }
            }
        }
        else
        {
            counterAttackIndicator.SetActive(false);
        }
    }

    void LookAtTheTarget()
    {
        lookPos = targetEnemy.position;

        Vector3 lookDir = lookPos - transform.position;
        lookDir.y = 0;

        Quaternion rot = Quaternion.LookRotation(lookDir);

        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * turnSpeed);
    }

    void NavigationAI()
    {
       // agent.destination = targetDesPos;
       float angle;

        angle = FindAngle(transform.forward, agent.desiredVelocity, transform.up);

        if(Mathf.Abs(angle)<deadZone)
        {
            transform.LookAt(transform.position + agent.desiredVelocity);
            angle = 0;
        }
    }

   

    void HandleMovementAnimation()
    {
        float distanceFromTarget = Vector3.Distance(transform.position, targetDesPos);

        if (distanceFromTarget > distanceThreshold)
        {
            Vector3 desiredVelocityOnLocalCoordinates = transform.InverseTransformDirection(agent.desiredVelocity);
            Debug.Log(agent.desiredVelocity);
            anim.SetFloat("Forward", desiredVelocityOnLocalCoordinates.z, 0.1f, Time.deltaTime);
            anim.SetFloat("Sideways", desiredVelocityOnLocalCoordinates.x, 0.1f, Time.deltaTime);
        }
        else
        {
            anim.SetFloat("Forward", 0);
            anim.SetFloat("Sideways", 0);
        }
    }

    void StateSwitch()
    {

    }

    #region Utilities
    float FindAngle(Vector3 fromVector, Vector3 toVector, Vector3 upVector)
    {
        if (toVector == Vector3.zero)
        
            return 0f;
        
        float angle = Vector3.Angle(fromVector, toVector);
        Vector3 normal = Vector3.Cross(fromVector, toVector);
        angle *= Mathf.Sign(Vector3.Dot(normal, upVector));
        angle *= Mathf.Deg2Rad;
        return angle;
    }
}
    #endregion