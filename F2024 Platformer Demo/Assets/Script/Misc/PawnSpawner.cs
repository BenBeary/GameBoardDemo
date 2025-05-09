using UnityEngine;

public class PawnSpawner : MonoBehaviour
{

    [SerializeField] GameObject spawnPrefab;
    [SerializeField] float spawnSpeed;
    bool isSpawning;


    public void triggerSpawning(bool turnOn)
    {
        if(!turnOn && isSpawning)
        {
            isSpawning = false;
            CancelInvoke();
        }
        else if (turnOn && !isSpawning) 
        {
            isSpawning = true;
            InvokeRepeating("SpawnPawn", 0, spawnSpeed);
        }
    }



    void SpawnPawn()
    {
        GameObject temp = Instantiate(spawnPrefab);
        BasicEnemy tempData = temp.GetComponent<BasicEnemy>();

        tempData.stopJumpCylce = false;
        temp.GetComponent<DotRandomizer>().RandomizeObject();
        if(tempData.dotColors.Length > 0 )
        {
            tempData.colorSelected = tempData.dotColors[temp.GetComponent<DotRandomizer>().spriteNumber];
        }

        temp.transform.position = transform.position;

    }




}
