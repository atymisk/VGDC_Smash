using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimatorManager : MonoBehaviour {
    
    public enum State
    {
        LEDGEGRABBING,
        STAGEGROUNDED,
        PLATFORMGROUNDED,
        RISING,
        FALLING,

        //transition states
        LEDGEDROPPING,
        //generalized states
        MIDAIR,

        //possibility states
        CANMOVE,
    }
    // put the animator state string names here for autocomplete and stuff
    private const string LedgeGrabbing  = "LedgeGrabbing";
    private const string StageGrounded  = "StageGrounded";
    private const string PlatformGrounded = "PlatformGrounded";
    private const string Rising = "Rising";
    private const string Falling = "Falling";

    //transition states
    private const string LedgeDropping = "LedgeDropping";


    private static Dictionary<State, string[]> stateStrings = new Dictionary<State, string[]> //mapping the enum to the string names in the Animator
    { 
        { State.LEDGEGRABBING,  new string[] { LedgeGrabbing, } },
        { State.STAGEGROUNDED,  new string[] { StageGrounded, } },
        { State.PLATFORMGROUNDED, new string[] { PlatformGrounded, } },
        { State.RISING,         new string[] { Rising, } },
        { State.FALLING,        new string[] { Falling, } },
        //transition states
        { State.LEDGEDROPPING,  new string[] { LedgeDropping, } },
        //generalized states
        { State.MIDAIR,         new string[] { Rising, Falling, } },
        //possibility states
        { State.CANMOVE,        new string[] { Rising, Falling, StageGrounded, PlatformGrounded} },

    };
    private Animator theStateMachine;
	// Use this for initialization
	void Start () {
        theStateMachine = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool InState(State state)
    {
        foreach (string stateName in stateStrings[state])
        {
            if(theStateMachine.GetCurrentAnimatorStateInfo(0).IsName(stateName))
                return true;
        }
        return false;
    }
}
