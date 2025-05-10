using UnityEngine;

public class ReactionSpawner : MonoBehaviour
{

    SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    public void CreateReaction(Sprite sprite, float duration)
    {
        GameObject newReaction = new GameObject("reaction");
        SpriteRenderer sr = newReaction.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = 5;

        newReaction.transform.position = (Vector2)transform.position + Vector2.up * spriteRenderer.size.y/2;
        Vector3 targetPosition = (Vector2)newReaction.transform.position + spriteRenderer.size + new Vector2(Random.Range(-.5f,.2f), Random.Range(-.2f, .2f));

        float fadeDuration = duration / 3;
        float moveDuration = fadeDuration;

        LeanTweenType moveEaseType = LeanTweenType.easeOutQuad;


        if (sr == null)
        {
            Debug.LogError("No SpriteRenderer found on this GameObject.");
            return;
        }

        // Set initial transparency to 0
        Color color = sr.color;
        color.a = 0f;
        sr.color = color;

        // Fade in
        LeanTween.value(newReaction, 0f, 1f, fadeDuration)
            .setOnUpdate((float val) =>
            {
                Color c = sr.color;
                c.a = val;
                sr.color = c;
            });

        // Move to target with ease out
        LeanTween.move(newReaction, targetPosition, moveDuration)
            .setEase(moveEaseType);

        // Delay before fade out

        LeanTween.delayedCall(newReaction, duration, () =>
        {
            // Fade out
            LeanTween.value(newReaction, 1f, 0f, fadeDuration)
                .setOnUpdate((float val) =>
                {
                    Color c = sr.color;
                    c.a = val;
                    sr.color = c;
                })
                .setOnComplete(() =>
                {
                    Destroy(newReaction);
                });
        });
    }
}
