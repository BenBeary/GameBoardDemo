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
    bool fading;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !fading)
        {
            StartCoroutine(fadeOut());
        }
    }

    IEnumerator fadeOut()
    {
        SoundManager.instance.playSound("Triggers", "Facade");
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
