using System.Collections;
using UnityEngine;

public class Slider : MonoBehaviour
{

    [SerializeField] float timeToTarget = 1f;
    [SerializeField] bool moveVertical;
    [SerializeField] BasicEnemy.ColorVarients colorAffected;


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("DeathBoxes") && collision.GetComponent<BasicEnemy>().backOnGround)
        {
            if(collision.GetComponent<BasicEnemy>().colorSelected != colorAffected) return;


            collision.GetComponent<BasicEnemy>().backOnGround = false;
            collision.GetComponent<BasicEnemy>().stopJumpCylce = true;
            StartCoroutine(movement(collision.transform));
        }
    }



    IEnumerator movement(Transform target)
    {
        Debug.Log("Sliding enemy");

        yield return new WaitForSeconds(target.GetComponent<BasicEnemy>().pauseInBetween);

        int dir = GetComponent<SpriteRenderer>().flipX ? -1 : 1;

        Vector2 newPosition = moveVertical ? (Vector2)target.position + Vector2.up * ((GetComponent<SpriteRenderer>().size.x - 1) * dir) :
                                             (Vector2)target.position + Vector2.right * ((GetComponent<SpriteRenderer>().size.x - .5f) * dir);
        Vector2 startPosition = target.position;

        float time = Time.deltaTime;

        while(time / timeToTarget < 1f)
        {
            time += Time.deltaTime;

            target.position = Vector2.Lerp(startPosition, newPosition, time / timeToTarget);

            yield return null;
        }

        yield return new WaitForSeconds(target.GetComponent<BasicEnemy>().pauseInBetween);
        target.GetComponent<BasicEnemy>().stopJumpCylce = false;
    }


}
