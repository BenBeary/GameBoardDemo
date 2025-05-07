using DataManage;
using UnityEngine;




public class item : MonoBehaviour
{

    [SerializeField] RegionController region;
    [SerializeField] SpriteRenderer sprite;
    bool hasBeenGrabbed;

    private void Start()
    {
        PlayerController.playerReset += resetItem;
    }


    private void OnDisable()
    {
        PlayerController.playerReset -= resetItem;
    }

    private void Update()
    {
        if (!hasBeenGrabbed) return;


        if (PlayerController.instance.grounded)
        {
            ItemSaver.saveItem(region, gameObject);
            Destroy(gameObject);
        }

    }


    void resetItem()
    {
        hasBeenGrabbed = false;
        sprite.enabled = true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            hasBeenGrabbed = true;
            sprite.enabled = false;
        }
    }

}
