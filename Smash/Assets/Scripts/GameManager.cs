using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	
	void Awake ()
	{
		Application.targetFrameRate = 60;
        CapsuleCollider one = GameObject.Find("player").GetComponent<CapsuleCollider>();
        CapsuleCollider two = GameObject.Find("player2").GetComponent<CapsuleCollider>();
        Physics.IgnoreCollision(one, two, true);
	}
}
