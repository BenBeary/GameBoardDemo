using System.Collections;
using UnityEngine;

public class Domino_Dropper : MonoBehaviour
{

    [SerializeField] float timeBeforeDrop;
    [SerializeField] float timeAfterDrop;
    [Header("Debug")]
    [SerializeField] bool triggered;
    [SerializeField] Collider2D boxCol;

    GameObject domino_DroppedState;

    private void Start()
    {
        domino_DroppedState = transform.GetChild(0).gameObject;
        domino_DroppedState.SetActive(false);
    }


    private IEnumerator dropper()
    {
        yield return new WaitForSecondsRealtime(timeBeforeDrop);
        boxCol.enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        domino_DroppedState.SetActive(true);
        
        yield return new WaitForSecondsRealtime(timeAfterDrop);
        boxCol.enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
        domino_DroppedState.SetActive(false);

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
