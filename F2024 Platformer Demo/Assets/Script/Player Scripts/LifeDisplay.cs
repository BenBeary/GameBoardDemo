using UnityEngine;


[RequireComponent(typeof(Animator))]
public class LifeDisplay : MonoBehaviour
{


    int currentHealth;
    Animator animMan;
    bool goingDown;

    private void Start()
    {
        animMan = GetComponent<Animator>();
        currentHealth = PlayerController.instance.currentHealth;

        InitializeAnimation();


    }

    public void InitializeAnimation()
    {
        string animName = "Change_" + (currentHealth - 1).ToString() + "-" + (currentHealth).ToString();
        Debug.Log(animName);
        animMan.Play(animName, 0, 1);
    }


    private void Update()
    {
        int difference = currentHealth - PlayerController.instance.currentHealth;
        string animName = string.Empty;

        if (Mathf.Abs(difference) > 1 && !animMan.GetBool("AutoChange"))  // Moving Multiple
        {
            animMan.SetBool("AutoChange", true);
            goingDown = (currentHealth - PlayerController.instance.currentHealth > 0) ? true : false;
            int direction = goingDown ? -1 : 1;
            animName = "Change_" + (currentHealth).ToString() + "-" + (currentHealth + direction).ToString();
            //Debug.Log(animName);
            animMan.Play(animName);
            currentHealth = PlayerController.instance.currentHealth;
        }
        else if (animMan.GetBool("AutoChange"))
        {
            if(currentHealth != PlayerController.instance.currentHealth)  // Fixes issue from going from low health to full health mid animation
            { 
                goingDown = (currentHealth - PlayerController.instance.currentHealth < 0) ? false : true; 
                currentHealth = PlayerController.instance.currentHealth;
            }

            if (goingDown) animName = "Change_" + (currentHealth).ToString() + "-" + (currentHealth - 1).ToString();
            else animName = "Change_" + (currentHealth - 1).ToString() + "-" + (currentHealth).ToString();

            //Debug.Log(animName +" | " + animMan.GetCurrentAnimatorClipInfo(0)[0].clip.name);
            if (animName != animMan.GetCurrentAnimatorClipInfo(0)[0].clip.name) return;

            animMan.SetBool("AutoChange", false);

        }

        if (Mathf.Abs(difference) == 1 && !animMan.GetBool("AutoChange")) // Moving Once
        {
            animName = "Change_" + currentHealth.ToString() + "-" + (currentHealth - difference).ToString();
            animMan.Play(animName);
            currentHealth = PlayerController.instance.currentHealth;
        }


    }



}
