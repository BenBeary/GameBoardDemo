using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpRestore : MonoBehaviour
{
    [SerializeField] float bobIntensity;
    [SerializeField] float bobFrequency;
    [SerializeField] float rotationSpeed = 0.5f;


    private void Start()
    {
        PlayerController.playerReset += resetAbility;
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector3.up * Mathf.Cos(Time.time * bobFrequency) * bobIntensity);
        transform.GetChild(0).transform.Rotate(0, 0, rotationSpeed);
    }

    
    private void resetAbility()
    {
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<ParticleSystem>().Play();
        transform.GetChild(0).gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == PlayerController.instance.gameObject)
        {
            PlayerController.instance.doubleJump = true;
            PlayerController.instance.dotCount++;
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<ParticleSystem>().Stop();
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
