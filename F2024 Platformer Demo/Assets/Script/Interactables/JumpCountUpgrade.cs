using UnityEngine;

public class JumpCountUpgrade : MonoBehaviour
{

    [SerializeField] float bobIntensity;
    [SerializeField] float bobFrequency;


    private void FixedUpdate()
    {
        transform.Translate(Vector3.up * Mathf.Cos(Time.time * bobFrequency) * bobIntensity);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerController>() != null && PlayerController.instance.maxDots < 6)
        {
            PlayerController.instance.maxDots++;
            Destroy(gameObject);
        }
        
    }

}
