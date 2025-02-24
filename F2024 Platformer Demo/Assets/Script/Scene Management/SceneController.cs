using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    public ChunkData activeChunk;



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



    public void SetNewChunk(ChunkData newChunk)
    {

        // do chunky stuff

        activeChunk = newChunk;


    }

}
