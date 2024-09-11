using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{

    [SerializeField] ClosingDoor door;

    bool hasBeenTriggered;

    IEnumerator openDoor()
    {
        
        yield return new WaitForSecondsRealtime(3);
        door.MoveDoor(false);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == PlayerController.instance.gameObject && !hasBeenTriggered)
        {
            hasBeenTriggered = true;
            StartCoroutine(openDoor());
        }
    }

}
