using UnityEngine;
using DataManage;
public class JumpCountUpgrade : MonoBehaviour
{

    [SerializeField] float bobIntensity;
    [SerializeField] float bobFrequency;

    bool hasBeenGrabbed;
    [SerializeField] RegionController region;

    private void OnEnable()
    {
        PlayerController.playerReset += resetObject;
    }
    private void OnDisable()
    {
        PlayerController.playerReset -= resetObject;
    }


    private void FixedUpdate()
    {
        transform.Translate(Vector3.up * Mathf.Cos(Time.time * bobFrequency) * bobIntensity);


        if(hasBeenGrabbed && PlayerController.instance.grounded)
        {
            PlayerController.instance.maxDots++;
            ItemSaver.saveItem(region,gameObject);
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerController>() != null && PlayerController.instance.maxDots < 6)
        {

            hasBeenGrabbed = true;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
        }
        
    }



    void resetObject()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;
        hasBeenGrabbed = false;
    }

}
