using UnityEngine;
using UnityEngine.Events;

public class BossActivateOnLeave : MonoBehaviour
{

    public UnityEvent onTriggerLeave;
    bool triggered;




    public void resetTriggeredState ()
    {
       triggered = false;
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !triggered)
        {
            triggered = true;
            onTriggerLeave.Invoke();
        }
    }
}
