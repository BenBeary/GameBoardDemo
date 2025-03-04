using UnityEngine;



namespace DataManage
{
    [System.Serializable]
    public class ItemSaver 
    {
        public static void saveItem(RegionController region, GameObject item)
        {
            region.AddCollectedItem(item);
        }
    }
}
