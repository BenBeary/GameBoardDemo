using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EndSceneCutscene : MonoBehaviour
{

    [SerializeField] Transform targetLocation;
    [SerializeField] float timeToTake;

    bool cutsceneStarted;


    IEnumerator Cutscene(Vector2 start, Vector2 end)
    {
  
        GameManager.Instance.cantPause = true;
        while (!PlayerController.instance.grounded) // wait for player to be on ground
        {
            yield return null;
        }
        start = PlayerController.instance.transform.position;
        PlayerController.instance.hasInputPaused = true;
        yield return new WaitForSecondsRealtime(0.5f);
        PlayerController.instance.GetComponent<SpriteRenderer>().flipX = false;
        float timePassed = 0f;

        while (timePassed < timeToTake)
        {
            PlayerController.instance.GetComponent<Animator>().speed = 1f; // force that animator to work!

            timePassed += Time.deltaTime;

            float linearT = timePassed / timeToTake;

            PlayerController.instance.transform.position = Vector2.Lerp(start, end, linearT);


            yield return null;
        }
        PlayerController.instance.gameObject.SetActive(false);
        yield return new WaitForSecondsRealtime(5f);
        GameManager.Instance.RestartGame();

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == PlayerController.instance.gameObject && !cutsceneStarted)
        {
            cutsceneStarted = true;
            StartCoroutine(Cutscene(collision.transform.position,targetLocation.position));
        }
    }

}
