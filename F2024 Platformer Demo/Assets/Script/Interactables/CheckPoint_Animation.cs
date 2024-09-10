using UnityEngine;

public class CheckPoint_Animation : MonoBehaviour
{


    [SerializeField] Transform Flag;
    [SerializeField] float moveAmount;
    [SerializeField] float speed;

    public void MoveFlag(bool moveDown)
    {
        int direction = (moveDown? -1 : 1);
        Vector2 endPos = new Vector2(Flag.localPosition.x, Flag.localPosition.y + (moveAmount * direction));
        

        while((Vector2)Flag.localPosition != endPos)
        {
            Flag.localPosition = Vector2.MoveTowards(Flag.localPosition,endPos, (speed/10) * Time.deltaTime);
        }


    }

    

}
