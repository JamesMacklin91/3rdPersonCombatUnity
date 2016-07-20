using UnityEngine;
using System.Collections;

public class AttackControl : MonoBehaviour {

    Animator anim;
    Rigidbody rigidbody;
    PlayerControl plControl;
    Stats plStats;

    bool attackInput;
    bool blockInput;
    public bool currentlyAttacking;
    bool blocking;
    bool decreaseStamina;


	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        plControl = GetComponent<PlayerControl>();
        plStats = GetComponent<Stats>();
    }

    void FixedUpdate()
    {
        if (plControl.inCombat)
        {
            UpdateInput();
            HandleAttacks();
            HandleBlock();
        }
    }

    void UpdateInput()
    {
        this.attackInput = plControl.attackInput;
        this.blockInput = plControl.blockInput;
    }

    void HandleAttacks()
    {
        if(currentlyAttacking)
        {
            anim.applyRootMotion = true;
        }
        else
        {
            anim.applyRootMotion = false;
        }

        if(attackInput && !blocking && !currentlyAttacking)
        {
            plControl.canMove = false;
            currentlyAttacking = true;

            if(!decreaseStamina)
            {
                plStats.stamina -= 30;
                decreaseStamina = true;
            }

            StartCoroutine("InitiateAttack");
        }
        else
        {
           // currentlyAttacking = false;
            decreaseStamina = false;
            //anim.SetBool("Attack", false);
        }
    }

    void HandleBlock()
    {
        if(blockInput)
        {
            plControl.canMove = false;
            blocking = true;
            anim.SetBool("Block", true);
        }
        else
        {
            blocking = false;
            anim.SetBool("Block", false);
        }
    }

    IEnumerator InitiateAttack()
    {
        yield return new WaitForEndOfFrame();
        anim.SetBool("Attack", true);
        int rndVal = Random.Range(0, 3);
        anim.SetInteger("AttackType", rndVal);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
