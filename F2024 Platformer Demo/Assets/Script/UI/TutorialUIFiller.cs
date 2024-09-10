using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialUIFiller : MonoBehaviour
{
    public static TutorialUIFiller Instance;

    [Header("Top Container")]
    [SerializeField] TMP_Text topText;
    [SerializeField] Image topImage;
    [Header("Bottom Container")]
    [SerializeField] TMP_Text bottomText;
    [SerializeField] Image bottomImage;

    [Header("Extra")]
    [SerializeField] GameObject buttonObject;
    [SerializeField] GameObject bottomContainer;
    bool hideBottomRow;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        CloseTutorial();
    }



    public void OpenTutorial()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        bottomContainer.SetActive(!hideBottomRow);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(buttonObject);
        Time.timeScale = 0;
        GameManager.Instance.cantPause = true;
    }

    public void CloseTutorial()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        GameManager.Instance.cantPause = false;
        Time.timeScale = 1f;
    }


    public void FillBothSections(string topDescription,Sprite t_Image, string botDescription, Sprite b_Image) 
    {
        topText.gameObject.SetActive(!string.IsNullOrEmpty(topDescription));
        topText.text = topDescription;
        topImage.gameObject.SetActive((t_Image == null) ? false : true);
        topImage.sprite = t_Image;

        bottomText.gameObject.SetActive(!string.IsNullOrEmpty(botDescription));
        bottomText.text = botDescription;
        bottomImage.gameObject.SetActive((b_Image == null) ? false : true);
        bottomImage.sprite = b_Image;

        hideBottomRow = (string.IsNullOrEmpty(botDescription) && b_Image == null)? true : false;
    }

}
