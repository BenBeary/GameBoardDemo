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
    [Min(0)]
    [SerializeField] float speedDampening;
    [SerializeField] float catchUpSpeed;
    [SerializeField] bool dynamicSpeed;
    [SerializeField] float lookAheadDistance;
    [SerializeField] float lookAheadSpeed;
    public static bool frozen;
    [Range(0,16f)]
    [SerializeField] float deadZone = 1f;

    [Header("Transition Settings")]
    [Min(0)]
    [SerializeField] float transitionTime = 1f;
    [SerializeField] bool transitioning;

    [Header("Debug")]
    [SerializeField] Vector3 clampedMove;
    public Transform target;
    
    Vector3 targetLastPos;

    Camera cam
    {
        get { return Camera.main; }
    }
    Vector2 percentConvert
    {
        get
        {
            if(cam == null) { return Vector2.zero; }
            return new Vector2(cam.pixelWidth / 32 / 4 * (paddingX / 100f),
                               cam.pixelHeight / 32 / 4 * (paddingY / 100f));
        }
    }



    private void Update()
    {
        if (target == null)
        {
            Debug.LogWarning("Camera has no target (Will be Broken)");
            return;
        }



        // check if target is in bounds
        if (transitioning) return;
        PutTargetInBox();

    }


    private void FixedUpdate()
    {
        targetLastPos = transform.position;
    }


    void PutTargetInBox()
    {
        if (!SceneController.Instance || !SceneController.Instance.activeChunk)
        {
            Debug.LogWarning("No Scene Controller to apply Clamp (Camera will no Move)");
            return;
        }


        Vector2 moveInput = Vector2.zero;
        Vector4 boundary = new Vector4(transform.position.x - percentConvert.x, transform.position.y - percentConvert.y, transform.position.x + percentConvert.x, transform.position.y + percentConvert.y);


        // Outside of border box

        if (target.position.x < boundary.x || target.position.x > boundary.z)
        {
            moveInput.x = target.position.x > boundary.z ? 1f : -1f;

        }
        if (target.position.y < boundary.y || target.position.y > boundary.w)
        {
            moveInput.y = target.position.y > boundary.w ? 1f : -1f;

        }





        // movement with current chunks restrictions

        clampedMove = transform.position + (Vector3)moveInput;

        clampedMove = ClampMovement(clampedMove);


        if (dynamicSpeed)
        {
            catchUpSpeed = Vector2.Distance(target.position, transform.position) * 2;

            if (target.position.x < boundary.x - cam.pixelWidth / 128 + deadZone || target.position.x > boundary.z + cam.pixelWidth / 128 - deadZone ||
                target.position.y < boundary.y - cam.pixelHeight / 128 + deadZone || target.position.y > boundary.w + cam.pixelHeight / 128 - deadZone) // Player is out of camera
            {
                catchUpSpeed *= 10f;
            }

        }

        transform.position = Vector3.Lerp(transform.position, clampedMove, (catchUpSpeed / (1+speedDampening)) * Time.deltaTime);
        
    }


    public Vector3 ClampMovement(Vector3 input)
    {
        input.x = Mathf.Clamp(clampedMove.x,
                                SceneController.Instance.activeChunk.transform.position.x - SceneController.Instance.activeChunk.cameraClampArea.x / 2f,
                                SceneController.Instance.activeChunk.transform.position.x + SceneController.Instance.activeChunk.cameraClampArea.x / 2f);
        input.y = Mathf.Clamp(clampedMove.y,
                                        SceneController.Instance.activeChunk.transform.position.y - SceneController.Instance.activeChunk.cameraClampArea.y / 2f,
                                        SceneController.Instance.activeChunk.transform.position.y + SceneController.Instance.activeChunk.cameraClampArea.y / 2f);

        return input;
    }


    public void TransitionCamera(Vector3 targetSpot)
    {
        if (transitioning) return;
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

       


        Vector3 horizontalPoint = new Vector3(transform.position.x + percentConvert.x,
                                              transform.position.y + (cam.pixelHeight / 32) / 4,
                                              transform.position.y - (cam.pixelHeight / 32) / 4);

        Vector3 verticalPoint = new Vector3(transform.position.x + (cam.pixelWidth / 32) / 4,
                                            transform.position.y + percentConvert.y,
                                            transform.position.x - (cam.pixelWidth / 32) / 4);
                                            



        // Camera Move Bounds
        Gizmos.color = Color.blue;
        Gizmos.DrawLine((Vector2)horizontalPoint, new Vector2(horizontalPoint.x,horizontalPoint.z));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine((Vector2)verticalPoint, new Vector2(verticalPoint.z,verticalPoint.y));

        horizontalPoint.x = transform.position.x - percentConvert.x;
        verticalPoint.y = transform.position.y - percentConvert.y;
        

        Gizmos.color = Color.blue;
        Gizmos.DrawLine((Vector2)horizontalPoint, new Vector2(horizontalPoint.x, horizontalPoint.z));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine((Vector2)verticalPoint, new Vector2(verticalPoint.z, verticalPoint.y));
        #endregion

        // Deadzone
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, cam.pixelRect.size / 32 / 2 - Vector2.one * deadZone);

    }


}
