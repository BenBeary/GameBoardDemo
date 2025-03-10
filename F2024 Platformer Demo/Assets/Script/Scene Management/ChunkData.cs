using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(BoxCollider2D))]
public class ChunkData : MonoBehaviour
{


    [Header("Chunking Chunk")]
    public bool isActiveChunk;

    [SerializeField] RegionController rgController;
    public Vector2 cameraClampArea { get { return Vector2.one * 2 + GetComponent<BoxCollider2D>().size - Camera.main.pixelRect.size / 64; } }
    public Vector2 offset { get { return GetComponent<BoxCollider2D>().offset; } }



    [ContextMenu("Parent All Objects In Chunk")]
    private void ParentAllObjectsInChunk()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(GetComponent<BoxCollider2D>().bounds.center, GetComponent<BoxCollider2D>().size, 0);

        foreach (Collider2D hit in hits)
        {
            if(hit.CompareTag("Player") || hit.CompareTag("Static")) continue;

            hit.transform.parent = transform;
        }
    }





    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube((Vector2)transform.position + offset, cameraClampArea);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube((Vector2)transform.position + offset, cameraClampArea + new Vector2(Camera.main.pixelWidth / 64, Camera.main.pixelHeight / 64));
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
            GameManager.Instance?.SetActiveChunk(this);
            CameraManager.instance?.TransitionCamera(CameraManager.instance.ClampMovement(CameraManager.instance.transform.position));
        }
    }
}
