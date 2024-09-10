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
    [SerializeField] Transform[] targetPoints;


    [Header("Debug")]
    [SerializeField] bool finishedJump = true;
    public bool backOnGround;
    public int currentTarget = 0;
    Transform jumpPoint;

    private void Start()
    {
        if (transform.childCount == 0) Debug.LogError("No Child Jump Point on " + gameObject.name);
        jumpPoint = transform.GetChild(0);
    }

    private void FixedUpdate()
    {
        if (!finishedJump) return;

        float jumpDist = Vector2.Distance(transform.position, jumpPoint.position);
        float targDist = Vector2.Distance(transform.position, targetPoints[currentTarget].position);
        float jumpPointToTarget = Vector2.Distance(jumpPoint.position, targetPoints[currentTarget].position);

        if(jumpPointToTarget > targDist) jumpPoint.localPosition = -jumpPoint.localPosition; // if the jumpoint is further than Object, flip direction

        if (targDist <= jumpDist)
        {
            StartCoroutine(Curve(transform.position, targetPoints[currentTarget].position));
            currentTarget++;
            if (currentTarget > targetPoints.Length - 1) currentTarget = 0;
        }
        else // Using Jump point instead of target
        {
            
            StartCoroutine(Curve(transform.position,jumpPoint.position));
        }

        finishedJump = false;
    }

    public IEnumerator Curve(Vector2 start, Vector2 target)
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

    private void OnDisable()
    {
        backOnGround = true;
        finishedJump = true;
        currentTarget = 0;
    }
}

