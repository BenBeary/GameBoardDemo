
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvents : MonoBehaviour
{

    public UnityEvent onTrigger;
    public bool allowAllTriggers;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == gameObject || collision.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast")) return;

        if (collision.CompareTag("Player") && onTrigger != null || allowAllTriggers && onTrigger != null)
        {
            Debug.Log(collision.name + "Triggered Event");
            onTrigger.Invoke();
        }
    }


}
