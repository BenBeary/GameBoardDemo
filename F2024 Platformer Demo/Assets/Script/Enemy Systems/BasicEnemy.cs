using System.Collections;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    [SerializeField] GameObject dustPrefab;
    [SerializeField] bool flipSpriteTowardTarget;

    [Header("Movement")]
    [Tooltip("Progress till reaching max height")]
    [SerializeField] AnimationCurve heightCurve;

    [SerializeField] float jumpHeight;

    [Tooltip("Time jump takes to finish")]
    public float timeToJump;

    [Tooltip("Time in between jumps")]
    public float pauseInBetween;

    [Space(15)]
    public Transform[] targetPoints;

    [Header("Killed Player")]
    [SerializeField] bool useAnim;
    [SerializeField] Animator anim;
    [SerializeField] float hideDelay = 1.5f;

    [Header("Background Prop")]
    [SerializeField] bool killOnDestination;

    [Header("Extras")]
    public ColorVarients[] dotColors;
    [HideInInspector] public ColorVarients colorSelected;
    public bool colorOnColorViolence;

    [Header("Debug")]
    public bool finishedJump = true;
    public bool backOnGround;
    public bool stopJumpCylce;
    public int currentTarget = 0;
    Transform jumpPoint;

    private void Start()
    {
        if (transform.childCount == 0) Debug.LogError("No Child Jump Point on " + gameObject.name);
        jumpPoint = transform.GetChild(0);
        anim.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (!finishedJump || stopJumpCylce) return;

        float jumpDist = Mathf.Abs(transform.position.x - jumpPoint.position.x);
        float targDist = Mathf.Abs(transform.position.x -  targetPoints[currentTarget].position.x);
        float jumpPointToTarget = Mathf.Abs(jumpPoint.position.x - targetPoints[currentTarget].position.x);

        if(jumpPointToTarget > targDist) jumpPoint.localPosition = -jumpPoint.localPosition; // if the jumpoint is further than Object, flip direction

        if (targDist <= jumpDist)
        {
            StartCoroutine(Curve(transform.position, targetPoints[currentTarget].position,timeToJump,jumpHeight));
            currentTarget++;
            if (currentTarget > targetPoints.Length - 1)
            {
                if(killOnDestination)
                {
                    Destroy(gameObject);
                    return;
                }
                currentTarget = 0;
            }
        }
        else // Using Jump point instead of target
        {
            
            StartCoroutine(Curve(transform.position,jumpPoint.position,timeToJump,jumpHeight));
        }

        finishedJump = false;
    }

    public void turnOnPathing()
    {
        stopJumpCylce = false;
    }


    public IEnumerator Curve(Vector2 start, Vector2 target, float timeToJump, float jumpHeight)
    {
        float timePassed = 0f;
        Vector2 end = target;
        backOnGround = false;

        // flips to face target (Default View is Left)
        if(flipSpriteTowardTarget) GetComponent<SpriteRenderer>().flipX = (targetPoints[currentTarget].position.x > transform.position.x) ? true : false;


        while (timePassed < timeToJump)
        {
            timePassed += Time.deltaTime;

            float linearT = timePassed / timeToJump;
            float heightT = heightCurve.Evaluate(linearT);

            float height = Mathf.Lerp(0f, jumpHeight, heightT);

            transform.position = Vector2.Lerp(start, end, linearT) + new Vector2(0, height);
            yield return null;
        }
        GameObject temp = Instantiate(dustPrefab);
        temp.transform.position = transform.position;
        backOnGround = true;
        yield return new WaitForSecondsRealtime(pauseInBetween);
        finishedJump = true;
    }

    public void CallToJump(Vector2 start, Vector2 target, float jumpTime, float jumpHeight)
    {
        StartCoroutine(Curve(start, target, jumpTime, jumpHeight));
    }

    private void OnDisable()
    {
        backOnGround = true;
        finishedJump = true;
        currentTarget = 0;
        stopJumpCylce = false;
    }

    IEnumerator playerDeathTrigger()
    {
        if (useAnim)
        {
            anim.gameObject.SetActive(true);
            anim.Play("Death");
        }

        yield return new WaitForSeconds(hideDelay);

        if(useAnim) anim.gameObject.SetActive(false);
    }

    public void killEnemy()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(playerDeathTrigger());
        }
        if (collision.GetComponent<BasicEnemy>() && collision.GetComponent<BasicEnemy>().colorSelected != colorSelected && stopJumpCylce)
        {
            collision.GetComponent<BasicEnemy>().killEnemy();
            StartCoroutine(playerDeathTrigger());
        }
    }


    [System.Serializable]
    public enum ColorVarients
    {
        Red,
        Green,
        Blue,
        Yellow,
        White,
        Black
    }
}

