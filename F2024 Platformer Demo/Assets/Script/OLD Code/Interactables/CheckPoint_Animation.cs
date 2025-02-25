using System.Collections;
using UnityEngine;

public class CheckPoint_Animation : MonoBehaviour
{


    [SerializeField] Transform Flag;
    [SerializeField] float moveAmount;
    [SerializeField] float animDuration;

    public void MoveFlag(bool moveDown)
    {
        int direction = (moveDown? -1 : 1);
        Vector2 endPos = new Vector2(Flag.localPosition.x, Flag.localPosition.y + (moveAmount * direction));


        StartCoroutine(flagAnimate(endPos));

    }

    IEnumerator flagAnimate(Vector2 endPos)
    {
        float count = Time.deltaTime;
        Vector2 start = Flag.localPosition;

        while (true)
        {
            yield return null;
            count += Time.deltaTime;
            Flag.localPosition = Vector2.Lerp(start, endPos, count / animDuration);
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
