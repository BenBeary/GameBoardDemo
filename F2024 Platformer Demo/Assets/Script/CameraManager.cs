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

        clampedMove.x = Mathf.Clamp(clampedMove.x,
                                        SceneController.Instance.activeChunk.transform.position.x - SceneController.Instance.activeChunk.cameraClampArea.x / 2f,
                                        SceneController.Instance.activeChunk.transform.position.x + SceneController.Instance.activeChunk.cameraClampArea.x / 2f);
        clampedMove.y = Mathf.Clamp(clampedMove.y,
                                        SceneController.Instance.activeChunk.transform.position.y - SceneController.Instance.activeChunk.cameraClampArea.y / 2f,
                                        SceneController.Instance.activeChunk.transform.position.y + SceneController.Instance.activeChunk.cameraClampArea.y / 2f);


        if (dynamicSpeed)
        {
            catchUpSpeed = Vector2.Distance(target.position, transform.position) * 2;

            if (target.position.x < boundary.x - cam.pixelWidth / 128 + deadZone || target.position.x > boundary.z + cam.pixelWidth / 128 - deadZone ||
                target.position.y < boundary.y - cam.pixelHeight / 128 + deadZone || target.position.y > boundary.w + cam.pixelHeight / 128 - deadZone) // Player is out of camera
            {
                catchUpSpeed *= 10f;
                Debug.Log("Player Out of Camera View");
            }

        }

        transform.position = Vector3.Lerp(transform.position, clampedMove, (catchUpSpeed / (1+speedDampening)) * Time.deltaTime);
        
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

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, cam.pixelRect.size / 32 / 2 - Vector2.one * deadZone);

    }


}
