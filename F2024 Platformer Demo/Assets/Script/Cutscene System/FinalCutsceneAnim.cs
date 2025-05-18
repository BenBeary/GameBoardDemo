using UnityEngine;
using UnityEngine.Events;

public class FinalCutsceneAnim : MonoBehaviour
{



    [SerializeField] LeverTrigger lever;

    public UnityEvent onAnimationFinished;


    public void ActivateLever()
    {
        lever.ActivateLever();
    }

    public void EndAnimation()
    {
        onAnimationFinished.Invoke();
    }





}
