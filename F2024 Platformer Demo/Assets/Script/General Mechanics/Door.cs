using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    [SerializeField] Vector2 openedPosition;
    [SerializeField] float timeToClose = 1f;
    [SerializeField] float delayedActivation = 0f;

    [Header("Camera Effects")]
    [SerializeField] bool applyShake;
    [SerializeField] float shakeMagnitude = 0.2f;

    [Header("Debug")]
    [SerializeField] bool hideGizmos;
    Vector2 startPosition;
    bool doorIsOpen;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void OnDisable()
    {
        StopCoroutine(doorInteract(default));
        StopAllCoroutines();
    }

    public void InteractWithDoor(bool openDoor)
    {
        if (!doorIsOpen && !openDoor || doorIsOpen && openDoor) return;

        StopAllCoroutines();

        StartCoroutine(doorInteract(openDoor));
        doorIsOpen = openDoor;


    }


    IEnumerator doorInteract(bool openDoor)
    {
        yield return new WaitForSeconds(delayedActivation);

        // This should fix the snapping that happens when Lerp gets cancelled early
        float dist = Vector2.Distance(startPosition,openedPosition+startPosition);
        float curDist = openDoor ? Vector2.Distance(transform.position, openedPosition + startPosition) : Vector2.Distance(transform.position, startPosition);
        float distPercent = ( 1 - curDist / dist) * timeToClose;


        float timePassed = Mathf.Abs(distPercent );


        if (applyShake) Camera.main.GetComponent<CameraShake>()?.StartShake(timeToClose - distPercent, shakeMagnitude);

        while (true)
        {
            timePassed += Time.deltaTime;
            float overallTime = timePassed / timeToClose;
            //Debug.Log($"Door has moved {(overallTime * 100).ToString("#.")}%");   
            if (openDoor)
            {
                transform.position = Vector2.Lerp(startPosition, openedPosition + startPosition, overallTime);
            }
            else
            {
                transform.position = Vector2.Lerp(startPosition + openedPosition, startPosition, overallTime);
            }
            if(overallTime >= 1f) break;
            yield return null;
        }
    }


    private void OnDrawGizmos()
    {
        if(hideGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(transform.position + (Vector3)openedPosition, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, GetComponent<SpriteRenderer>().size);
    }

}
