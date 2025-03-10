using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class ButtonTrigger : MonoBehaviour
{

    [Header("Button Settings")]
    [SerializeField] float pushDownDistance = .1f;
    [SerializeField] float downTimeLength = 1f;

    [Space(20)]
    public UnityEvent onPressed;
    public UnityEvent onRelease;

    bool isPressed;

    Vector2 startPos;


    private void Start()
    {
        startPos = transform.position;
    }

    IEnumerator pressedTimeDelay()
    {
        
        yield return new WaitForSeconds(downTimeLength);

        transform.position = startPos;

        onRelease.Invoke();
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isPressed && collision.CompareTag("Player"))
        {
            isPressed = true;
            StopAllCoroutines();
            transform.position = startPos + Vector2.down * pushDownDistance;
            onPressed.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(isPressed && collision.CompareTag("Player"))
        {
            isPressed = false;
            StartCoroutine(pressedTimeDelay());
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;



        Gizmos.DrawWireCube(transform.position + Vector3.down * pushDownDistance, GetComponent<SpriteRenderer>().size);
    }


}
