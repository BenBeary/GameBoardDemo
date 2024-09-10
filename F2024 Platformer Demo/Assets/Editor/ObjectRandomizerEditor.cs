using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DotRandomizer)),CanEditMultipleObjects]
public class ObjectRandomizerEditor : Editor
{

    private SerializedObject m_Object;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        

        if (GUILayout.Button("Randomize Object"))
        {
            foreach(Object target  in targets)
            {
                DotRandomizer dot = (DotRandomizer)target;

                dot.RandomizeObject();
            }
        }
    }



}
