using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Color Swatch", menuName = "ScriptableObjects/Spawn Skin Color Swatch", order = 1)]
public class SkinColorSwatches : ScriptableObject
{

   [System.Serializable]
   public class colorSwatch
    {
        public Color mainColor = Color.white,
                        outlineColor = Color.black,
                        dotColor = Color.black;
    }

    public List<colorSwatch> colorSwatches = new List<colorSwatch>();

}
