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
    [SerializeField] float catchUpSpeed;
    public static bool frozen;

    [Header("Debug")]
    //public bool dontMove;
    public Transform target;
    Camera cam
    {
        get { return GetComponent<Camera>(); }
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


    void PutTargetInBox()
    {
        if(!SceneController.Instance || !SceneController.Instance.activeChunk) 
        {
            Debug.LogWarning("No Scene Controller to apply Clamp (Camera will no Move)");
            return; 
        }


        Vector2 moveInput = Vector2.zero;

        // Outside of border box

        if(target.position.x < transform.position.x - percentConvert.x || target.position.x > transform.position.x + percentConvert.x)
        {
            moveInput.x = -Vector2.Distance(target.position, transform.position) * catchUpSpeed / 10f;
            if (target.position.x > transform.position.x + percentConvert.x) moveInput.x *= -1f;
           
        }
        if (target.position.y < transform.position.y - percentConvert.y || target.position.y > transform.position.y + percentConvert.y)
        {
            moveInput.y = -Vector2.Distance(target.position, transform.position) * catchUpSpeed / 10f;
            if (target.position.y > transform.position.y + percentConvert.y) moveInput.y *= -1f;
            
        }


        // movement with current chunks restrictions

        Vector3 clampedMove = transform.position + (Vector3)moveInput;

        clampedMove.x = Mathf.Clamp(clampedMove.x,
                                        SceneController.Instance.activeChunk.transform.position.x - SceneController.Instance.activeChunk.cameraClampArea.x / 2f,
                                        SceneController.Instance.activeChunk.transform.position.x + SceneController.Instance.activeChunk.cameraClampArea.x / 2f);
        clampedMove.y = Mathf.Clamp(clampedMove.y,
                                        SceneController.Instance.activeChunk.transform.position.y - SceneController.Instance.activeChunk.cameraClampArea.y / 2f,
                                        SceneController.Instance.activeChunk.transform.position.y + SceneController.Instance.activeChunk.cameraClampArea.y / 2f);
        transform.Translate(clampedMove - transform.position);
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

    }


}
