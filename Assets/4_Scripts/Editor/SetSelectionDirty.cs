using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class SetSelectionDirtyTool
{

    [MenuItem("Tools/GoodHub/Set Selection Dirty")]
    public static void SetSelectionDirty()
    {
        foreach (Object obj in Selection.objects)
        {
            EditorUtility.SetDirty(obj);
        }
    }

}