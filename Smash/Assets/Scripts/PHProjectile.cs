using UnityEngine;
using System.Collections;

public class PHProjectile : Projectile {
    public float damage = 50;

    protected override void SetStartVelocity()
    {
        GetComponent<Rigidbody>().velocity = transform.InverseTransformDirection(new Vector3(10, 0, 0)); // set the velocity forward in our current facing
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == Tags.Player)
        {
            PlayerController otherController = other.transform.GetComponent<PlayerController>();
            if(!otherController.InState(AnimatorManager.State.UNTOUCHABLE))
            {
                Animator otherAnimator = other.transform.GetComponent<Animator>();
                otherAnimator.SetTrigger(Triggers.ReelingEnter);

                otherAnimator.SetBool(Triggers.PlatformGrounded, false); //the knockback will take them off of the stage
                otherAnimator.SetBool(Triggers.StageGrounded, false);

                other.transform.GetComponent<PlayerStateScript>().TakeHit(damage, transform.position);

                other.transform.GetComponent<AnimatorManager>().startTimer(15);
                //the other player's player collider will do the stuff needed if our attack priority is lower than theirs

                if (otherController.InState(AnimatorManager.State.STUNNED))
                    otherAnimator.SetTrigger(Triggers.StunExit);

            }
        }
        Destroy(this.gameObject); //no matter what we collided with, be destroyed
        // TODO: spawn particle effect object?
    }
}
