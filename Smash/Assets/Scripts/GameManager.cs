using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	
	void Awake ()
	{
		Application.targetFrameRate = 60;
        GameObject[] players;
        players = GameObject.FindGameObjectsWithTag(Tags.Player);

        //ignore collisions between all players
        foreach (GameObject player1 in players)
        {
            CapsuleCollider colliderOne = player1.GetComponent<CapsuleCollider>();
            foreach (GameObject player2 in players)
            {
                CapsuleCollider colliderTwo = player2.GetComponent<CapsuleCollider>();

                if (!colliderOne.Equals(colliderTwo)) // make sure they aren't the same collider; cannot stop collisions between yourself
                {
                    Debug.Log(colliderOne.ToString() + colliderTwo.ToString());
                    Physics.IgnoreCollision(colliderOne, colliderTwo, true); // prevent physics collisions between the two players
                }
            }
        }
	}
}
