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
    [SerializeField] bool isActive;

    [Space(20)]
    public UnityEvent onActive;
    public UnityEvent onDeactive;

    bool onCooldown;

    private void Start()
    {
        SetInitialState();
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


    IEnumerator ChangeDelay()
    {
        onCooldown = true;
        yield return new WaitForSeconds(coolDown);

        onCooldown = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!onCooldown && collision.CompareTag("Player") && Input.GetAxisRaw("Activate") == 1f)
        {
            if (!isActive)
            {
                StartCoroutine(ChangeDelay());
                isActive = true;
                onActive.Invoke();
                transform.GetChild(0).transform.localRotation = Quaternion.Euler(0, 0, leverRotation);
            }
            else
            {
                StartCoroutine(ChangeDelay());
                isActive = false;
                onDeactive.Invoke();
                transform.GetChild(0).transform.localRotation = Quaternion.Euler(0, 0, -leverRotation);
            }
        }
    }






}
