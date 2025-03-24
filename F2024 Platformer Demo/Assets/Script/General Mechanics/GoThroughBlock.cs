using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class GoThroughBlock : MonoBehaviour
{
    [SerializeField] bool stopFallThrough;

    // Update is called once per frame
    void Update()
    {
        // turns off collider if player is holding "S" or under
        if (PlayerController.instance.transform.position.y < transform.position.y + GetComponent<SpriteRenderer>().size.y) 
        {
            GetComponent<BoxCollider2D>().enabled = false;
        }
        else if(Input.GetAxis("Vertical") < -0.1f && !stopFallThrough)
        {
            GetComponent<BoxCollider2D>().enabled = false;
        }
        else
        {
            GetComponent<BoxCollider2D>().enabled = true;
        }
    }
}
