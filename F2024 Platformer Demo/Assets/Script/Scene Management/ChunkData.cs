using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class ChunkData : MonoBehaviour
{
    [System.Serializable]
    public struct ItemData
    {
        public string itemId;
        public GameObject obj;
        [HideInInspector] public bool hasBeenCleared;
    }

    [Header("Chunking Chunk")]
    public bool isActiveChunk;
    public string ChunkID;


    public Vector2 cameraClampArea { get { return Vector2.one * 2 + (Vector2)GetComponent<BoxCollider2D>().bounds.size - Camera.main.pixelRect.size / 64; } }

    [Header("Double Chunk Chocolate Chip Cookie")]
    [Tooltip("Only Store Items that need to be destoryed on load")]
    public List<ItemData> items = new List<ItemData>();


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube((Vector2)transform.position, cameraClampArea);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube((Vector2)transform.position, cameraClampArea + new Vector2(Camera.main.pixelWidth / 64, Camera.main.pixelHeight / 64));
    }





    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneController.Instance.SetNewChunk(this);
            Camera.main.GetComponent<CameraManager>().TransitionCamera(Camera.main.GetComponent<CameraManager>().ClampMovement(Camera.main.transform.position));
        }
    }
}
