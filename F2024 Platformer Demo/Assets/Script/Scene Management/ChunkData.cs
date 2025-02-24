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

    public bool isActiveChunk;


    [Header("Camera Settings")]
    public Vector2Int cameraClampArea;

    [Header("Double Chunk Chocolate Chip Cookie")]
    [Tooltip("Only Store Items that need to be destoryed on load")]
    public List<ItemData> items = new List<ItemData>();


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube((Vector2)transform.position, (Vector2)cameraClampArea);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube((Vector2)transform.position, cameraClampArea + new Vector2(Camera.main.pixelWidth/32, Camera.main.pixelHeight/32)/2);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneController.Instance.SetNewChunk(this);
        }
    }
}
