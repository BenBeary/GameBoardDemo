using System.Collections;
using UnityEngine;

public class Domino_Dropper : MonoBehaviour
{

    [SerializeField] float timeBeforeDrop;
    [SerializeField] float timeAfterDrop;
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


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!triggered && collision.gameObject.GetComponent<PlayerController>() != null || !triggered && collision.tag == "Enemy")
        {
            Debug.Log("Dropper Triggered");
            triggered = true;
            StartCoroutine(dropper());
        }
       
    }

}
