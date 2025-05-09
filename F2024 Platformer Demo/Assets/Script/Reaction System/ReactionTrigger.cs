using System.Collections;
using UnityEngine;


[RequireComponent(typeof(ReactionSpawner))]
public class ReactionTrigger : MonoBehaviour
{

    [SerializeField] Sprite[] reactionSprites;
    [SerializeField] float cooldown = 2f;
    [SerializeField] float reactionDuration = 2f;

    bool coolingDown;


    ReactionSpawner reactSpawn;

    private void Awake()
    {
        reactSpawn = GetComponent<ReactionSpawner>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !coolingDown)
        {
            TriggerReaction();
            StartCoroutine(cdTimer());
        }
    }


    public void TriggerReaction()
    {
        reactSpawn.CreateReaction(reactionSprites[Random.Range(0, reactionSprites.Length - 1)], reactionDuration);
    }


    IEnumerator cdTimer()
    {
        coolingDown = true;
        yield return new WaitForSeconds(cooldown);
        coolingDown = false;
    }
}
