using System.Collections;
using UnityEngine;

public class Domino_Dropper : MonoBehaviour
{
    [SerializeField] bool vertical;
    [SerializeField] float timeBeforeDrop;
    [SerializeField] float timeAfterDrop;
    [SerializeField] float shakeMagnitude = .5f;
    [Header("Debug")]
    [SerializeField] bool triggered;
    [SerializeField] Collider2D boxCol;

    [SerializeField] GameObject dominoTop;
    [SerializeField] GameObject dominoDroppedState;


    private void Start()
    {
        dominoDroppedState.SetActive(false);
    }


    private IEnumerator dropper()
    {
        StartCoroutine(Shake(timeBeforeDrop, shakeMagnitude));

        yield return new WaitForSecondsRealtime(timeBeforeDrop);
        boxCol.enabled = false;
        dominoTop.GetComponent<SpriteRenderer>().enabled = false;
        dominoDroppedState.SetActive(true);
        

        yield return new WaitForSecondsRealtime(timeAfterDrop);
        boxCol.enabled = true;
        dominoTop.GetComponent<SpriteRenderer>().enabled = true;
        dominoDroppedState.SetActive(false);

        triggered = false;
    }


    private IEnumerator Shake(float duration, float magnitude)
    {

        Vector3 originalPosition = dominoTop.transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float x = Random.Range(-1f, 1f) * (magnitude / 50);
            float y = Random.Range(-1f, 1f) * (magnitude / 50);

            dominoTop.transform.localPosition = originalPosition + new Vector3(x, y, 0f);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for next frame
        }


        dominoTop.transform.localPosition = originalPosition; // Reset position
    }



    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if(PlayerController.instance.grounded && !triggered && collision.CompareTag("Player") || !triggered && collision.CompareTag("Enemy") || vertical && !triggered && collision.CompareTag("Player"))
        {
            Debug.Log("Dropper Triggered");
            triggered = true;
            StartCoroutine(dropper());
        }
    }

}
