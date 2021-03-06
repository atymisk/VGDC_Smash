﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStateScript : MonoBehaviour {

    public float maxDamage = 500;
    public int numLives = 4;
    public float forceScalar = 10;
    public float verticalKnockback = 30;
    public Controls.Controller playerNumber;
    public GameObject[] projectilePrefabs;                  //set in the inspector
    public Dictionary<string, GameObject> projectiles;
    private float currentDamage;
    private PlayerController controller;                    //reference to playercontroller
    private Animator theStateMachine;                       //reference to state machine
    private CameraController cam;                           //reference to camera controller for screen shake effect
    private UIController UI;                                   //reference to UI controller to update the UI readouts
    private Collider thisCollider;                          //reference to this player's main physics collider
    private Collider triggerCollider;                       //reference to our child collider for trigger stuff
	// Use this for initialization
	void Start () {
	    currentDamage = 0;
        controller = GetComponent<PlayerController>();
        cam = GameObject.FindGameObjectWithTag(Tags.Camera).GetComponent<CameraController>();
        UI = GameObject.FindWithTag(Tags.UI).transform.FindChild("Player" + Controls.ControllerToString(playerNumber)).GetComponent<UIController>();
        theStateMachine = GetComponent<Animator>();
        theStateMachine.SetInteger("numLives", numLives);
        thisCollider = GetComponent<CapsuleCollider>();
        triggerCollider = this.transform.FindChild("TriggerCollider").GetComponent<CapsuleCollider>();
        UI.UpdateUI(damage: currentDamage, lives: numLives);
	}

    public void TakeHit(float damage, Vector3 hitOrigin)
    {
        //damage stuff
        AddDamage(damage);
        CheckDeath();
        //boosted forces

        //reset velocity to zero before forces?
        Vector2 forceDirection = transform.position - (hitOrigin); // the new vector is to make sure they are bumped upward
        forceDirection = forceDirection / forceDirection.magnitude; //make unit vector
        GetComponent<Rigidbody>().velocity = (forceDirection * currentDamage * forceScalar / 100) + new Vector2(0,verticalKnockback); //the 100 is to cancel out the effects of writing percents as 100, 200, etc.
        // second vector is to bump them up
    }
    private void CheckDeath()
    {
        if (currentDamage > maxDamage)
        {
            Die();
        }
    }
    public void Die()
    {
        SetDamage(0);
        //add stuff to reset position, count down numLives, etc.
        numLives = numLives - 1;
        theStateMachine.SetInteger("numLives", numLives);
        controller.ResetPosition();
        cam.ScreenShake(10, 3);

        UI.UpdateUI(lives: numLives);
    }

    public void SpawnProjectile(int index) // method for the animator to call when an attack spawns a projectile
    {
        Collider otherCollider = (Instantiate(projectilePrefabs[index], this.transform.position, this.transform.rotation) as GameObject).GetComponent<Collider>();
        Debug.Log(otherCollider);
        Physics.IgnoreCollision(thisCollider, otherCollider, true);
        Physics.IgnoreCollision(triggerCollider, otherCollider, true);
    }

    //use these methods so we can add in UI linking
    void AddDamage(float damage)
    {
        currentDamage += damage;
        UI.UpdateUI(damage: currentDamage);
    }

    void SetDamage(float damage)
    {
        currentDamage = damage;
        UI.UpdateUI(damage: currentDamage);
    }

    public float GetCurrentDamage()
    {
        return currentDamage;
    }
}
