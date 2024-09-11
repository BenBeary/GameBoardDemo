using UnityEngine;

public class BossRoomTrigger : MonoBehaviour
{

    [SerializeField] ClosingDoor[] doorsToClose;
    [SerializeField] GameObject boss;
    [SerializeField] float bossCameraSize;

    [Header("Debug")]
    [SerializeField] bool hasBeenTriggered;
    [SerializeField] bool fightIsOver;

    private void Start()
    {
        boss.SetActive(false);
        PlayerController.playerReset += ResetFight;
    }

    private void OnDestroy()
    {
        PlayerController.playerReset -= ResetFight;
    }

    private void StartFight()
    {
        ChangeDoorState(false);
        boss.SetActive(true);
        GameManager.Instance.SetNewTarget(boss.transform, bossCameraSize);
    }

    private void ChangeDoorState(bool moveUp)
    {
        foreach (var door in doorsToClose)
        {
            door.MoveDoor(moveUp);
        }
    }


    private void ResetFight()
    {
        GameManager.Instance.SetNewTarget(null, 1);
        hasBeenTriggered = false;
        foreach(var door in doorsToClose)
        {
            door.ResetDoor();
        }
        boss.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!hasBeenTriggered && collision.gameObject == PlayerController.instance.gameObject)
        {
            hasBeenTriggered = true;
            StartFight();
        } 
    }

}
