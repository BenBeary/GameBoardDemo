using UnityEditor;
using UnityEngine;

public class PlayerSlideColorChange : MonoBehaviour
{


    [SerializeField] BasicEnemy.ColorVarients affectedColor;
    [Range(1,9)]
    [SerializeField] int amountOfSlides = 1;
    public static PlayerSlideColorChange currentlySelected;

    [Header("Number Sprites")]
    [SerializeField] SpriteRenderer numbSprite;
    [SerializeField] Sprite[] sprites;




    private void OnValidate()
    {
    #if UNITY_EDITOR
        EditorApplication.delayCall += () =>
        {
            if (this != null && numbSprite != null && sprites != null && amountOfSlides < sprites.Length)
            {
                numbSprite.sprite = sprites[amountOfSlides];
            }
        };
    #endif
    }


private void Update()
    {
        if (currentlySelected == this)
        {
            if (numbSprite) numbSprite.sprite = sprites[PlayerController.instance.slidesLeft];
        }
        if (currentlySelected == this && PlayerController.instance.slidesLeft <= 0)
        {
            if (numbSprite) numbSprite.sprite = sprites[amountOfSlides];
            currentlySelected = null;
        }
        else if(currentlySelected != this)
        {
            if (numbSprite) numbSprite.sprite = sprites[amountOfSlides];
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController.instance.currentSlideColor = affectedColor;
            PlayerController.instance.slidesLeft = amountOfSlides;
            currentlySelected = this;
        }
    }






}
