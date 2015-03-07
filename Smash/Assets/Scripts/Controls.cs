using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Controls : MonoBehaviour {

	// Command: enumeration of possible commands
	public enum Command {
        ATTACK,
        SPECIAL,
        JUMP,
		DUCK,
        MOVE,
        BLOCK,
        DASH,
	};

    public enum Controller {
        ONE,
        TWO,
        THREE,
        FOUR,
    }

    private static Dictionary<Command, string> commandStrings = new Dictionary<Command, string>
    { 
        { Command.ATTACK, "InputAttack" },
        { Command.SPECIAL, "InputSpecial" },
        { Command.JUMP, "InputJump" },
        { Command.DUCK, "InputDuck" },
        { Command.MOVE, "InputMove" },
        { Command.BLOCK, "InputBlock" },
        { Command.DASH, "InputDash" },
    };
    public static string CommandToString(Command com)
    {
        return commandStrings[com];
    }

    private static Dictionary<Controller, string> controllerStrings = new Dictionary<Controller, string>
    {
        { Controller.ONE, "One" },
        { Controller.TWO, "Two" },
        { Controller.THREE, "Three" },
        { Controller.FOUR, "Four" },
    };
    public static string ControllerToString(Controller con)
    {
        return controllerStrings[con];
    }

    public Controller controller;

	// PRIVATE VARIABLES
	private Dictionary<Command, float> holdDict;			// tracks command hold duration
	private Dictionary<Command, HashSet<KeyCode>> keyDict;	// stores association between commands and controller inputs
	private Dictionary<Command, bool> startDict;			// track newly issued commands
	private Dictionary<Command, bool> endDict;				// track newly ended commands
    private Animator theStateMachine;
	private float previousFacing;
	private Vector2 stickInput;

	// INITIALIZE
	void Awake()
	{
		holdDict = new Dictionary<Command, float>();
		keyDict = new Dictionary<Command, HashSet<KeyCode>>();
		startDict = new Dictionary<Command, bool>();
		endDict = new Dictionary<Command, bool>();
		previousFacing = 0f;
		stickInput = Vector2.zero;
        theStateMachine = GetComponent<Animator>();
        InitializeKeyDict();
		InitializeHoldDict();
		InitializeStartDict();
		InitializeEndDict();
	}

	// DoUpdate forces controls to update values so it stays synced with other Update calls
	public void DoUpdate()
	{
		stickInput = GetNormalizedAxisInput();
		foreach (Command com in keyDict.Keys)
		{
			bool switchDir = false;
			if (com == Command.MOVE) {		// handle move separately for directional switches
				float mag = GetCommandMagnitude(com);
				if (mag * previousFacing < 0) {	// if direction changed while moving ...
					startDict[com] = true;				// start a new command, don't flag end
					holdDict[com] = Time.deltaTime;
					switchDir = true;
				}
				// Otherwise, either command isn't being issued or command is being issue in same direction
				previousFacing = Mathf.Sign(mag);	// assign new facing
			}
			if (!switchDir) {
				if (GetCommand (com)) {			// if the command is being issued ...
                    if (holdDict[com] == 0f)
                    {
                        startDict[com] = true;			// flag start command
                        theStateMachine.SetTrigger(CommandToString(com) + "Trigger");
                    }
                    endDict[com] = false;
					holdDict[com] += Time.deltaTime;	// add to hold time
                    theStateMachine.SetBool(CommandToString(com) + "Bool", true);
				} else {						// Otherwise ...
                    if (holdDict[com] > 0f)
                    {
                        endDict[com] = true;			// flag end command
                        theStateMachine.ResetTrigger(CommandToString(com) + "Trigger");
                    }
					startDict[com] = false;				// clear start command
					holdDict[com] = 0f;					// reset hold time
                    theStateMachine.SetBool(CommandToString(com) + "Bool", false);
				}
			}
		}
	}

	// INITIALIZERS
	private void InitializeKeyDict()
	{
		keyDict[Command.ATTACK] = new HashSet<KeyCode>{KeyCode.JoystickButton0, KeyCode.X, KeyCode.K};
		keyDict[Command.SPECIAL] = new HashSet<KeyCode>{KeyCode.JoystickButton1, KeyCode.Y, KeyCode.L};
        keyDict[Command.BLOCK] = new HashSet<KeyCode> { KeyCode.JoystickButton2, KeyCode.H }; // @TODO: map these to the correct joystick codes (bumpers+triggers)
        keyDict[Command.DASH] = new HashSet<KeyCode> { KeyCode.JoystickButton3, KeyCode.J };
		keyDict[Command.JUMP] = new HashSet<KeyCode>{KeyCode.Alpha0, KeyCode.JoystickButton2, KeyCode.JoystickButton3};
		keyDict[Command.DUCK] = new HashSet<KeyCode>{KeyCode.Alpha1};
		keyDict[Command.MOVE] = new HashSet<KeyCode>{KeyCode.Alpha2};
	}
	private void InitializeHoldDict() { foreach (Command com in keyDict.Keys) holdDict[com] = 0f; }
	private void InitializeStartDict() { foreach (Command com in keyDict.Keys) startDict[com] = false; }
	private void InitializeEndDict() { foreach (Command com in keyDict.Keys) endDict[com] = false; }

	// GETTERS
	public float GetCommandHoldDuration(Command com) { return holdDict[com]; }
	public bool GetCommand(Command com) { return GetCommandMagnitude(com) != 0f; }
	public bool GetCommandStart(Command com) { return startDict[com]; }
	public bool GetCommandEnd(Command com) { return endDict[com]; }
	// FLAG CONSUMERS
	public bool ConsumeCommandStart(Command com)
	{
		bool result = startDict[com];
		startDict[com] = false;
        theStateMachine.ResetTrigger(CommandToString(com) + "Trigger");
		return result;
	}
	public bool ConsumeCommandEnd(Command com)
	{
		bool result = endDict[com];
		endDict[com] = false;
		return result;
	}

	// GetCommandMagnitude: check the valid keys for a given command and return its magnitude.
	public float GetCommandMagnitude(Command com)
	{
		HashSet<KeyCode> validKeys = keyDict[com];
		foreach (KeyCode s in validKeys) {
			if (Input.GetKey (s))						// Normal commands
				return 1f;
			else if (s == KeyCode.Alpha0 && stickInput.y > 0.3f)	// Alpha0 reserved for up command
				return 1f;
			else if (s == KeyCode.Alpha1 && stickInput.y < -0.5f)	// Alpha1 reserved for down command
				return 1f;
			else if (s == KeyCode.Alpha2 && stickInput.x != 0f)		// Alpha 2 reserved for horizontal moves
				return stickInput.x;
        }
        return 0f;
	}

	// GetNormalizedAxisInput: normalize stick input and return as Vector2
	private Vector2 GetNormalizedAxisInput()
	{
		float deadZone = 0.2f;
        Vector2 inputVec = new Vector2(Input.GetAxis("Horizontal" + ControllerToString(controller)), Input.GetAxis("Vertical" + ControllerToString(controller)));
		if (inputVec.magnitude < deadZone)
			return Vector2.zero;
		return inputVec.normalized * ((inputVec.magnitude - deadZone) / (1 - deadZone));
	}
}
