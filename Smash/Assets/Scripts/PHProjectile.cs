using UnityEngine;
using System.Collections;

public class PHProjectile : Projectile {

    protected override void SetStartVelocity()
    {
        rigidbody.velocity = transform.InverseTransformDirection(new Vector3(10, 0, 0)); // set the velocity forward in our current facing
    }
}
