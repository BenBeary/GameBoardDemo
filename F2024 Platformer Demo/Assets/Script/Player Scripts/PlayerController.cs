using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [Header("Health")]
    public int Maxhealth = 6;
    public Transform checkPoint;
    public static Action playerReset;


    [Header("Movement")]
    public int dotCount = 1;
    public int maxDots = 1;
    public float speed = 5f;
    [Space(10)]
    public float dotReplenishSpeed = 1f;
    [SerializeField] float jumpForce = 3f;
    [SerializeField] GameObject dustPrefab;
    [Space(10)]
    [SerializeField] float wallSlideSpeed = 2f;



    [Header("Debug")]
    [SerializeField] bool dotAnim;
    [SerializeField] float currentAnimTime;
    public int currentHealth = 6;
    [SerializeField] bool grounded;
    [SerializeField] bool leftWallHang, rightWallHang;
    [SerializeField] bool doubleJump = true;
    public bool hasInputPaused;


    Animator animMan;
    Rigidbody2D rb;
    Vector2 motionInput;
    float animSpeed;
    int oldDot = 1;
    bool replenishing;
    bool wallJump;
    bool hasJumped;
    bool isDying;

    private void Awake()
    {
        if(instance  == null) instance = this;
        else Destroy(gameObject);

        currentHealth = Maxhealth;
        rb = GetComponent<Rigidbody2D>();
        animMan = GetComponent<Animator>();
        
        animSpeed = animMan.speed;
        animMan.speed = 0f;

    }

    private void Update()
    {
        if(!isDying && !hasInputPaused) MovementManager();
        AnimManager();
    }

    public void DamagePlayer(int damage)
    {
        if (isDying) return;

        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            currentHealth = 1;
            StartCoroutine(PlayerDeath());
        }
    }
    private IEnumerator PlayerDeath()
    {
        isDying = true;
        yield return new WaitForSecondsRealtime(1f);
        resetToCheckpoint();
        isDying = false;
    }
    
    public void resetToCheckpoint()
    {
        transform.position = checkPoint.position;
        currentHealth = Maxhealth;
        playerReset?.Invoke();
    }


    private void MovementManager()
    {
        Debug.DrawRay(transform.position, Vector2.down * .1f, Color.red);
        Debug.DrawRay(transform.position + (Vector3.left * 0.35f) + Vector3.up * .2f, Vector2.left * .1f, Color.yellow);
        Debug.DrawRay(transform.position + (Vector3.right * 0.35f) + Vector3.up * .2f, Vector2.right * .1f, Color.yellow);
        
        motionInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        // Raycasts for ground / Wall Movements
        grounded = (Physics2D.Raycast(transform.position, Vector2.down, .1f, LayerMask.GetMask("Ground")));
        leftWallHang = (Physics2D.Raycast(transform.position + (Vector3.left * 0.35f) + Vector3.up * .2f, Vector2.left, .1f, LayerMask.GetMask("Wall")));
        rightWallHang = (Physics2D.Raycast(transform.position + (Vector3.right * 0.35f) + Vector3.up * .2f, Vector2.right, .1f, LayerMask.GetMask("Wall")));



        #region Jumping
        if (grounded) doubleJump = true;


        if (grounded && !replenishing && dotCount < maxDots ) 
        {
            StartCoroutine(replenishDot());
        }

        if( Input.GetButtonDown("Jump") && grounded) // Regular Jump
        {
            hasJumped = true;
            JumpCall();
            replenishing = false;
        }
        else if(Input.GetButtonDown("Jump") && doubleJump && !grounded && !leftWallHang && !rightWallHang && dotCount > 1) // Double Jump
        {
            rb.velocity = Vector3.zero;
            JumpCall();
            doubleJump = false;
            dotCount--;
        }
        if (hasJumped && rb.velocity.y == 0)
        {
            hasJumped = false;
            GameObject temp = Instantiate(dustPrefab);
            temp.transform.position = transform.position;
        }
        #endregion

        #region Wall Jumping



        if (rightWallHang && !grounded || leftWallHang && !grounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
            if (rightWallHang && motionInput.x > 0) motionInput = Vector2.zero; // stop pushing into wall and allow to leave wall 
            else if(leftWallHang && motionInput.x < 0) motionInput = Vector2.zero;

            if (Input.GetButtonDown("Jump") && dotCount > 1)
            {
                if (rightWallHang)
                {
                    rb.velocity = Vector2.zero; // Jumping Reset Velocity
                    rb.AddForce(Vector2.up * jumpForce * 1000 + Vector2.left * speed * 250);
                    StartCoroutine(wallJumpVelocityCooldown());
                }

                else if(leftWallHang)
                {
                    rb.velocity = Vector2.zero;
                    rb.AddForce(Vector2.up * jumpForce * 1000 + Vector2.right * speed * 250);
                    StartCoroutine(wallJumpVelocityCooldown());
                }
                dotCount--;
            }
        }

        if (wallJump && motionInput.x < 0 && rb.velocity.x > 0) motionInput = Vector2.zero; // Cancel movement input to stop player from going back to same wall
        else if(wallJump && motionInput.x > 0 && rb.velocity.x < 0) motionInput = Vector2.zero;
        else if(!wallJump && motionInput.x != 0 && rb.velocity.x != 0) rb.velocity = new Vector2(0,rb.velocity.y); 
        #endregion


        transform.Translate(motionInput * speed * Time.deltaTime);
    }

    IEnumerator wallJumpVelocityCooldown() // cant stop jumping away from wall
    {
        wallJump = true;
        yield return new WaitForSecondsRealtime(.2f);
        wallJump = false;
    }

    IEnumerator replenishDot()
    {
        replenishing = true;
        while(dotCount < maxDots && grounded)
        {
            yield return new WaitForSecondsRealtime(dotReplenishSpeed);
            if (!grounded) break;
            dotCount++;
            dotCount = Mathf.Clamp(dotCount, 0, maxDots);

        }
        replenishing = false;
    }

    private void JumpCall()
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.up * jumpForce * 1000);
        Debug.Log("Jump! ");
    }

    private void AnimManager()
    {
        string animationsToPlay = dotCount.ToString() + "_Walk";
        currentAnimTime = Mathf.Round((animMan.GetCurrentAnimatorStateInfo(0).normalizedTime % (int)animMan.GetCurrentAnimatorStateInfo(0).normalizedTime) * 10) / 10;
        if (motionInput.x < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            animMan.speed = animSpeed;
            if (!grounded) animMan.speed = 3f;
        }
        else if(motionInput.x > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            animMan.speed = animSpeed;
            if (!grounded) animMan.speed = 3f;
        }
        else
        {
            if (!grounded && !leftWallHang && !rightWallHang)
            {
                animMan.speed = .5f;
                if(rb.velocity.x != 0) GetComponent<SpriteRenderer>().flipX = (rb.velocity.x < 0) ? true : false;
            }
            else if (currentAnimTime == .5f || currentAnimTime == 0)
            {
                animMan.speed = 0;
                if(currentAnimTime == .5) // this is what I get for having even number animations :(
                {
                    animMan.Play(animationsToPlay, 0, .6f);
                }
            }
        }




        
        if(oldDot != dotCount && animMan.GetCurrentAnimatorClipInfo(0)[0].clip.name != animationsToPlay)
        {
            animMan.Play(animationsToPlay, 0, animMan.GetCurrentAnimatorStateInfo(0).normalizedTime);
            oldDot = dotCount;
        }
        else if(animMan.GetCurrentAnimatorClipInfo(0)[0].clip.name != animationsToPlay)
        {
            animMan.Play(animationsToPlay);
        }




    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            DamagePlayer(1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Checkpoint"))
        {
            if (collision.transform == checkPoint) return;

            if (checkPoint != null) checkPoint.GetComponent<CheckPoint_Animation>().MoveFlag(true);
            collision.gameObject.GetComponent<CheckPoint_Animation>().MoveFlag(false);
            checkPoint = collision.transform;
            currentHealth = Maxhealth;
         
        }
        if (collision.gameObject.CompareTag("OutOfBounds"))
        {
            rb.velocity = Vector2.zero;
            resetToCheckpoint();
            resetToCheckpoint();
        }
    }
}
