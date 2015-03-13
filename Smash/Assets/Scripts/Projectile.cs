using UnityEngine;
using System.Collections;

public abstract class Projectile : MonoBehaviour {

    //abstract class for all projectiles
    public int durationRemaining_frames; //how many frames will this projectile stay active
	// Use this for initialization
	void Start () {
        SetStartVelocity();
	}
    void FixedUpdate()
    {
        durationRemaining_frames -= 1;
        if (durationRemaining_frames <= 0)
            Destroy(this.gameObject);
        OnUpdate();
    }

    protected void OnUpdate() {} // overriding this is optional
    protected abstract void SetStartVelocity();
}
