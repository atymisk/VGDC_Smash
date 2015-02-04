using UnityEngine;
using System.Collections;

public class GrabEdgeBehavior : MonoBehaviour {

	public GameObject stopEdge;

	public Vector3 GetEdgePosition()
	{
		return stopEdge.transform.position;
	}
}
