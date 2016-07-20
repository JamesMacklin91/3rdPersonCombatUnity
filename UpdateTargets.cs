using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UpdateTargets : MonoBehaviour {

    Stats plStats;
    PlayerControl plControl;

	// Use this for initialization
	void Start () {
	
        plStats = GetComponentInParent<Stats>();
        plControl = GetComponentInParent<PlayerControl>();
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Stats>())
        {
            Stats stats = other.GetComponent<Stats>();

            if(stats.transform != plControl.transform)
            {
                if(!plControl.Enemies.Contains(stats.transform))
                {
                    plControl.Enemies.Add(stats.transform);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<Stats>())
        {
            if(plControl.Enemies.Contains(other.transform))
            {
                plControl.Enemies.Remove(other.transform);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
