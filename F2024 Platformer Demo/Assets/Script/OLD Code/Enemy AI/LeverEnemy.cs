using System.Collections;
using UnityEngine;


[RequireComponent(typeof(BasicEnemy))]
public class LeverEnemy : MonoBehaviour
{

    public Transform target;
    public Transform idleTarget;

    [Header("Lever Stats")]
    public LeverTrigger lever;
    float leverPullDelay = 1f;
    float delayedReaction = 3f;

    [Header("Reactions")]
    [SerializeField] Sprite firstIdleSprite;
    [SerializeField] Sprite idlingSprite;
    [SerializeField] Sprite pulledLeverSprite;
    [SerializeField] Sprite reactDelayFinishSprite;
    [SerializeField] float reactionDurations = 1f;
    [Tooltip("0 = infinite")]
    [SerializeField] int idleReactionLimit = 3;
    int currentReactions;

    bool enemyIsActive;
    bool enemyIdle = true;
    bool reacting;

    bool pullingLever;
    bool idling;


    BasicEnemy enemyAI;
    ReactionSpawner reaction;

    private void Awake()
    {
        enemyAI = GetComponent<BasicEnemy>();
        reaction = GetComponent<ReactionSpawner>();
    }



    private void Update()
    {
        if (!enemyIsActive) return;


        if (enemyIdle) GoToIdleState();
        else TryToActivateLever();


        if(lever.isActive == false && reacting == false && enemyIdle)
        {
            reacting = true;
            StartCoroutine(reactionTime());
        }
    }

    public void activateEnemy()
    {
        enemyIsActive = true;
    }

    public void switchState(bool idling)
    {
        enemyIdle = idling;
        StopAllCoroutines();
        this.idling = false;
        pullingLever = false;
        currentReactions = 0;
    }

    IEnumerator reactionTime()
    {
        yield return new WaitForSeconds(delayedReaction - .5f);
        
        if(reaction && reactDelayFinishSprite) reaction.CreateReaction(reactDelayFinishSprite,reactionDurations);
        
        yield return new WaitForSeconds(.5f);
        switchState(false);
        reacting = false;
    }


    void GoToIdleState()
    {
        if (enemyAI.targetPoints[0] != idleTarget || enemyAI.stopJumpCylce && Vector2.Distance(transform.position, idleTarget.position) != 0)
        {
            enemyAI.targetPoints[0] = idleTarget;
            enemyAI.stopJumpCylce = false;
        }


        if(Vector2.Distance(transform.position, idleTarget.position) == 0 && !idling)
        {
            enemyAI.stopJumpCylce = true;
            StartCoroutine(idleDelay());
        }
    }

    IEnumerator idleDelay()
    {
        idling = true;
        if(reaction)
        {
            if(idleReactionLimit == 0 || idleReactionLimit > currentReactions)
            {
                if(currentReactions == 0 && firstIdleSprite)
                {
                    reaction.CreateReaction(firstIdleSprite, reactionDurations);
                    yield return new WaitForSeconds(.5f);
                }
                else if(idlingSprite) reaction.CreateReaction(idlingSprite, reactionDurations);
                
                currentReactions++;
            }
        }
        yield return new WaitForSeconds(.5f);
        idling = false;
    }

    void TryToActivateLever()
    {
        if (enemyAI.targetPoints[0] != target || enemyAI.stopJumpCylce == true && Vector2.Distance(transform.position,target.position) != 0) // Move to target
        {
            enemyAI.targetPoints[0] = target;
            enemyAI.stopJumpCylce = false;
        }

        if(Vector2.Distance(transform.position, target.position) == 0 && !lever.isActive && !pullingLever) // Activate lever
        {
            enemyAI.stopJumpCylce = true;
            StartCoroutine(pullLever());
        }
    }

    IEnumerator pullLever()
    {
        pullingLever = true;
        yield return new WaitForSeconds(leverPullDelay);

        lever.ActivateLever();
        if (reaction && pulledLeverSprite) reaction.CreateReaction(pulledLeverSprite, reactionDurations);

        yield return new WaitForSeconds(0.5f);
        pullingLever = false;
        enemyIdle = true;
    }

}
