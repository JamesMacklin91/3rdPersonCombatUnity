using UnityEngine;
using System.Collections;

public class Stats : MonoBehaviour {

    public float health = 100;
    public float maxHealth = 100;
    public float stamina = 100;
    public float maxStamina = 100;
    public float staminaRegenRate = 5;
    public float healthRegenRate = 0;
    public bool dead;
    public bool blocking;
    RagdollManager rM;
    EnemyAI aI;

	// Use this for initialization
	void Start () {
        if (GetComponent<EnemyAI>())
        {
            aI = GetComponent<EnemyAI>();
        }
        health = maxHealth;
        stamina = maxStamina;
        rM = GetComponent<RagdollManager>();
	}
	
	// Update is called once per frame
	void Update () {

        HandleRegeneration();
        if(health<=0)
        {
            rM.RagdollCharacter();
            aI.enabled = false;
            return;
        }
	}

    void HandleRegeneration()
    {
        if(health<maxHealth)
        {
            //health += Time.deltaTime * healthRegenRate;
        }
        else
        {
            health = maxHealth;
        }

        if(stamina<maxStamina)
        {
            stamina += Time.deltaTime * staminaRegenRate;
        }
        else
        {
            stamina = maxStamina;
        }

        if(health<=0)
        {
            health = 0;
        }

        if(stamina<0)
        {
            stamina = 0;
        }
    }
}
