using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Card : MonoBehaviour
{
    [SerializeField] float bobIntensity;
    [SerializeField] float bobFrequency;

    [TextArea(2, 4)]
    [SerializeField] string topDescription;
    [SerializeField] Sprite topImage;

    [TextArea(2,4)]
    [SerializeField] string bottomDescription;
    [SerializeField] Sprite bottomImage;


    

    private void FixedUpdate()
    {
        transform.Translate(Vector3.up * Mathf.Cos(Time.time * bobFrequency) * bobIntensity);
    }


    private void SendTutorialData()
    {
        TutorialUIFiller.Instance.FillBothSections(topDescription, topImage, bottomDescription, bottomImage);
        TutorialUIFiller.Instance.OpenTutorial();
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == PlayerController.instance.gameObject)
        {
            SendTutorialData();
            gameObject.SetActive(false);
        }
    }
}
