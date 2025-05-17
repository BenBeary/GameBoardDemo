using UnityEngine;

public class EventSystemParadox : MonoBehaviour
{
    public static EventSystemParadox Instance;

    private void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
