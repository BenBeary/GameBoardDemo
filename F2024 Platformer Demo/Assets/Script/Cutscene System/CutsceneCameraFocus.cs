using UnityEngine;
using UnityEngine.Events;

public class CutsceneCameraFocus : MonoBehaviour
{
    [Header("MovementSettings")]
    public Transform target;
    [SerializeField] float moveDuration = 1f;
    public float duration = 5;
    [SerializeField] bool stackingTargerts;

    [Header("Zoom Setting")]
    [SerializeField] bool turnOnZoom;
    [SerializeField] float targetZoom = 2;
    [SerializeField] float zoomSpeed = 1;
    float baseZoom;

    public UnityEvent onFinished;

    Camera cam;

    private void Start()
    {
        cam = CameraManager.instance.CutSceneCamera;
        baseZoom = cam.orthographicSize;
    }


    public void TriggerCameraCutscene()
    {
        cam.gameObject.SetActive(true);
        if (turnOnZoom) ZoomCamera(false);
        PanTo(target.position);


        Invoke(nameof(revertBackToPlayer), duration-moveDuration);

        
    }

    void ZoomCamera(bool reverseZoom)
    {


        float targetSize = baseZoom;
        
        if(!reverseZoom) targetSize /= targetZoom;
        else targetSize = baseZoom;

        LeanTween.value(gameObject, cam.orthographicSize, targetSize, zoomSpeed)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnUpdate(val => cam.orthographicSize = val);

    }

    public void PanTo(Vector3 targetPosition, bool resetting = false)
    {
        Vector3 startPos = cam.transform.position;
        Vector3 finalPos = new Vector3(targetPosition.x, targetPosition.y, startPos.z);

        LeanTween.move(cam.gameObject, finalPos, moveDuration)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() =>
            {
                if (resetting)
                {
                    cam.gameObject.SetActive(false);
                    onFinished.Invoke();
                }
                
            });

    }



    void revertBackToPlayer()
    {
        if (!stackingTargerts && turnOnZoom) ZoomCamera(true);
        if(!stackingTargerts) PanTo(CameraManager.instance.transform.position, true);
        else
        {
            Invoke(nameof(invokeFinished),moveDuration);
        }
    }

    void invokeFinished()
    {
        onFinished.Invoke();
    }




}
