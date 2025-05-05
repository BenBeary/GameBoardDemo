using UnityEngine;

public class DotRandomizer : MonoBehaviour
{

    [SerializeField] Sprite[] dots;
    [SerializeField] bool tryToCopyIndex;
    [SerializeField] bool removeInvisible;
    [HideInInspector] public int spriteNumber;

    public void RandomizeObject(int index = default)
    {
        int rand = Random.Range(0, dots.Length + 1);
        spriteNumber = rand;

        if (dots.Length != 0)
        {
            if (removeInvisible) 
            {
                rand = Random.Range(0, dots.Length);
                spriteNumber = rand;
            }


            if (index != default)
            {
                if (index >= dots.Length) index = dots.Length - 1;
                rand = index;

            }


            if (rand == dots.Length)
            {
                GetComponent<SpriteRenderer>().enabled = false;
                return;
            }
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<SpriteRenderer>().sprite = dots[rand];
        }


        for(int i = 0; i < transform.childCount; i++)
        {
            if(tryToCopyIndex)
            {
                transform.GetChild(i).GetComponent<DotRandomizer>()?.RandomizeObject(rand);
            }
            else
            {
                transform.GetChild(i).GetComponent<DotRandomizer>()?.RandomizeObject();
            }
        }



    }
}
