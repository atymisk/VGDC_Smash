using UnityEngine;
using System.Collections;

public class PlayerStateScript : MonoBehaviour {
    public float maxDamage = 500;
    public int numLives = 5;
    public float forceScalar = 10;
    private float currentDamage;
	//private Controls controls;								// reference to input handler // not needed anymore
    private PlayerController controller;                    //reference to playercontroller
	// Use this for initialization
	void Start () {
	    currentDamage = 0;
		// controls = GetComponent<Controls>(); // not needed anymore
        controller = GetComponent<PlayerController>();
	}

    public void TakeHit(float damage, Vector3 hitOrigin)
    {
        //damage stuff
        currentDamage +=damage;
        CheckDeath();
        //boosted forces

        //reset velocity to zero before forces?
        Vector2 forceDirection = transform.position - (hitOrigin - new Vector3(0f,0f,-0.5f)); // the new vector is to make sure they are bumped upward
        forceDirection = forceDirection / forceDirection.magnitude; //make unit vector
        rigidbody.velocity = forceDirection * currentDamage * forceScalar / 100; //the 100 is to cancel out the effects of writing percents as 100, 200, etc.
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
        controller.ResetPosition();

    }

    float GetCurrentDamage()
    {
        return currentDamage;
    }
}
