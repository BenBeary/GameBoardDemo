using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public static TitleManager Instance;
    [SerializeField] CanvasGroup canvasGroup;

    Image titleUI;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        titleUI = transform.GetChild(0).GetComponent<Image>();
    }

    public void ShowTitle(Sprite titleImage, float duration, float fadeSpeed)
    {
        StartCoroutine(titleAnimation(titleImage, duration, fadeSpeed));
    }


    private IEnumerator titleAnimation(Sprite titleImage, float duration, float fadeSpeed)
    {
        
        titleUI.sprite = titleImage;

        LeanTween.alphaCanvas(canvasGroup, 1, fadeSpeed);

        yield return new WaitForSeconds(duration);

        LeanTween.alphaCanvas(canvasGroup, 0, fadeSpeed);
    

    }


}
