using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class RegionController : MonoBehaviour
{

    [System.Serializable]
    public struct ItemData
    {
        public string itemId;
        public GameObject obj;
    }




    public string RegionName;
    public List<ItemData> itemsInScene = new List<ItemData>();

    [Header("Debug")]
    public List<string> savedItemIds = new List<string>();






    private void Start()
    {
        if (!GameManager.Instance) 
        {
            Debug.LogWarning("No GameManager found");
            return;
        } 

        if (GameManager.Instance.CheckForData(RegionName)) // grab any saved data that is on the game manager
        {
            savedItemIds = new List<string>(GameManager.Instance.GetRegionData(RegionName));

            foreach(string item in savedItemIds)
            {
                Destroy(itemsInScene.First(x => x.itemId == item).obj);
            }
        }
    }



    public void AddCollectedItem(GameObject itemID)
    {
        if(!itemsInScene.Any(x => x.obj == itemID))
        {
            Debug.LogWarning(itemID.name + " tried to save but its not on " + this.name + "'s list");
            return;
        }

        savedItemIds.Add(itemsInScene.First(x => x.obj == itemID).itemId);
    }



    private void OnDisable()
    {
        if(savedItemIds.Count > 0)
        {
            if (!GameManager.Instance.CheckForData(RegionName)) // Send Data
            {
                GameManager.Instance.addRegion(this);
                return;
            }
            GameManager.Instance.UpdateRegionData(RegionName, savedItemIds);
        }
    }




}
