using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TempRenameUtil : MonoBehaviour
{
    public string folder = "";
    public string replace_name = "";
    public string new_name = "";

    [ContextMenu("Command")]
    void RenameFiles()
    {
        foreach (var asset in AssetDatabase.FindAssets("", new[] { folder }))
        {
            var path = AssetDatabase.GUIDToAssetPath(asset);
            var temp =  AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            var name = temp.name.Replace(replace_name, new_name);
            AssetDatabase.RenameAsset(path, name);
        }
    }
}
