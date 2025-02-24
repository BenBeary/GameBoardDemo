using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DestroyWhenFinished : MonoBehaviour
{

    Animator animMan;
    [SerializeField] bool hideInstead;
    [SerializeField] bool doNothing;
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
        if(!doNothing && !hideInstead && animMan.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) Destroy(gameObject);
        else if(!doNothing && animMan.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) gameObject.SetActive(false);

    }

}
