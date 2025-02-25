using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class GoThroughBlock : MonoBehaviour
{
    

    // Update is called once per frame
    void Update()
    {
        // turns off collider if player is holding "S" or under
        if (PlayerController.instance.transform.position.y < transform.position.y + GetComponent<SpriteRenderer>().size.y / 64 || Input.GetAxisRaw("Vertical") < 0) 
        {
            GetComponent<BoxCollider2D>().enabled = false;
        }    
        else
        {
            GetComponent<BoxCollider2D>().enabled = true;
        }
    }
}
