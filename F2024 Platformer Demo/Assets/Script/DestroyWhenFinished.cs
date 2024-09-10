using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DestroyWhenFinished : MonoBehaviour
{

    Animator animMan;
    [SerializeField] bool hideInstead;
    private void Awake()
    {
        animMan = GetComponent<Animator>();

    }

    private void OnEnable()
    {
        animMan.Play(animMan.GetCurrentAnimatorClipInfo(0)[0].clip.name); // resets the normalized Times
    }

    private void Update()
    {
        if(!hideInstead && animMan.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) Destroy(gameObject);
        else if(animMan.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) gameObject.SetActive(false);
    }

}
