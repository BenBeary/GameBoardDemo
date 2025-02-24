using System.Collections;
using UnityEngine;

public class CheckPoint_Animation : MonoBehaviour
{


    [SerializeField] Transform Flag;
    [SerializeField] float moveAmount;
    [SerializeField] float speed;

    public void MoveFlag(bool moveDown)
    {
        int direction = (moveDown? -1 : 1);
        Vector2 endPos = new Vector2(Flag.localPosition.x, Flag.localPosition.y + (moveAmount * direction));


        StartCoroutine(flagAnimate(endPos));

    }

    IEnumerator flagAnimate(Vector2 endPos)
    {
        float count = Time.deltaTime;

        while (true)
        {
            yield return null;
            count += Time.deltaTime / speed;
            Flag.localPosition = Vector2.Lerp(Flag.localPosition, endPos, count);
            if (count >= 1f) break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)Flag.position + Vector2.up * moveAmount + Vector2.right * (Flag.GetComponent<SpriteRenderer>().sprite.bounds.size.x/2), Flag.GetComponent<SpriteRenderer>().sprite.bounds.size);

        Gizmos.color = new Color(.8f, .6f, 0);
        Gizmos.DrawWireCube((Vector2)transform.position + GetComponent<BoxCollider2D>().offset, GetComponent<BoxCollider2D>().size);
    }

}
