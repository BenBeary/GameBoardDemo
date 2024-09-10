using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MadKnightManager : MonoBehaviour
{
    [Tooltip("The Amount of times the boss jumps around")]
    [SerializeField] int jumpingCycles = 1;

    BasicEnemy jumpingState;

    bool hasJumped;
    Vector2 startPos;
    private void Awake()
    {
        jumpingState = GetComponent<BasicEnemy>();
        startPos = transform.position;
    }

    private void OnEnable()
    {
        StartCoroutine(startRunningState());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        GetComponent<SpriteRenderer>().flipX = false;
        transform.position = startPos;
        ResetStates();
    }

    private void Update()
    {
        if (!hasJumped && jumpingState.backOnGround)
        {
            hasJumped = true;
            GameManager.Instance.CameraShake(1.2f, .25f);
        }
        if (hasJumped && !jumpingState.backOnGround) hasJumped = false;
    }

    IEnumerator startRunningState()
    {
        yield return new WaitForSecondsRealtime(1);
        transform.GetChild(1).gameObject.SetActive(true); // Snort


        yield return new WaitForSecondsRealtime(1);
        jumpingState.enabled = true;

        int currentCycle = 0;
        bool cycleComplete = true;
        while(jumpingCycles > currentCycle)
        {
            if(jumpingState.currentTarget == 0 && !cycleComplete)
            {
                cycleComplete = true;
                currentCycle++;

            }
            if(jumpingState.currentTarget == 1) cycleComplete = false;

            yield return null;
        }
        yield return new WaitForSecondsRealtime(jumpingState.timeToJump + jumpingState.pauseInBetween + .1f); // Wait for 1 more jump to complete
        jumpingState.enabled = false;
    }

    private void ResetStates()
    {
        jumpingState.enabled = false;
    }
}
