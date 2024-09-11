using UnityEngine;

public class RemoveCameraTarget : MonoBehaviour
{

    [SerializeField] float addToPlayerCameraSize;
    [SerializeField] Transform changeToTarget;
    [SerializeField] float cameraSizeForTarget;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (changeToTarget != null && changeToTarget.gameObject.activeSelf == false) return;
        GameManager.Instance.SetNewTarget(changeToTarget, cameraSizeForTarget);
        GameManager.Instance.SetPlayerCameraSize(addToPlayerCameraSize);
    }
}
