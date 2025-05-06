using UnityEngine;

public class UIMovement : MonoBehaviour
{

    [SerializeField] Vector2 targetPosition; // The UI target position
    [SerializeField] float moveDuration = 0.5f;     // Duration of the move
    [SerializeField] LeanTweenType easeType = LeanTweenType.easeInOutQuad;
    
    
    RectTransform rectTransform;
    Vector2 startPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;
    }

    public void MoveToTarget()
    {
        // Move anchoredPosition (relative to parent)
        LeanTween.value(gameObject, rectTransform.anchoredPosition, targetPosition, moveDuration).setEase(easeType).setOnUpdate((Vector2 val) => { rectTransform.anchoredPosition = val; });
    }

    public void MoveToStart()
    {

        LeanTween.value(gameObject, rectTransform.anchoredPosition, startPosition, moveDuration).setEase(easeType).setOnUpdate((Vector2 val) => { rectTransform.anchoredPosition = val; });
    }
}
