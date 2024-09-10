using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosingDoor : MonoBehaviour
{

    [SerializeField] Vector2 closedPosition;
    [SerializeField] float timeToClose = 1f;

    [Header("Debug")]
    [SerializeField] bool hideGizmos;

    Vector3 startPosition;
    bool moving;

    private void Start()
    {
        startPosition = transform.position;
    }

    public void MoveDoor(bool moveUp)
    {
        if(!moving)
        {
            moving = true;
            StartCoroutine(doorAnimation(moveUp ? startPosition : closedPosition + (Vector2)transform.position));
        }
        else
        {
            StopAllCoroutines(); // Reset The Thing
            StartCoroutine(doorAnimation(moveUp ? startPosition : closedPosition + (Vector2)transform.position));
        }
    }

    private IEnumerator doorAnimation(Vector2 targetPos)
    {
        float timePassed = 0f;
        while (timePassed < timeToClose)
        {
            timePassed += Time.deltaTime;
            float linearT = timePassed / timeToClose;

            transform.position = Vector2.Lerp(transform.position, targetPos, linearT);
            yield return null;
        }
        moving = false;
    }

    public void ResetDoor()
    {
        moving = false;
        StopAllCoroutines();
        transform.position = startPosition;
    }

    private void OnDrawGizmos()
    {
        if(hideGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(closedPosition + (Vector2)transform.position, new Vector2(GetComponent<SpriteRenderer>().size.y, GetComponent<SpriteRenderer>().size.x));
    }

}
