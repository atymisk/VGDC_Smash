using UnityEngine;
using System.Collections;

public class Tags : MonoBehaviour {
	public const string Stage = "Stage";
	public const string Player = "Player";
    public const string PlayerTrigger = "PlayerTrigger";
	public const string Boundary = "Boundary";
	public const string GrabEdge = "GrabEdge";
	public const string StopEdge = "StopEdge";
    public const string Platform = "Platform";
}

public class Triggers : MonoBehaviour
{
    public const string LedgeGrab = "LedgeGrab";
    public const string StageGrounded = "StageGrounded";
    public const string PlatformGrounded = "PlatformGrounded";
    public const string Death = "Death";
    public const string ReelingEnter = "ReelingEnter";
    public const string MovingDown = "MovingDown";
}