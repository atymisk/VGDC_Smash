using UnityEngine;

public class Acceleration
{
	// PRIVATE VARIABLES
	private float magnitude;
	private float? targetx;
	private float? targety;
	private float? targetz;
	private float originalMagnitude;
	private float? originalTargetx;
	private float? originalTargety;
	private float? originalTargetz;

	// CONSTRUCTOR: null values in the targetVelocity will allow values to pass through when applied to a vector.
	public Acceleration (float? targetx, float? targety, float? targetz, float magnitude)
	{
		this.magnitude = magnitude;
		this.targetx = targetx;
		this.targety = targety;
		this.targetz = targetz;
		this.originalTargetx = targetx;
		this.originalTargety = targety;
		this.originalTargetz = targetz;
		this.originalMagnitude = magnitude;
	}

	// Get: magnitude
	public float getMagnitude() { return magnitude; }

	// Set: change values. null magnitude will keep it unchanged.
	public void Set(float? newx, float? newy, float? newz, float? magnitude)
	{
		this.targetx = newx;
		this.targety = newy;
		this.targetz = newz;
		this.magnitude = (magnitude.HasValue)? magnitude.Value : this.magnitude;
	}

	// Reset: revert current values to original values passed into the constructor.
	public void Reset()
	{
		this.targetx = originalTargetx;
		this.targety = originalTargety;
		this.targetz = originalTargetz;
		this.magnitude = originalMagnitude;
	}

	// ApplyToVector: return a vector with the acceleration applied to it.
	public Vector3 ApplyToVector(Vector3 current)
	{
		float x = (targetx.HasValue)? targetx.Value : current.x;
		float y = (targety.HasValue)? targety.Value : current.y;
		float z = (targetz.HasValue)? targetz.Value : current.z;
		return Vector3.MoveTowards(current, new Vector3(x, y, z), magnitude * Time.fixedDeltaTime);
	}

	// ToString: override
	public override string ToString()
	{
		return "(" + targetx.ToString() + ", " + targety.ToString() + ", " + targetz.ToString() + ", " + magnitude.ToString() + ")";
	}
}
