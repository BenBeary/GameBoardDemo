using UnityEngine;
using UnityEngine.EventSystems;

public class OnlyOneEventSystem : MonoBehaviour
{
    private void Awake()
    {
        if(EventSystem.current != null && EventSystem.current != GetComponent<EventSystem>())
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
