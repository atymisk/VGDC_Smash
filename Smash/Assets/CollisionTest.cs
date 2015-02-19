using UnityEngine;
using System.Collections;

public class CollisionTest : MonoBehaviour {
    PlayerController controller; //reference to PlayerController script
	// Use this for initialization
	void Start () {
        controller = transform.parent.GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other);
    }

    void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case Tags.Platform:
                Debug.Log(other);
                controller.OnPlatformDropEnd(other);
                break;
            default:
                break;
        }
    }
}
