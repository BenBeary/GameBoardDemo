using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] Vector2 fallPosition;
    [SerializeField] float fallSpeed;

    bool falling;
    Vector2 startPos;

    private void Start()
    {
        startPos = transform.position;
        PlayerController.playerReset += ResetState;
    }

    private void OnDisable()
    {
        PlayerController.playerReset -= ResetState;
    }


    private void Update()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position + Vector3.up * .1f, GetComponent<SpriteRenderer>().size, 0, Vector2.up, 0.1f, LayerMask.GetMask("Default"));



        falling = hit.collider && hit.collider.CompareTag("Player");

        if (falling)
        {
            transform.position = Vector2.MoveTowards(transform.position, fallPosition + startPos, fallSpeed * Time.deltaTime);
            hit.transform.position += Vector3.down * (fallSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, startPos, fallSpeed * Time.deltaTime);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if(!Application.isPlaying ) Gizmos.DrawWireCube((Vector2)transform.position + fallPosition, GetComponent<SpriteRenderer>().size);
        else Gizmos.DrawWireCube(startPos + fallPosition, GetComponent<SpriteRenderer>().size);

    }


    private void ResetState()
    {
        transform.position = startPos;
        falling = false;
    }







}
