using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTeleporter : MonoBehaviour
{
    [System.Serializable]
    public enum direction
    {
        up, down, left, right
    }

    [SerializeField] direction teleportDirection;
    [SerializeField] float teleportDistance;

    public Vector2 vectDirection
    {
        get
        {
            switch (teleportDirection)
            {
                case direction.up:
                    return Vector2.up * teleportDistance;

                case direction.down:
                    return Vector2.down * teleportDistance;

                case direction.left:
                    return Vector2.left * teleportDistance;

                case direction.right:
                    return Vector2.right * teleportDistance;

                default: return Vector2.zero;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.Translate(vectDirection);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2)transform.position + vectDirection, GetComponent<SpriteRenderer>().size);
    }

}
