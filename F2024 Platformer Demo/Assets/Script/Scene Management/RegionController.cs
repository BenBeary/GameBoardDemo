using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionController : MonoBehaviour
{

    [System.Serializable]
    public struct ItemData
    {
        public string itemId;
        public GameObject obj;
        [HideInInspector] public bool hasBeenCleared;
    }

    public static RegionController Instance;




    public ChunkData activeChunk;

    List<ItemData> saveditems = new List<ItemData>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }


    private void OnEnable()
    {
        foreach(ItemData item in saveditems)
        {

        }
    }


    public void SetNewChunk(ChunkData newChunk)
    {

        // do chunky stuff

        activeChunk = newChunk;


    }

}
