using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Facade : MonoBehaviour
{

    [System.Serializable]
    public enum colliderType
    {
        none,
        requireGrounded,
    }

    [SerializeField] float fadeOutTime = .5f;
    [SerializeField] Facade cascadingObject;
    bool fading;
    bool isCascading;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !fading)
        {
            StartCoroutine(fadeOut());
            if (cascadingObject) cascadingObject.triggerCascade();
        }
    }


    public void triggerCascade()
    {
        StartCoroutine(fadeOut());
        isCascading = true;
    }

    IEnumerator fadeOut()
    {
        // If Cascading, dont repeat the sound call
        if(!isCascading) SoundManager.instance.playSound("Triggers", "Facade");
        fading = true;
        float timePassed = 0;
        Color temp = GetComponent<SpriteRenderer>().color;

        while (true)
        {
            timePassed += Time.deltaTime;
            float fixedTime = timePassed / fadeOutTime; 
            temp.a = Mathf.Lerp(1, 0, fixedTime);

            GetComponent<SpriteRenderer>().color = temp;

            if (fixedTime >= 1f) break;

            yield return null;
        }
        Destroy(gameObject);
    }
}
