using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * AnimatorManager: This is a service class that acts as an interface for state setting and checking. State logic should be done
 * 					outside of this class.
 * 
 * TODO : using triggers for now, remember if state machine bugs start to happen check the triggers.
 */
public class AnimatorManager : MonoBehaviour {
    
    public enum State
    {
        LEDGEGRABBING,
        STAGEGROUNDED,
        PLATFORMGROUNDED,
        RISING,
        FALLING,
        TUMBLING,
        REELING,

        //transition states
        LEDGEDROPPING,
        PLATFORMDROPPING,
        LAG,

        //generalized states
        MIDAIR,
        GROUNDED,
        GROUNDATTACK,
        ATTACKING,

        //possibility states
        CANMOVE,
    }

    // put the animator state string names here for autocomplete and stuff
    private const string LedgeGrabbing  = "LedgeGrabbing";
    private const string StageGrounded  = "StageGrounded";
    private const string PlatformGrounded = "PlatformGrounded";
    private const string Rising = "Rising";
    private const string Falling = "Falling";
    private const string Tumbling = "Tumbling";
    private const string Reeling = "Reeling";

    //attacks
    private const string GroundAttack = "GroundAttack";
    private const string Lag = "Lag";

    //transition states
    private const string LedgeDropping = "LedgeDropping";
    private const string PlatformDropping = "PlatformDropping";
    private const string ReelingTriggerClear = "ReelingTriggerClear";
	// Dictionary of states linked to their substates
    private static Dictionary<State, string[]> stateStrings = new Dictionary<State, string[]> //mapping the enum to the string names in the Animator
    { 
        { State.LEDGEGRABBING,  new string[] { LedgeGrabbing, } },
        { State.STAGEGROUNDED,  new string[] { StageGrounded, } },
        { State.PLATFORMGROUNDED, new string[] { PlatformGrounded, } },
        { State.RISING,         new string[] { Rising, } },
        { State.FALLING,        new string[] { Falling, } },
        { State.TUMBLING,       new string[] { Tumbling, } },
        { State.REELING,        new string[] { Reeling, ReelingTriggerClear} },
        //attacks
        { State.GROUNDATTACK,   new string[] { GroundAttack, } },
        //transition states
        { State.LEDGEDROPPING,  new string[] { LedgeDropping, } },
        { State.LAG,            new string[] { Lag,} },

        //generalized states
        { State.MIDAIR,         new string[] { Rising, Falling, LedgeDropping, PlatformDropping, Tumbling, Reeling, } },
        { State.GROUNDED,       new string[] { StageGrounded, PlatformGrounded, } },
        { State.ATTACKING,      new string[] { GroundAttack, } },     
        //possibility states
        { State.CANMOVE,        new string[] { Rising, Falling, StageGrounded, PlatformGrounded, Tumbling, } },

    };
    private Animator theStateMachine;

    private float timerTime = 0;
    private float timerMultiplier = 1;
	// Use this for initialization
	void Start () {
        theStateMachine = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (timerTime > 0)
        {
            timerTime += Time.deltaTime / timerMultiplier;
            theStateMachine.SetFloat("timer", timerTime);
            if (timerTime > 1)
                timerTime = 0;
        }
	}

	// InState: check if animator is in a given state. Can check on multiple substates.
    public bool InState(State state)
    {
		// check each substate
        foreach (string stateName in stateStrings[state])
        {
            if(theStateMachine.GetCurrentAnimatorStateInfo(0).IsName(stateName))
                return true;
        }
        return false;
    }

    public void startTimer(float timeMultiplier) //only used for Reeling exit
    {
        timerMultiplier = timeMultiplier; //set the new cutoff
        if (timerTime == 0) //if the timer isn't running, run the timer
            timerTime += Time.deltaTime / timerMultiplier;
    }
}
