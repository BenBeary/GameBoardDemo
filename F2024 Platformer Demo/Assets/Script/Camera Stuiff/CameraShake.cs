using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{


    float currentDuration;

    private IEnumerator Shake(float duration, float magnitude)
    {
    
        Vector3 originalPosition = transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float x = Random.Range(-1f, 1f) * (magnitude/ 50);
            float y = Random.Range(-1f, 1f) * (magnitude/ 50);

            transform.localPosition = originalPosition + new Vector3(x, y, 0f);
            currentDuration = duration - elapsedTime;
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for next frame
        }

        
        transform.localPosition = originalPosition; // Reset position
    }

    // Call this function from another script
    public void StartShake(float duration, float magnitude, bool priority = default)
    {
        if (priority)
        {
            StopAllCoroutines();
            StartCoroutine(Shake(duration, magnitude));
        }
        if(duration >= currentDuration)
        {
            StartCoroutine(Shake(duration, magnitude));
        }
    }


    public void CancelShake()
    {
        StopAllCoroutines();
    }
}

