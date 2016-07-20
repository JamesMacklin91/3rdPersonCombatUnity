using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerAnimations : MonoBehaviour {

    PlayerControl plControl;

    public List<AnimationsBase> AnimationsList = new List<AnimationsBase>();

    public float rollSpeed = 0.1f;

    float horizontal;
    float vertical;
    bool rollInput;

    public bool rolling;
    bool hasRolled;

    Vector3 directionPos;
    Vector3 storeDir;
    Transform camHolder; 
    Rigidbody rigidbody;
    Animator anim;

    public bool hasDirection;

    Vector3 dirForward;
    Vector3 dirSides;
    Vector3 dir;
    Stats plStats;


	// Use this for initialization
	void Start () {

        plStats = GetComponent<Stats>();
        plControl = GetComponent<PlayerControl>();
        camHolder = plControl.camHolder;
        rigidbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
	}

    void FixedUpdate()
    {
        if (plControl.inCombat)
        {
            this.rollInput = plControl.rollInput;
            this.horizontal = plControl.horizontal;
            this.vertical = plControl.vertical;
            storeDir = camHolder.right;

            if (rollInput && plStats.stamina > 40)
            {
                if (!rolling && (Mathf.Abs(vertical) > 0 || Mathf.Abs(horizontal) > 0))
                {
                    plControl.canMove = false;
                    rolling = true;
                }
            }

            if (rolling)
            {
                if (!hasDirection)
                {
                    dirForward = storeDir * horizontal;
                    dirSides = camHolder.forward * vertical;

                    directionPos = transform.position + (storeDir * horizontal) + (camHolder.forward * vertical);
                    dir = directionPos - transform.position;
                    dir.y = 0;

                    anim.SetTrigger("Roll");
                    hasDirection = true;
                    plStats.stamina -= 40;
                }
                Debug.Log((dirForward + dirSides).normalized * rollSpeed / Time.deltaTime);
                rigidbody.AddForce((dirForward + dirSides).normalized * rollSpeed / Time.deltaTime);

                float angle = Vector3.Angle(transform.forward, dir);

                if (angle != 0)
                {
                    rigidbody.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 15 * Time.deltaTime);
                }
            }
        }
    }

	// Update is called once per frame
	void Update () {
        //Debug.Log(dir);
	}
}
[System.Serializable]
public class AnimationsBase
{
    public string playerAnimName;
    public string enemAnimName;
    public float offset;
}