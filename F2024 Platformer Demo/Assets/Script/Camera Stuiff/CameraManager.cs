using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [Header("Camera Settings")]
    [Range(0, 100)]
    [SerializeField] int paddingX;
    [Range(0, 100)]
    [SerializeField] int paddingY;
    [Range(0,16f)]
    [SerializeField] float deadZone = 1f;
    [Min(0)]
    [SerializeField] float speedDampening;
    [SerializeField] float catchUpSpeed;
    [SerializeField] bool dynamicSpeed;

    [Header("Look Ahead Settings")]
    [SerializeField] bool useLookAhead;
    [SerializeField] Vector2 lookAheadDistance;
    [SerializeField] float lookAheadSpeed;
    

    

    [Header("Transition Settings")]
    [Min(0)]
    [SerializeField] float transitionTime = 1f;
    [SerializeField] bool transitioning;

    [Header("Debug")]
    [SerializeField] Vector3 targetPosition;
    [SerializeField] Vector2 lookAheadOffset;
    public Transform target;
    public static bool frozen;
    

    Camera cam
    {
        get { return Camera.main; }
    }
    Vector2 percentConvert
    {
        get
        {
            if(cam == null) { return Vector2.zero; }
            return new Vector2(cam.pixelWidth / 64 * (paddingX / 100f),
                               cam.pixelHeight / 64 * (paddingY / 100f));
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Camera has no target (Will be Broken)");
            return;
        }
        


        
        if (transitioning || frozen) return;
        PutTargetInBox(); // Movement Function

    }




    bool checkWithinBounds(Vector4 boundary, bool xAxisOnly = default, bool yAxisOnly = default, bool useDeadzone = default)
    {

        Vector2 targetPos = (Vector2)target.position + lookAheadOffset;

        
        Rect deadzoneConvert = new Rect(transform.position.x, transform.position.y, cam.pixelRect.size.x / 64 - deadZone*2, cam.pixelRect.size.y/64 - deadZone*2);
        Debug.DrawLine(deadzoneConvert.position, deadzoneConvert.position + Vector2.down * deadzoneConvert.height/2, Color.white);
        Debug.DrawLine(deadzoneConvert.position, deadzoneConvert.position + Vector2.left * deadzoneConvert.width/2, Color.white);


        if (xAxisOnly)
        {
            return targetPos.x < boundary.x || targetPos.x > boundary.z;
        }

        if(yAxisOnly)
        {
            return targetPos.y < boundary.y || targetPos.y > boundary.w;
        }

        if (useDeadzone)
        {
            return targetPos.x < deadzoneConvert.position.x - deadzoneConvert.width || targetPos.x > deadzoneConvert.position.x + deadzoneConvert.width ||
                   targetPos.y < deadzoneConvert.position.y - deadzoneConvert.height || targetPos.y > deadzoneConvert.position.y + deadzoneConvert.height;
        }

        return targetPos.x < boundary.x || targetPos.x > boundary.z ||
               targetPos.y < boundary.y || targetPos.y > boundary.w;

    }


    void PutTargetInBox()
    {
        if (!SceneController.Instance || !SceneController.Instance.activeChunk)
        {
            Debug.LogWarning("No Scene Controller to apply Clamp (Camera will no Move)");
            return;
        }

        Vector4 boundary = new Vector4(transform.position.x - percentConvert.x/2, 
                                       transform.position.y - percentConvert.y/2, 
                                       transform.position.x + percentConvert.x/2, 
                                       transform.position.y + percentConvert.y/2);

        Vector2 moveInput = Vector2.zero;




        if (target.position.x + lookAheadOffset.x < boundary.x || target.position.x + lookAheadOffset.x > boundary.z)
        {
            moveInput.x = target.position.x + lookAheadOffset.x > boundary.z ? .5f : -.5f;
        }
        if (target.position.y + lookAheadOffset.y < boundary.y || target.position.y + lookAheadOffset.y > boundary.w)
        {
            moveInput.y = target.position.y + lookAheadOffset.y > boundary.w ? .5f : -.5f;

        }


        if (useLookAhead && !checkWithinBounds(boundary,false,false,true)) // only run if not in Deadzone
        {
            if(PlayerController.instance.currentMomentum.x > 0f) 
            { 
                lookAheadOffset.x = Mathf.Lerp(lookAheadOffset.x, lookAheadDistance.x, lookAheadSpeed * Time.deltaTime);
            }
            else if (PlayerController.instance.currentMomentum.x < 0f)
            {
                lookAheadOffset.x = Mathf.Lerp(lookAheadOffset.x, -lookAheadDistance.x, lookAheadSpeed * Time.deltaTime);
            }
            if (PlayerController.instance.currentMomentum.y > 0f)
            {
                lookAheadOffset.y = Mathf.Lerp(lookAheadOffset.y, lookAheadDistance.y, lookAheadSpeed * Time.deltaTime);
            }
            else if (PlayerController.instance.currentMomentum.y < 0f)
            {
                lookAheadOffset.y = Mathf.Lerp(lookAheadOffset.y, -lookAheadDistance.y, lookAheadSpeed * Time.deltaTime);
            }

            lookAheadOffset.x = PlayerController.instance.currentMomentum.x == 0 ? 0f : lookAheadOffset.x;
            lookAheadOffset.y = PlayerController.instance.currentMomentum.y == 0 ? 0f : lookAheadOffset.y;

        }

        targetPosition = transform.position + (Vector3)moveInput;

        targetPosition = ClampMovement(targetPosition);

        if (dynamicSpeed)
        {
            catchUpSpeed = Vector2.Distance(target.position + (Vector3)lookAheadOffset, targetPosition) * 2;

            if (checkWithinBounds(boundary,false,false,true)) // Deadzone
            {
                Debug.Log("Target in Deadzone");
                catchUpSpeed *= PlayerController.instance.speed;
            }

        }

            transform.position = Vector3.Lerp(transform.position, targetPosition, (catchUpSpeed / (1+speedDampening)) * Time.deltaTime);

        
    }


    public Vector3 ClampMovement(Vector3 input)
    {
        input.x = Mathf.Clamp(targetPosition.x,
                                SceneController.Instance.activeChunk.transform.position.x + SceneController.Instance.activeChunk.offset.x - SceneController.Instance.activeChunk.cameraClampArea.x / 2f,
                                SceneController.Instance.activeChunk.transform.position.x + SceneController.Instance.activeChunk.offset.x + SceneController.Instance.activeChunk.cameraClampArea.x / 2f);
        input.y = Mathf.Clamp(targetPosition.y,
                                        SceneController.Instance.activeChunk.transform.position.y + SceneController.Instance.activeChunk.offset.y - SceneController.Instance.activeChunk.cameraClampArea.y / 2f,
                                        SceneController.Instance.activeChunk.transform.position.y + SceneController.Instance.activeChunk.offset.y + SceneController.Instance.activeChunk.cameraClampArea.y / 2f);

        return input;
    }


    public void TransitionCamera(Vector3 targetSpot)
    {
        if (transitioning) return;

        lookAheadOffset = Vector2.zero;
        transitioning = true;

        targetSpot.z = cam.transform.position.z;
        StartCoroutine(CameraTransition(targetSpot));
    }

    IEnumerator CameraTransition(Vector3 targetSpot)
    {
        float timePassed = 0;
        Vector3 start = transform.position;

        if (targetSpot.y > start.y + 10) // 2 = 1 block so if 5 blocks above current camera causes boost 
        {
            PlayerController.instance.JumpCall(); // add extra velocity up if coming from chunk below
            Debug.Log("added Force for Chunk");
        }

        PlayerController.instance.FreezePlayer();

        while (true)
        {
            timePassed += Time.deltaTime;

            transform.position = Vector3.Lerp(start, targetSpot, timePassed / transitionTime);

            if (Vector2.Distance(transform.position,targetSpot) <= 0.1f) break;

            yield return null;
        }

        PlayerController.instance.UnFreezePlayer();

        transitioning = false;
    }

    private void OnDrawGizmosSelected()
    {
        #region Camera Linger Settings


        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, percentConvert);

        #endregion

        // Deadzone
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, cam.pixelRect.size / 64 - Vector2.one * deadZone);


        Gizmos.DrawWireSphere(targetPosition, .25f);


        // Look Ahead
        if(!useLookAhead) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(target.transform.position + (Vector3)lookAheadOffset, .25f);

    }


}
