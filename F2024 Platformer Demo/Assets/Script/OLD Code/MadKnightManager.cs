using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MadKnightManager : MonoBehaviour
{
    [SerializeField] Collider2D antiJumpOverBox;

    BasicEnemy jumpingState;

    float startingJumpPause;
    bool hasJumped;
    Vector2 startPos;
    private void Awake()
    {
        jumpingState = GetComponent<BasicEnemy>();
        startPos = transform.position;
        startingJumpPause = jumpingState.pauseInBetween;
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
            GameManager.Instance.CameraShake(.8f, .25f);
        }
        if (hasJumped && !jumpingState.backOnGround) hasJumped = false;
    }

    IEnumerator startRunningState()
    {
        yield return new WaitForSecondsRealtime(1);
        transform.GetChild(1).gameObject.SetActive(true); // Snort


        yield return new WaitForSecondsRealtime(1);
        jumpingState.enabled = true;
        while (jumpingState.currentTarget == 0)
        {
            yield return null;
        }// wait till after first point
        antiJumpOverBox.enabled = true;

        while(jumpingState.currentTarget != 0 || jumpingState.enabled == false)
        {
            if (jumpingState.currentTarget == 16) jumpingState.pauseInBetween = .5f;
            else if (jumpingState.currentTarget == 20) jumpingState.pauseInBetween = startingJumpPause;
            else if (jumpingState.currentTarget == 22) antiJumpOverBox.enabled = false;
            else if (jumpingState.currentTarget == 27) antiJumpOverBox.enabled = true;
            yield return null;
        }
        jumpingState.stopJumpCylce = true;
        yield return new WaitForSecondsRealtime(1);

        for(int i = 0; i < 3; i++)
        {
            if(jumpingState.enabled == false) break;
            jumpingState.CallToJump(transform.position, jumpingState.targetPoints[jumpingState.targetPoints.Length-1].position, .5f, 2);
            GetComponent<SpriteRenderer>().flipX = true;
            yield return new WaitForSecondsRealtime(jumpingState.pauseInBetween + .5f);
        }
        yield return new WaitForSecondsRealtime(.5f);
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(1).transform.localPosition *= new Vector2(-1, 1); // This is what I get for wanting it to breath at the end
        transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = false;
        transform.GetChild(1).transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = false; // this will break if player resets -> put checkpoint at end
        transform.GetChild(1).transform.Rotate(0, 0, -90);
        jumpingState.enabled = false;
    }

    private void ResetStates()
    {
        jumpingState.pauseInBetween = startingJumpPause;
        jumpingState.enabled = false;
        antiJumpOverBox.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == PlayerController.instance.gameObject)
        {
            PlayerController.instance.DamagePlayer(PlayerController.instance.currentHealth);
        }
    }
}
