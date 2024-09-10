using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleTrigger : MonoBehaviour
{

    [SerializeField] Sprite titleImage;
    [SerializeField] float titleDuration;
    [SerializeField] float titleFadeSpeed;

    [Header("Debug")]
    [SerializeField] bool hasBeenTriggered;





    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!hasBeenTriggered && collision.gameObject == PlayerController.instance.gameObject)
        {
            hasBeenTriggered = true;
            TitleManager.Instance.ShowTitle(titleImage, titleDuration, titleFadeSpeed);
        }
    }

}
