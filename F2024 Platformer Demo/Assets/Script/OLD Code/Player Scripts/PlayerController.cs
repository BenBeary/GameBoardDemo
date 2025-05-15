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
    [SerializeField] GameObject deadPrefab;

    [Header("Movement")]
    public int dotCount = 1;
    public int maxDots = 1;
    public float speed = 5f;


    [Header("Jump System")]
    [SerializeField] Material playerMat;
    [SerializeField] Color dotColorOnDoubleJump = Color.blue;
    Color baseDotColor;
    public float dotReplenishSpeed = 1f;
    [SerializeField] float jumpForce = 3f;
    [SerializeField] GameObject dustPrefab;
    public float maxVelocity = 10f;


    [Header("Sliding (Debug)")]
    public BasicEnemy.ColorVarients currentSlideColor = BasicEnemy.ColorVarients.White; // White = none
    [SerializeField] SkinColorSwatches skinColorSwatch;
    public int slidesLeft;
    public bool sliding;


    [Header("Wall Slide")]
    [SerializeField] float wallSlideSpeed = 2f;
    [SerializeField] float wallSlideSlow = 2.5f;
    [SerializeField] float wallJumpCD = .2f;
    [SerializeField] GameObject halfDustPrefab;
    [SerializeField] float wallDustSpawnrate = 0.1f;
    bool dustSpawnCD;

    [Header("Debug")]
    [SerializeField] bool dotAnim;
    [SerializeField] float currentAnimTime;
    public int currentHealth = 6;
    public bool grounded;
    [SerializeField] bool leftWallHang, rightWallHang, checkHead;
    public bool doubleJump = true;
    public bool hasInputPaused;


    Animator animMan;
    Rigidbody2D rb;
    public Vector2 motionInput;
    float animSpeed;
    int oldDot = 1;
    bool replenishing;
    bool wallJump;
    bool hasJumped;
    bool isDying;
    string animationsToPlay;
    bool landed;

    Vector2 storedVelocity;
    Vector2 storedForce;
    public Vector2 currentMomentum;

    private void Awake()
    {
        if(instance  == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        currentHealth = Maxhealth;
        rb = GetComponent<Rigidbody2D>();
        animMan = GetComponent<Animator>();
        
        animSpeed = animMan.speed;
        animMan.speed = 0f;
        baseDotColor = playerMat ? playerMat.GetColor("_DotColor") : Color.black;
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Alpha1) && Input.GetKey(KeyCode.Alpha2))
        {
            Debug.Log("Application Closing");
            Application.Quit();
        }
        if (rb.velocity.magnitude > maxVelocity) rb.velocity = rb.velocity.normalized * maxVelocity;

        if(!isDying && !hasInputPaused) MovementManager();
        currentMomentum = motionInput + Vector2.up * rb.velocity.y;
        AnimManager();
        BaseColorChanger();
    }

    private void OnDisable()
    {
        playerMat.SetColor("_DotColor", baseDotColor);
        currentSlideColor = BasicEnemy.ColorVarients.White;
        BaseColorChanger();
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
        GameManager.Instance.playerDeathCount++;

        #region Corpse Spawning
        GetComponent<SpriteRenderer>().enabled = false;
        rb.bodyType = RigidbodyType2D.Static;

        GameObject corpse = Instantiate(deadPrefab);
        corpse.transform.position = transform.position;
        corpse.GetComponent<Animator>().runtimeAnimatorController = animMan.runtimeAnimatorController;
        corpse.GetComponent<Animator>().Play(animationsToPlay);
        corpse.GetComponent<Animator>().speed = 1.5f;
        corpse.GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce * 1000 + Vector2.left * (motionInput.x*1000));
        Destroy(corpse, 2);

        #endregion

        yield return new WaitForSeconds(2f);
        GetComponent<SpriteRenderer>().enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        //GameManager.Instance?.clearCameraShake();
        resetToCheckpoint();

        isDying = false;
    }
    
    private void delayedReset()
    {
        playerReset?.Invoke();
    }

    public void resetToCheckpoint()
    {
        transform.position = checkPoint.position;
        currentHealth = Maxhealth;
        currentSlideColor = BasicEnemy.ColorVarients.White;
        PlayerSlideColorChange.currentlySelected = null;
        playerReset?.Invoke();
    }

    void wallSlideDustEffect()
    {
        if (rb.velocity.y < 0 && !dustSpawnCD) // Wall Slide Effect
        {
            GameObject temp = Instantiate(halfDustPrefab);
            StartCoroutine(wallSlideDustDelay());
            if (rightWallHang)
            {
                temp.transform.position = (Vector2)transform.position + GetComponent<SpriteRenderer>().size + Vector2.left * GetComponent<SpriteRenderer>().size.x * .5f;
            }
            else
            {
                temp.transform.position = (Vector2)transform.position + GetComponent<SpriteRenderer>().size + Vector2.left * (GetComponent<SpriteRenderer>().size.x * 1.5f);
                temp.GetComponent<SpriteRenderer>().flipY = true;
            }
        }
    }
    IEnumerator wallSlideDustDelay()
    {
        dustSpawnCD = true;

        if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand))
        {
            yield return new WaitForSeconds(wallDustSpawnrate + wallSlideSlow / 20f);
        }
        else yield return new WaitForSeconds(wallDustSpawnrate);


        dustSpawnCD = false;
    }

    void BaseColorChanger()
    {
        if(slidesLeft == 0) currentSlideColor = BasicEnemy.ColorVarients.White;

        switch (currentSlideColor)
        {
            case BasicEnemy.ColorVarients.Red:
                playerMat.SetColor("_MainColor", skinColorSwatch.colorSwatches[0].mainColor);
                playerMat.SetColor("_OutlineColor", skinColorSwatch.colorSwatches[0].outlineColor);
                break;
            case BasicEnemy.ColorVarients.Green:
                playerMat.SetColor("_MainColor", skinColorSwatch.colorSwatches[1].mainColor);
                playerMat.SetColor("_OutlineColor", skinColorSwatch.colorSwatches[1].outlineColor);
                break;
            case BasicEnemy.ColorVarients.Blue:
                playerMat.SetColor("_MainColor", skinColorSwatch.colorSwatches[2].mainColor);
                playerMat.SetColor("_OutlineColor", skinColorSwatch.colorSwatches[2].outlineColor);
                break;
            case BasicEnemy.ColorVarients.Yellow:
                playerMat.SetColor("_MainColor", skinColorSwatch.colorSwatches[3].mainColor);
                playerMat.SetColor("_OutlineColor", skinColorSwatch.colorSwatches[3].outlineColor);
                break;
            default:
                playerMat.SetColor("_MainColor", skinColorSwatch.colorSwatches[4].mainColor);
                playerMat.SetColor("_OutlineColor", skinColorSwatch.colorSwatches[4].outlineColor);
                break;
        }
    }


    private void MovementManager()
    {
        //Debug.DrawRay(transform.position, Vector2.down * .1f, Color.red);
        Debug.DrawRay(transform.position + (Vector3.left * 0.35f) + Vector3.up * .2f, Vector2.left * .1f, Color.yellow);
        Debug.DrawRay(transform.position + (Vector3.right * 0.35f) + Vector3.up * .2f, Vector2.right * .1f, Color.yellow);
        
        motionInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        
        
        if(motionInput.magnitude != 0 && !GameManager.Instance.startTimer)
        {
            Debug.Log("Timer Has Started!");
            GameManager.Instance.startTimer = true;
        }
        

        // Raycasts for ground / Wall Movements
        RaycastHit2D groundHit = Physics2D.BoxCast(transform.position, new Vector2(GetComponent<SpriteRenderer>().sprite.bounds.size.x * .8f, 0.1f), 0,Vector2.down, 0.1f, LayerMask.GetMask("Ground", "Wall"));
        RaycastHit2D leftWallHit = (Physics2D.Raycast(transform.position + (Vector3.left * 0.35f) + Vector3.up * .2f, Vector2.left, .1f, LayerMask.GetMask("Ground", "Wall")));
        RaycastHit2D rightWallHit = (Physics2D.Raycast(transform.position + (Vector3.right * 0.35f) + Vector3.up * .2f, Vector2.right, .1f, LayerMask.GetMask("Ground", "Wall")));
        RaycastHit2D headHit = Physics2D.BoxCast(transform.position + Vector3.up * (GetComponent<SpriteRenderer>().size.y /2), GetComponent<SpriteRenderer>().size / 4, 0, Vector2.up, 0.1f, LayerMask.GetMask("Ground"));


        grounded = groundHit.collider != null && !groundHit.collider.isTrigger;
        leftWallHang = leftWallHit.collider != null && !leftWallHit.collider.isTrigger;
        rightWallHang = rightWallHit.collider != null && !rightWallHit.collider.isTrigger;
        checkHead = headHit.collider != null && !headHit.collider.isTrigger;

        //Debug.Log(headHit.collider + " | " + checkHead + " | " + grounded);


        #region Squish Check

        if (checkHead)
        {
            Debug.Log("Player is Squished");
            DamagePlayer(currentHealth);
        }

        #endregion

        #region Jumping

        if(doubleJump && dotCount > 1)
        {
            playerMat.SetColor("_DotColor", baseDotColor);
        }
        else
        {
            playerMat.SetColor("_DotColor", dotColorOnDoubleJump);
        }

        if (grounded) 
        {
            doubleJump = true;
            if (!landed)
            {
                landed = true;
                SoundManager.instance.playSound("Player", "Land", .2f);
                GameObject temp = Instantiate(dustPrefab);
                temp.transform.position = transform.position;
            }
        }
        else if(!grounded && landed)
        {
            landed = false;
            // Play Jump SoundEffect here? idk
        }


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
        if (hasJumped && rb.velocity.y == 0) // ADD COYOTE TIME HERE
        {
            grounded = false;
            hasJumped = false;

        }
        #endregion

        #region Wall Jumping

        if (rightWallHang && motionInput.x > 0 || leftWallHang && motionInput.x < 0)
        {
            motionInput = Vector2.zero; // stop pushing into wall and allow to leave wall 

        }


        if (rightWallHang && !grounded || leftWallHang && !grounded)
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) // stick to the wall longer
            {
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed + wallSlideSlow, float.MaxValue));
            }
            else rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));


            wallSlideDustEffect();
            



            if (Input.GetButtonDown("Jump") && dotCount > 1)
            {
                if (rightWallHang)
                {
                    rb.velocity = Vector2.zero; // Jumping Reset Velocity
                    rb.AddForce(Vector2.up * jumpForce * 1000 + Vector2.left * speed * 250);
                    StartCoroutine(wallJumpVelocityCooldown());
                }

                else if (leftWallHang)
                {
                    rb.velocity = Vector2.zero;
                    rb.AddForce(Vector2.up * jumpForce * 1000 + Vector2.right * speed * 250);
                    StartCoroutine(wallJumpVelocityCooldown());
                }
                dotCount--;
            }
            if(dotCount == 1) playerMat.SetColor("_DotColor", dotColorOnDoubleJump);
        }

        if (wallJump && motionInput.x < 0 && rb.velocity.x > 0 || wallJump && motionInput.x > 0 && rb.velocity.x < 0)
        {
            motionInput = Vector2.zero; // Cancel movement input to stop player from going back to same wall
        }
        else if (!wallJump && motionInput.x != 0 && rb.velocity.x != 0) 
        {
            rb.velocity = new Vector2(0,rb.velocity.y); 
        }
        #endregion



        transform.Translate(motionInput * speed * Time.deltaTime);

        if (motionInput.x == 0) rb.velocity.Set(0, rb.velocity.y);

    }

    IEnumerator wallJumpVelocityCooldown() // cant stop jumping away from wall
    {
        wallJump = true;
        yield return new WaitForSecondsRealtime(wallJumpCD);
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


    public void FreezePlayer()
    {
        hasInputPaused = true;
        storedVelocity = rb.velocity;
        storedForce = rb.totalForce;

        rb.velocity = Vector2.zero;
        rb.totalForce = Vector2.zero;
        rb.gravityScale = 0;
        animMan.enabled = false;
        
    }

    public void UnFreezePlayer()
    {
        hasInputPaused = false;
        rb.gravityScale = 2f;
        rb.velocity = storedVelocity;
        rb.totalForce = storedForce;
        animMan.enabled = true;
       
    }

    public void ClearPlayerMomentum()
    {
        rb.velocity = Vector2.zero;
        rb.totalForce = Vector2.zero;
    }


    public void JumpCall(Vector2 forceDir = default)
    {
        if (forceDir == default) forceDir = Vector2.up;
        rb.velocity = Vector2.zero;
        rb.AddForce(forceDir * jumpForce * 1000);
        // Debug.Log("Jump! ");
    }

    private void AnimManager()
    {
        animationsToPlay = dotCount.ToString() + "_Walk";
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
        if (collision.CompareTag("Checkpoint"))
        {
            if (collision.transform == checkPoint) return;

            if (checkPoint != null) checkPoint.GetComponent<CheckPoint_Animation>().MoveFlag(true);
            collision.gameObject.GetComponent<CheckPoint_Animation>().MoveFlag(false);
            checkPoint = collision.transform;
            currentHealth = Maxhealth;
         
        }
        if (collision.CompareTag("OutOfBounds"))
        {
            rb.velocity = Vector2.zero;
            resetToCheckpoint();
            resetToCheckpoint();
        }
        if (collision.CompareTag("DeathBoxes"))
        {
            if(collision.GetComponent<BasicEnemy>() && collision.GetComponent<BasicEnemy>().colorSelected == currentSlideColor) return;
            DamagePlayer(currentHealth);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector2(GetComponent<SpriteRenderer>().sprite.bounds.size.x, 0.1f));
    }
}
