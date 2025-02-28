using UnityEngine;

public class ItemSaver : MonoBehaviour
{
    [SerializeField] RegionController region;

    private void OnDisable()
    {
        region.AddCollectedItem(gameObject);
    }
}
