using UnityEngine;

public class BossDeathSequence : MonoBehaviour
{


    [SerializeField] float fallSpeed = 5f;
    [SerializeField] float rotateSpeed = 5f;


    private void FixedUpdate()
    {

        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
    }


    public void TriggerDeath()
    {
        Destroy(gameObject, 5);
    }

}
