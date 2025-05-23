using UnityEngine;



public class PlayerMoveTutorial : MonoBehaviour
{

    [SerializeField] SpriteRenderer selectedRender;


    public void SwitchSprite(Sprite newSprite)
    {
        selectedRender.sprite = newSprite;
    }


}
