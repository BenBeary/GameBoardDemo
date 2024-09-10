using UnityEngine;

public class DotRandomizer : MonoBehaviour
{

    [SerializeField] Sprite[] dots;
    [SerializeField] bool isDomino;

    public void RandomizeObject()
    {
  
        int rand = Random.Range(0, dots.Length+1);
        if(isDomino)
        {
            rand = Random.Range(0, dots.Length-1);
        }

        if (rand == dots.Length)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            return;
        }
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<SpriteRenderer>().sprite = dots[rand];

        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<DotRandomizer>()?.RandomizeObject();
        }


    }
}
