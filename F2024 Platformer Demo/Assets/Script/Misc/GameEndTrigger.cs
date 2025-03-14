using TMPro;
using UnityEngine;

public class GameEndTrigger : MonoBehaviour
{

    [SerializeField] TMP_Text Deaths;
    [SerializeField] TMP_Text Timer;


    private void OnEnable()
    {
        Time.timeScale = 0f;
        
        Deaths.text = GameManager.Instance.playerDeathCount.ToString();
        Timer.text = (GameManager.Instance.timePlayedInGame / 60f).ToString("#.00") + " Minutes";
    }

}
