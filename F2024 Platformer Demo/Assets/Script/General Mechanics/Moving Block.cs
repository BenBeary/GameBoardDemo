using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MovingBlock : MonoBehaviour
{

    public float speed = 5f;
    public bool loopBack;
    [SerializeField] List<Vector2> positions = new List<Vector2>();

    [Header("Debug")]
    Vector2 startPos;
    int currentPos;
    int dir = 1;
    bool active;


    private void Start()
    {
        startPos = transform.position;
        if(transform.parent == null) 
        {
            Debug.LogError(name + " does not have a chunk parent");
        }

        PlayerController.playerReset += resetToStartPos;
    }


    private void OnDisable()
    {
        PlayerController.playerReset -= resetToStartPos;
    }

    private void Update()
    {
        if(transform.parent == null)
        {
            return;
        }

        if(transform.parent.GetComponent<ChunkData>().isActiveChunk && active == false)
        {
            active = true;
        }
        else if(!transform.parent.GetComponent<ChunkData>().isActiveChunk && active) 
        {
            Invoke("resetToStartPos", 0.5f);
        }

    }


    public void resetToStartPos()
    {
        transform.position = startPos;
        active = false;
        currentPos = 0;
    }

    private void FixedUpdate()
    {
        if (!active) return;

        transform.position = Vector2.MoveTowards(transform.position, startPos + positions[currentPos], speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, startPos + positions[currentPos]) == 0) 
        {
            
            currentPos += dir;

            if (loopBack && currentPos >= positions.Count) currentPos = 0;
            else if (currentPos >= positions.Count) { dir *= -1; currentPos += dir; }
            else if( currentPos < 0 ) { dir *= -1; currentPos += dir; }
        }
    }



    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Get the current direction Block is heading
            Vector2 objDir = (startPos + positions[currentPos] - (Vector2)transform.position).normalized;

            collision.transform.Translate(objDir * speed * Time.deltaTime);
        }
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        for(int i = 0; i < positions.Count; i++)
        {
            if(i >= positions.Count-1)
            {
                if (loopBack)
                {
                    if (Application.isPlaying) Gizmos.DrawLine(startPos + positions[i], startPos + positions[0]);
                    else Gizmos.DrawLine((Vector2)transform.position + positions[i], (Vector2)transform.position + positions[0]);
                }

            }
            else
            {
                if (Application.isPlaying) Gizmos.DrawLine(startPos + positions[i], startPos + positions[i+1]);
                else Gizmos.DrawLine((Vector2)transform.position + positions[i], (Vector2)transform.position + positions[i+1]);
            }
        }

        Gizmos.color = Color.cyan;
        foreach (Vector2 pos in positions)
        {
            if (Application.isPlaying) Gizmos.DrawWireCube(startPos + pos, GetComponent<BoxCollider2D>().bounds.size);
            else Gizmos.DrawWireCube((Vector2)transform.position + pos, GetComponent<BoxCollider2D>().bounds.size);
        }
    }



}
