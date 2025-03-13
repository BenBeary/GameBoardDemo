using UnityEngine;

public class MusicChangeTrigger : MonoBehaviour
{
    [SerializeField] string songCategoryName, songSoundlistName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SoundManager.instance.PlaySingleSong(songCategoryName, songSoundlistName, true);
        }
    }
}
