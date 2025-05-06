using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CutscenePlayerMovement : MonoBehaviour
{

    [SerializeField] Transform targetPoint;
    public UnityEvent finishedMovement;


    public void BeginMovement()
    {
        StartCoroutine(movePlayer());
    }

    public void CancelMovement()
    {
        StopAllCoroutines();
    }

    IEnumerator movePlayer()
    {
        yield return new WaitUntil(() => PlayerController.instance.currentMomentum == Vector2.zero);
        yield return new WaitForSeconds(.5f);

        while (true)
        {
            PlayerController.instance.transform.position = Vector3.MoveTowards(PlayerController.instance.transform.position, targetPoint.position, PlayerController.instance.speed * Time.deltaTime);
            PlayerController.instance.motionInput.x = PlayerController.instance.transform.position.x - targetPoint.position.x >= 0 ? -1 : 1;

            if(Vector2.Distance(PlayerController.instance.transform.position, targetPoint.position) == 0)
            {
                PlayerController.instance.motionInput.x = 0;
                break;
            }

            yield return null;
        }

        finishedMovement.Invoke();

    }

}
