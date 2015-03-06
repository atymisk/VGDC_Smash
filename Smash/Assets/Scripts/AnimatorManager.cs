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
        GROUNDED,
        RISING,
        FALLING,
        TUMBLING,
        REELING,
        MOSTLYDEAD,
        ALLDEAD,
        RESPAWNING,
        DOWNED,
        LANDING,
        BLOCKING,
        STUNNED,
        GROUNDDASHING,
        AIRDASHING,

        NEUTRALATTACK,
        FORWARDTILTATTACK,
        DOWNTILTATTACK,
        UPTILTATTACK,

        NEUTRALSMASH,
        FORWARDSMASH,
        DOWNSMASH,
        UPSMASH,

        //transition states
        LEDGEDROPPING,
        LAG,
        UNCONSCIOUS,
        RECOVERING,

        //generalized states
        MIDAIR,
        ATTACKING,
        DEAD,
        GROUNDINCAPACITATED,
        AIRINCAPACITATED,
        DASHING,

        //possibility states
        CANMOVE,
        CANJUMP,
        UNTOUCHABLE,
    }

    // put the animator state string names here for autocomplete and stuff
    private const string LedgeGrabbing  = "LedgeGrabbing";
    private const string Grounded  = "Grounded";
    private const string Rising = "Rising";
    private const string Falling = "Falling";
    private const string Tumbling = "Tumbling";
    private const string Reeling = "Reeling";
    private const string Landing = "Landing";
    private const string Blocking = "Blocking";
    private const string Stunned = "Stunned";
    private const string GroundDashing = "GroundDashing";
    private const string AirDashing = "AirDashing";

    private const string MostlyDead = "MostlyDead";
    private const string AllDead = "AllDead";
    //Miracle Max: There's a big difference between mostly dead and all dead. Mostly dead is slightly alive. With all dead, well, with all dead there's usually only one thing you can do.
    //Inigo Montoya: What's that?
    //Miracle Max: Go through his clothes and look for loose change.
    private const string Respawning = "Respawning";

    private const string Downed = "Downed";


    //attacks
    private const string NeutralAttack = "NeutralAttack";
    private const string ForwardTiltAttack = "ForwardTiltAttack";
    private const string DownTiltAttack = "DownTiltAttack";
    private const string UpTiltAttack = "UpTiltAttack";
    private const string Lag = "Lag";
    private const string NeutralSmash = "NeutralSmash";
    private const string ForwardSmash = "ForwardSmash";
    private const string DownSmash = "DownSmash";
    private const string UpSmash = "UpSmash";

    //transition states
    private const string LedgeDropping = "LedgeDropping";
    private const string Unconscious = "Unconscious";
    private const string Recovering = "Recovering";

	// Dictionary of states linked to their substates
    private static Dictionary<State, string[]> stateStrings = new Dictionary<State, string[]> //mapping the enum to the string names in the Animator
    { 
        { State.LEDGEGRABBING,  new string[] { LedgeGrabbing, } },
        { State.GROUNDED,       new string[] { Grounded, } },
        { State.RISING,         new string[] { Rising, } },
        { State.FALLING,        new string[] { Falling, } },
        { State.TUMBLING,       new string[] { Tumbling, } },
        { State.REELING,        new string[] { Reeling, } },
        { State.MOSTLYDEAD,     new string[] { MostlyDead, } },
        { State.ALLDEAD,        new string[] { AllDead, } },
        { State.RESPAWNING,     new string[] { Respawning, } },
        { State.DOWNED,         new string[] { Downed, } },
        { State.LANDING,        new string[] { Landing, } },
        { State.BLOCKING,       new string[] { Blocking, } },
        { State.STUNNED,        new string[] { Stunned,} },
        { State.GROUNDDASHING,  new string[] { GroundDashing, } },
        { State.AIRDASHING,     new string[] { AirDashing, } },

        //attacks
        { State.NEUTRALATTACK,   new string[] { NeutralAttack, } },
        { State.FORWARDTILTATTACK, new string[] { ForwardTiltAttack, } },
        { State.DOWNTILTATTACK, new string[] { DownTiltAttack, } },
        { State.UPTILTATTACK, new string[] { UpTiltAttack, } },

        { State.NEUTRALSMASH,   new string[] { NeutralSmash, } },
        { State.FORWARDSMASH,   new string[] { ForwardSmash, } },
        { State.DOWNSMASH,      new string[] { DownSmash, } },
        { State.UPSMASH,        new string[] { UpSmash, } },

        //transition states
        { State.LEDGEDROPPING,  new string[] { LedgeDropping, } },
        { State.LAG,            new string[] { Lag, } },
        { State.UNCONSCIOUS,    new string[] { Unconscious, } },
        { State.RECOVERING,     new string[] { Recovering, } },


        //generalized states
        { State.MIDAIR,         new string[] { Rising, Falling, LedgeDropping, Tumbling, Reeling, } },
        { State.ATTACKING,      new string[] { NeutralAttack, ForwardTiltAttack, DownTiltAttack, UpTiltAttack, NeutralSmash, ForwardSmash, DownSmash, UpSmash} },   
        { State.DEAD,           new string[] { MostlyDead, AllDead, Respawning, } },
        { State.GROUNDINCAPACITATED,  new string[] { Unconscious, Downed, Recovering, } },
        { State.AIRINCAPACITATED,   new string[] { Reeling, Tumbling, } },
        { State.DASHING,        new string[] { GroundDashing, AirDashing, } },

        //possibility states
        { State.CANMOVE,        new string[] { Rising, Falling, Grounded, Tumbling, AirDashing, } },
        { State.CANJUMP,        new string[] { Rising, Falling, Grounded, Tumbling, LedgeDropping, LedgeGrabbing, Respawning, } },
        { State.UNTOUCHABLE,    new string[] { LedgeGrabbing, MostlyDead, AllDead, Respawning, Unconscious, Downed, Recovering, Reeling, GroundDashing, AirDashing, } },

    };
    private Animator theStateMachine;

    private int timerTime = 0;
    private int timerEnd = 1;
	// Use this for initialization
	void Start () {
        theStateMachine = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (timerTime > 0)
        {
            timerTime += 1;

            if (timerTime >= timerEnd)
            {
                timerTime = 0;
                theStateMachine.SetBool(Triggers.timerDone, true);
            }
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

    public void startTimer(int numFrames) //only used for Reeling exit
    {
        timerTime = 1;
        timerEnd = numFrames;
        theStateMachine.SetBool(Triggers.timerDone, false);
    }
}
