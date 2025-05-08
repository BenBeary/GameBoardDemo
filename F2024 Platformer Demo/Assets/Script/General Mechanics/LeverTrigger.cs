using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class LeverTrigger : MonoBehaviour
{





    [Header("Lever Settings")]
    [SerializeField] float leverRotation = .1f;
    [SerializeField] float coolDown = 1f;
    public bool isActive;

    [Space(20)]
    public UnityEvent onActive;
    public UnityEvent onDeactive;

    bool onCooldown;

    private void Start()
    {
        SetInitialState();
        PlayerController.playerReset += resetToDefault;
    }

    private void OnDisable()
    {
        PlayerController.playerReset -= resetToDefault;
    }

    void SetInitialState()
    {
        if (!isActive)
        {
            transform.GetChild(0).transform.localRotation = Quaternion.Euler(0,0, -leverRotation);
        }
        else
        {
            transform.GetChild(0).transform.localRotation = Quaternion.Euler(0, 0, leverRotation);
        }
    }

    void resetToDefault()
    {
        isActive = false;
        SetInitialState();
    }


    IEnumerator ChangeDelay()
    {
        onCooldown = true;
        yield return new WaitForSeconds(coolDown);

        onCooldown = false;
    }

    public void ActivateLever()
    {
        StartCoroutine(ChangeDelay());
        isActive = true;
        onActive.Invoke();
        transform.GetChild(0).transform.localRotation = Quaternion.Euler(0, 0, leverRotation);
    }

    public void DeactivateLever()
    {
        StartCoroutine(ChangeDelay());
        isActive = false;
        onDeactive.Invoke();
        transform.GetChild(0).transform.localRotation = Quaternion.Euler(0, 0, -leverRotation);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!onCooldown && collision.CompareTag("Player") && Input.GetAxisRaw("Activate") == 1f)
        {
            if (!isActive) ActivateLever();
            else DeactivateLever();
        }
    }






}
