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
    public const string LedgeGrabEnter = "LedgeGrabEnter";
    public const string LedgeGrabExit = "LedgeGrabExit";
    public const string StageEnter = "StageEnter";
    public const string StageExit = "StageExit";
    public const string PlatformEnter = "PlatformEnter";
    public const string PlatformExit = "PlatformExit";
    public const string Death = "Death";
}

public class States : MonoBehaviour
{
    public const string LedgeGrabbing = "LedgeGrabbing";
    public const string StageGrounded = "StageGrounded";
    public const string Rising = "Rising";
    public const string Falling = "Falling";
}