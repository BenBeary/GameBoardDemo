using UnityEngine;
using UnityEngine.Events;

public class BossResetToTransform : MonoBehaviour
{

    public Transform targetSpot;
    public UnityEvent onPlayerDeath;


    private void Start()
    {
        PlayerController.playerReset += resetOnPlayerDeath;
    }


    private void OnDisable()
    {
        PlayerController.playerReset -= resetOnPlayerDeath;
    }

    void resetOnPlayerDeath()
    {
        onPlayerDeath.Invoke();
        transform.position = targetSpot.position;
    }








}
