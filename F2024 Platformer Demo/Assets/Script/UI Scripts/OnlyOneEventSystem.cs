using UnityEngine;
using UnityEngine.EventSystems;

public class OnlyOneEventSystem : MonoBehaviour
{
    private void Awake()
    {
        if(EventSystem.current != null && EventSystem.current != GetComponent<EventSystem>())
        {
            Destroy(EventSystem.current.gameObject);

            EventSystem.current = GetComponent<EventSystem>();
        }
    }
}
