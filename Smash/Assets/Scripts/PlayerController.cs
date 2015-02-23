using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	private enum PlayerState
	{
		MIDAIR,
		FALLING,
        PLATFORMGROUNDED,
        STAGEGROUNDED,
        RISING,
        LEDGEGRABBING,
	};

	public enum AccelType
	{
		FALL,
		JUMP,
		MOVE
	};

	public enum TimerType
	{
		JUMP,
		DELAY_JUMP
    };

	public int JUMP_DELAY_FRAMES = 2;		// delay before actually jumping
	public int MIN_JUMP_FRAMES = 5;			// short hop frames
	public int MID_JUMP_FRAMES = 10;		// mid jump frames
	public int MAX_JUMP_FRAMES = 24;		// max jump frames
	public int FALL_ACCEL_FRAMES = 8;		// how many frames it take to acclerate to falling speed

	public float groundAcceleration = 100f;		// ground horizontal acceleration
	public float groundDrag = 50f;				// ground drag
	public float minRunSpeed = 4f;				// walk speed
	public float midRunSpeed = 8f;				// jog speed
	public float maxRunSpeed = 16f;				// dash speed
	public float maneuverability = 300f;		// maneuverability
	public float jumpSpeed = 24f;				// jump speed
	public float baseJumpAccel = 900f;			// jump acceleration
	public float jumpDegradeFactor = 0.7f;		// how quickly jump degrades
	public int maxJumps = 2;					// maximum number of jumps
	public float midairAcceleration = 40f;		// midair horizontal acceleration
	public float midairDrag = 8f;				// midair horizontal drag
	public float midairSpeed = 16f;				// midair movement speed
	public float fallAccel = 120f;				// fall acceleration
	public float dropAccel = 260f;				// fall acceleration
	public float maxFallSpeed = 18f;			// max falling speed
	public float maxDropSpeed = 26f;			// max forced drop speed

	public Text HUDText;
	
	private HashSet<PlayerState> states;						// collection of the states the player is in
	private Dictionary<AccelType, Acceleration> accelerations;	// collection of accelerations ocurring on this player
	private Dictionary<TimerType, int> timers;				// collection of timers for recovery delays, smash charges, etc.
	private Dictionary<TimerType, int> timerMaxes;			// collection of timer maximum values
	private Controls controls;								// reference to input handler
    private PlayerStateScript stateScript;                  // reference to state script
	private int jumpCount;									// number of jumps
	private Vector2 prevPos;
	private Vector2 prevVel;
	private Vector2 currVel;
    private Collider thisCollider; //reference to this player's main physics collider
    private Transform platform; // may be null until initialized; DO NOT USE TO CHECK IF GROUNDED ON A PLATFORM!! THAT"S WHY WE HAVE THE ENUMS!!
    private List<Collider> platformColliders; // reference to every platform collider, so that they can be disabled when jumping up
	// INITIALIZE
	void Awake ()
	{
		// Initialize states. start in midair.
		states = new HashSet<PlayerState> {PlayerState.MIDAIR};

		// Get reference to control script
		// @TODO: move this outside of player game object and into a MatchManager or something
		controls = GetComponent<Controls>();
        stateScript = GetComponent<PlayerStateScript>();
        thisCollider = GetComponent<CapsuleCollider>();
		// Initialize accelerations.
		// @TODO: maybe move these into an entirely different acceleration handling class?
		accelerations = new Dictionary<AccelType, Acceleration>();
		accelerations.Add(AccelType.FALL, new Acceleration(null, maxFallSpeed * -1, null, fallAccel));
		accelerations.Add(AccelType.MOVE, new Acceleration(0f, null, null, midairAcceleration));
		accelerations.Add(AccelType.JUMP, new Acceleration(null, null, null, 0f));
        
		// Initialize timers. (not f2p timers, thank god)
		timers = new Dictionary<TimerType, int>();
		timers.Add (TimerType.JUMP, MAX_JUMP_FRAMES);
		timers.Add (TimerType.DELAY_JUMP, JUMP_DELAY_FRAMES);
		timerMaxes = new Dictionary<TimerType, int>();
		timerMaxes.Add (TimerType.JUMP, MAX_JUMP_FRAMES);
		timerMaxes.Add (TimerType.DELAY_JUMP, JUMP_DELAY_FRAMES);
        
        // Player starts midair, so allow one air jump
		jumpCount = 1;

		// Set previous player position and velocity
		currVel = Vector2.zero;
		UpdatePreviousVectors();

        // Set reference to all platform colliders
        platformColliders = new List<Collider>();
        GameObject[] platformObjects = GameObject.FindGameObjectsWithTag(Tags.Platform); //note: this actually gets one of the child objects, not the head platform object
        foreach (GameObject platformObject in platformObjects)
        {
            Transform platformTransform = platformObject.transform.parent; // get the reference to the head platform object

            platformColliders.Add(platformTransform.FindChild("stage_surface").GetComponent<MeshCollider>());
            platformColliders.Add(platformTransform.FindChild("stage_model").GetComponent<BoxCollider>());
        }
	}

	// FIXED UPDATE : update interval is exactly 1/60
	void FixedUpdate ()
	{
		// update controller values
		controls.DoUpdate();

		// update timers
		UpdateTimer(TimerType.DELAY_JUMP);
		UpdateTimer(TimerType.JUMP);

		// update position and velocity storage
		UpdateCurrentVectors();

		// handle commands
		DoMove ();
		DoJump ();
        DoPlatformDrop();
		DoFall ();
		DoDrop ();
       
		// apply accelerations
		foreach (AccelType accelType in accelerations.Keys)
			rigidbody.velocity = accelerations[accelType].ApplyToVector(rigidbody.velocity);

		// update previous position and velocity information
		UpdatePreviousVectors();
	}

	// COLLISIONS
	void OnTriggerEnter (Collider other)
	{
		switch (other.tag)
		{
            case Tags.Platform :
                StageCollideEnter();
                PlatformCollideEnter(other);
                break;
			case Tags.Stage :
				StageCollideEnter();
				break;
			case Tags.Boundary :
				BoundaryCollideEnter();
				break;
			case Tags.GrabEdge :
				GrabEdgeCollideEnter();
				break;
			case Tags.StopEdge :
				StopEdgeCollideEnter();
				break;
			default:
				break;
		}
    }
	void OnTriggerExit(Collider other)
	{
		switch (other.tag)
		{   
        case Tags.Platform : //non-platform-drop (normal) exiting
			StageCollideExit();
            PlatformCollideExit();
			break;
		case Tags.Stage :
			StageCollideExit();
			break;
		case Tags.Boundary :
			BoundaryCollideExit();
			break;
		case Tags.GrabEdge :
			GrabEdgeCollideExit();
			break;
		case Tags.StopEdge :
			StopEdgeCollideExit();
			break;
		default:
			break;
		}
	}
    void OnTriggerStay(Collider other)
    {
        switch (other.tag)
        {
            case Tags.GrabEdge:
                GrabEdgeCollideStay(other); //it's in OnTriggerStay instead of OnTriggerEnter or Exit so that it can trigger at the top of the parabola.
			break;
            default:
                break;
        }
    }

	// COLLISION HANDLERS
	void StageCollideEnter()
	{
		RemoveState(PlayerState.MIDAIR);			// player is no longer midair
		RemoveState(PlayerState.FALLING);			// player is no longer falling
        RemoveState(PlayerState.RISING);            // player is no longer rising
        AddState(PlayerState.STAGEGROUNDED);        // player is grounded on a stage
		jumpCount = 0;								// reset number of jumps player has made
		ResetAccel(AccelType.FALL);					// return fall acceleration to natural value
        
	}
    

	void StageCollideExit()
	{
		AddState(PlayerState.MIDAIR);				// player is midair
        RemoveState(PlayerState.STAGEGROUNDED);     // player is no longer grounded on a stage
	}

    void PlatformCollideEnter(Collider other)
    {
        platform = other.transform.parent; // update the reference to the platform's collider
        AddState(PlayerState.PLATFORMGROUNDED);
    }

    void PlatformCollideExit()
    {
        RemoveState(PlayerState.PLATFORMGROUNDED);
    }

	void BoundaryCollideEnter()
	{
        stateScript.Die();                          // kill the player
	}
	void BoundaryCollideExit(){}
	void GrabEdgeCollideEnter(){}
	void GrabEdgeCollideExit()
    {
        if (HasState(PlayerState.LEDGEGRABBING))
        {
            RemoveState(PlayerState.LEDGEGRABBING); 
            EnableAccel(AccelType.FALL, true);      // basic cleanup stuff
        }
    }
    void GrabEdgeCollideStay(Collider other)
    {
        if (HasState(PlayerState.FALLING) && !HasState(PlayerState.LEDGEGRABBING)) //start a ledge grab
        {
            RemoveState(PlayerState.FALLING);   // reset states
            RemoveState(PlayerState.MIDAIR);
            AddState(PlayerState.LEDGEGRABBING);
            transform.position = other.transform.position;	// move to grabbing position // TODO : use something that would make a smooth animation, not just this teleport
            if(jumpCount < 1)
                jumpCount = 1;								// let the player jump out of it
            ResetAccel(AccelType.FALL);					// return fall acceleration to natural value 
            EnableAccel(AccelType.FALL, false);         // disable falling while hanging
            SetVelocity(0f, 0f, 0f);                    //reset velocity and velocity tracker
        }
    }
	void StopEdgeCollideEnter(){}
	void StopEdgeCollideExit(){}

	// STATE CHECKERS
    bool CanMove() { return !HasState(PlayerState.LEDGEGRABBING); }
	bool CanJump () { return jumpCount < maxJumps; }
	bool CanFall () { return HasState(PlayerState.MIDAIR); }
	bool CanDrop () { return HasState(PlayerState.MIDAIR); }
    bool CanPlatformDrop() { return HasState(PlayerState.PLATFORMGROUNDED) && platform != null;  } // the not null check is not strictly necessary, since HasState should be accurate. Added a check here just in case
    bool CanLedgeDrop() { return HasState(PlayerState.LEDGEGRABBING); }
	// UTILITY FUNCTIONS
	bool TimerDone(TimerType timer) { return timers[timer] >= timerMaxes[timer]; }
	void SetTimer(TimerType timer, int frames) { timers[timer] = frames; }
	void SetTimerMax(TimerType timer, int frames) { timerMaxes[timer] = frames; }
	void SetAccel(AccelType accel, float? x, float? y, float? z, float mag) { accelerations[accel].Set(x, y, z, mag); }
	void ResetAccel(AccelType accel) { accelerations[accel].Reset(); }
    void EnableAccel(AccelType accel, bool enabled) { accelerations[accel].enabled = enabled; }
	void RemoveState(PlayerState state) { states.Remove(state); }
	void AddState(PlayerState state) { states.Add(state); }
	bool HasState(PlayerState state) { return states.Contains(state); }
	bool ChangedDirectionHorizontal(){ return currVel.x < 0f && prevVel.x * currVel.x <= 0f; }
	void UpdatePreviousVectors()
	{
		prevVel.x = currVel.x;
		prevVel.y = currVel.y;
		prevPos.x = transform.position.x;
		prevPos.y = transform.position.y;
	}
	void UpdateCurrentVectors()
	{
		currVel.x = (transform.position.x - prevPos.x) / Time.fixedDeltaTime;
		currVel.y = (transform.position.y - prevPos.y) / Time.fixedDeltaTime;
	}
    void UpdateTimer(TimerType timer)
	{
		if (timers[timer] < timerMaxes[timer])
			timers[timer]++;
	}
	void SetVelocity(float? x, float? y, float? z)
	{
		rigidbody.velocity = new Vector3((x.HasValue)? x.Value : rigidbody.velocity.x, (y.HasValue)? y.Value : rigidbody.velocity.y, (z.HasValue)? z.Value : rigidbody.velocity.z);
	}
	
	// COMMAND HANDLERS
	void DoMove ()
	{
		float magnitude = Mathf.Abs (controls.GetCommandMagnitude(Controls.Command.MOVE));	// check magnitude
		float sign = Mathf.Sign (controls.GetCommandMagnitude(Controls.Command.MOVE));		// get sign

		// While moving ...
        if (controls.GetCommand(Controls.Command.MOVE) && CanMove())
        {		// if move command is being issued ...
			if (HasState(PlayerState.MIDAIR))	// flat movement speed in midair
				SetAccel(AccelType.MOVE, midairSpeed * sign, null, null, midairAcceleration);
			else {
				if (magnitude > 0.98f) {				 // dashing speed
					SetAccel(AccelType.MOVE, maxRunSpeed * sign, null, null, groundAcceleration);
					SetVelocity (maxRunSpeed * sign, null, null);
				}else if (magnitude > 0.5f)			// jogging speed
					SetAccel(AccelType.MOVE, midRunSpeed * sign, null, null, groundAcceleration);
				else									// walking speed
					SetAccel(AccelType.MOVE, minRunSpeed * sign, null, null, groundAcceleration);
			}
		} else {					// if not moving ...
			if (HasState(PlayerState.MIDAIR))							// in midair apply midair drag
				SetAccel(AccelType.MOVE, 0f, null, null, midairDrag);
			else {
				SetAccel(AccelType.MOVE, 0f, null, null, groundDrag);	// on ground apply ground drag
			}
		}
	}
	void DoJump()
	{
		// While jump command is being issued ...
		if (controls.GetCommand(Controls.Command.JUMP) && CanJump()) {
			if (timers[TimerType.JUMP] == MID_JUMP_FRAMES)							// max jump duration
				SetTimerMax(TimerType.JUMP, MAX_JUMP_FRAMES);
			else if (timers[TimerType.JUMP] == MIN_JUMP_FRAMES)						// mid jump duration
				SetTimerMax(TimerType.JUMP, MID_JUMP_FRAMES);
		}

		// While jump timer is active ...
		if (!TimerDone(TimerType.JUMP)) {

			float scale = 1f - ((float) timers[TimerType.JUMP] / (float) MAX_JUMP_FRAMES);
			float powerScale = Mathf.Pow(scale, jumpDegradeFactor);
			SetAccel(AccelType.JUMP, null, jumpSpeed, null, baseJumpAccel * powerScale);
			HUDText.text = "Jump: " + (jumpSpeed * powerScale).ToString();
		} else
			ResetAccel(AccelType.JUMP);		// reset when jump is done

		// On jump start ...
		if (controls.ConsumeCommandStart(Controls.Command.JUMP) && CanJump()) {
			accelerations[AccelType.FALL].Reset();			// reset gravity when another jump starts. for repeated accelerated falls.
			SetVelocity(null, 0f, null);					// reset vertical velocity for new jump
			if (HasState(PlayerState.MIDAIR)) {				// give maneuverability burst while starting a jump in midair
				float horizSign = Mathf.Sign (controls.GetCommandMagnitude(Controls.Command.MOVE));
				horizSign = horizSign * Mathf.Sign (rigidbody.velocity.x);
				SetAccel(AccelType.MOVE, rigidbody.velocity.x * horizSign, null, null, maneuverability);
			}
			SetTimer(TimerType.JUMP, 0);					// start jump timer
			SetTimerMax(TimerType.JUMP, MIN_JUMP_FRAMES);	// set jump to short hop duration
		}

		// On jump command end ...
		if (controls.ConsumeCommandEnd(Controls.Command.JUMP))
			if (HasState(PlayerState.MIDAIR))
				jumpCount++;
    }
	void DoFall()
	{
		// the below returns true when we have STARTED to fall
        if (currVel.y < 0f && !HasState(PlayerState.FALLING) && !HasState(PlayerState.PLATFORMGROUNDED) && !HasState(PlayerState.LEDGEGRABBING))
        {
            AddState(PlayerState.FALLING);
            RemoveState(PlayerState.RISING);
            SetPlatformCollision(true); //enable collisions so we don't phase through
        }
        else if (currVel.y > 0f && !HasState(PlayerState.RISING) && !HasState(PlayerState.PLATFORMGROUNDED) && !HasState(PlayerState.LEDGEGRABBING)) //check to see if we're rising; only returns true when we've entered the rising state
        {
            RemoveState(PlayerState.FALLING);
            AddState(PlayerState.RISING);
            SetPlatformCollision(false); //disable collisions so we can phase through
        }

		if (CanFall()) {
            
		}
	}
    void DoDrop()
    {
        if (CanDrop() && controls.ConsumeCommandStart(Controls.Command.DUCK))
			SetAccel(AccelType.FALL, null, maxDropSpeed * -1f, null, dropAccel);
	}
	void DoSmash ()
	{
		print ("Smash");
	}
	void DoNeutralAttack ()
	{
		print ("Neutral ");
	}

    void DoPlatformDrop()
    {
        if (CanPlatformDrop() && controls.ConsumeCommandStart(Controls.Command.DUCK)) //normal platforms
        {
            AddState(PlayerState.FALLING);

            //platform dropping code

            StageCollideExit(); // collision will be disabled with the platform, so must call these here
            PlatformCollideExit(); // ^ same

            Physics.IgnoreCollision(thisCollider, platform.FindChild("stage_surface").GetComponent<MeshCollider>(), true);
            Physics.IgnoreCollision(thisCollider, platform.FindChild("stage_model").GetComponent<BoxCollider>(), true);
            

        }
        if (CanLedgeDrop() && controls.ConsumeCommandStart(Controls.Command.DUCK)) //dropping from a ledge grab
        {
            AddState(PlayerState.FALLING);
            AddState(PlayerState.MIDAIR);
            EnableAccel(AccelType.FALL, true);
            jumpCount = 0; // can't use ledge grabs to get more jumps
        }
    }

    void SetPlatformCollision(bool toggle)
    {
        foreach (Collider platformCollider in platformColliders)
        {
            Physics.IgnoreCollision(thisCollider, platformCollider, !toggle);
        }
    }
    //PUBLIC METHODS
    public void ResetPosition()
    {
        
        transform.position = new Vector3(0, 10, 0);	// reset player position
        jumpCount = 0;								// reset jump count
        ResetAccel(AccelType.FALL);					// return fall acceleration to natural value
        AddState(PlayerState.MIDAIR);				// player should be in midair and falling   
        SetVelocity(0f, 0f, 0f);                    //reset velocity and velocity tracker
        RemoveState(PlayerState.FALLING);           // reset all other previous states
        RemoveState(PlayerState.RISING);            
        RemoveState(PlayerState.PLATFORMGROUNDED);
        RemoveState(PlayerState.STAGEGROUNDED);
        RemoveState(PlayerState.LEDGEGRABBING);
        SetPlatformCollision(true);                 // reset platform collision
    }

    public void OnPlatformDropEnd(Collider other)
    {

        Transform otherTransform = other.transform.parent; // get the reference to the object's parent so we can get to all the colliders we need

        Physics.IgnoreCollision(thisCollider, otherTransform.FindChild("stage_surface").GetComponent<MeshCollider>(), false);
        Physics.IgnoreCollision(thisCollider, otherTransform.FindChild("stage_model").GetComponent<BoxCollider>(), false);

    }
}
