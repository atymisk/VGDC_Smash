using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimatorManager : MonoBehaviour {
    
    public enum State
    {
        LEDGEGRABBING,
        STAGEGROUNDED,
        RISING,
        FALLING,
        MIDAIR,
    }
    
    private static Dictionary<State, string[]> stateStrings = new Dictionary<State, string[]> //mapping the enum to the string names in the Animator
    { 
        { State.LEDGEGRABBING,  new string[] { "LedgeGrabbing", } },
        { State.STAGEGROUNDED,  new string[] { "StageGrounded", } },
        { State.RISING,         new string[] { "Rising", } },
        { State.FALLING,        new string[] { "Falling", } },
        //generalized states
        { State.MIDAIR,         new string[] { "Rising", "Falling", } },

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
