using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	
    public AudioClip[] BGMPlaylist; //MUST BE ASSIGNED!
    private int currentBGM = 0; //the playlist should be a queue, but it's easier on non-programmers if we make it an array
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

    void FixedUpdate()
    {
        if (!audio.isPlaying)
        {
            currentBGM += 1;
            if(currentBGM >= BGMPlaylist.Length) //we've reached the end of the array and need to return to the beginning
                currentBGM = 0;
            audio.clip = BGMPlaylist[currentBGM];
            audio.Play();
        }
    }
}
