using UnityEngine;
using System.Collections;

public class PlayerStateScript : MonoBehaviour {
    public float maxDamage = 500;
    public int numLives = 5;
    public float forceScalar = 10;
    public float verticalKnockback = 30;
    private float currentDamage;
    private PlayerController controller;                    //reference to playercontroller
    private Animator theStateMachine;                       //reference to state machine

	// Use this for initialization
	void Start () {
	    currentDamage = 0;
        controller = GetComponent<PlayerController>();
        theStateMachine = GetComponent<Animator>();
        theStateMachine.SetInteger("numLives", numLives);
	}

    public void TakeHit(float damage, Vector3 hitOrigin)
    {
        //damage stuff
        currentDamage +=damage;
        CheckDeath();
        //boosted forces

        //reset velocity to zero before forces?
        Vector2 forceDirection = transform.position - (hitOrigin); // the new vector is to make sure they are bumped upward
        forceDirection = forceDirection / forceDirection.magnitude; //make unit vector
        rigidbody.velocity = (forceDirection * currentDamage * forceScalar / 100) + new Vector2(0,verticalKnockback); //the 100 is to cancel out the effects of writing percents as 100, 200, etc.
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
        currentDamage = 0;
        //add stuff to reset position, count down numLives, etc.
        numLives = numLives - 1;
        theStateMachine.SetInteger("numLives", numLives);
        controller.ResetPosition();

    }

    float GetCurrentDamage()
    {
        return currentDamage;
    }
}
