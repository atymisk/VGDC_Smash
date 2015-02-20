using UnityEngine;
using System.Collections;

public class PlayerTriggerColliderScript : MonoBehaviour
{
    PlayerController controller; //reference to PlayerController script

    void Start()
    {
        controller = transform.parent.GetComponent<PlayerController>();
    }

    void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case Tags.Platform :
                controller.OnPlatformDropEnd(other); // notify the PlayerController that we're outside the platform and that collision can be re-enabled. (Must be done in this trigger collider because disabling collisions also disabled detection of the end of the collision on the main collider)
                break;
            default :
                break;
        }
    }
}
