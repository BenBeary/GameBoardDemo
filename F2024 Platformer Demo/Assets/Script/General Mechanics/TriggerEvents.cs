
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvents : MonoBehaviour
{

    public UnityEvent onTrigger;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && onTrigger != null)
        {
            Debug.Log("Event Triggered");
            onTrigger.Invoke();
        }
    }


}
