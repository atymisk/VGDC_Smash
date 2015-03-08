using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerStateScript : MonoBehaviour {
    public float maxDamage = 500;
    public int numLives = 5;
    public float forceScalar = 10;
    public float verticalKnockback = 30;
    public Controls.Controller playerNumber;
    private float currentDamage;
    private PlayerController controller;                    //reference to playercontroller
    private Animator theStateMachine;                       //reference to state machine
    private CameraController cam;                           //reference to camera controller for screen shake effect
    private Text damageReadout;
	// Use this for initialization
	void Start () {
	    currentDamage = 0;
        controller = GetComponent<PlayerController>();
        cam = GameObject.FindGameObjectWithTag(Tags.Camera).GetComponent<CameraController>();
        damageReadout = GameObject.FindWithTag(Tags.UI).transform.FindChild("Health" + Controls.ControllerToString(playerNumber)).GetComponent<Text>();
        theStateMachine = GetComponent<Animator>();
        theStateMachine.SetInteger("numLives", numLives);
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
        SetDamage(0);
        //add stuff to reset position, count down numLives, etc.
        numLives = numLives - 1;
        theStateMachine.SetInteger("numLives", numLives);
        controller.ResetPosition();
        cam.ScreenShake(10, 3);
    }


    //use these methods so we can add in UI linking
    void AddDamage(float damage)
    {
        currentDamage += damage;
        updateUI();
    }

    void SetDamage(float damage)
    {
        currentDamage = damage;
        updateUI();
    }

    float GetCurrentDamage()
    {
        return currentDamage;
    }

    void updateUI()
    {
        Debug.Log("updating UI");
        damageReadout.text = currentDamage + "%";
    }
}
