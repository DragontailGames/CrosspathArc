using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class ReplaceAnimation
{
    public static string folder = "Assets/Animation/Creatures";
    public static string oldName = "Flayer";
    public static string newName = "Goblin";
    public static string oldSprites_folder = "./Assets/Sprite/Wolf Inimigo";
    public static string newSprites_folder = "./Assets/Sprite/Female_Archer_Naked_Hand";

    [MenuItem("Dragon Tail/Replace Animation")]
    static void LoadFiles()
    {
        foreach (var asset in AssetDatabase.FindAssets("", new[] { folder + "/" + newName }))
        {
            var path = AssetDatabase.GUIDToAssetPath(asset);
            var temp = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            var name = temp.name.Replace(oldName, newName);
            AssetDatabase.RenameAsset(path, name);
        }

        /*
        DirectoryInfo d = new DirectoryInfo(newFolder);

        foreach (var file in d.GetDirectories())
        {
            RunFiles(file.FullName);
            RunDirectory(file.FullName);
        }

        processFiles();
        processController();

        Debug.Log("---------------------ACABOU---------------------");*/
    }

    public static void RunDirectory(string path)
    {
        DirectoryInfo d = new DirectoryInfo(path);

        foreach (var aux in d.GetDirectories())
        {
            RunFiles(aux.FullName);
        }
    }

    static List<string> metaFilesPath = new List<string>();

    public static void RunFiles(string path)
    {
        DirectoryInfo d = new DirectoryInfo(path);

        foreach (var aux in d.GetFiles("*anim"))
        {
            metaFilesPath.Add(aux.FullName);
        }
    }

    public static void processFiles()
    {
        foreach (var metaFile in metaFilesPath)
        {
            string[] fileText = File.ReadAllLines(metaFile);

            List<string> framesLine = new List<string>();

            foreach (var aux in fileText)
            {
                if (aux.Trim().StartsWith("value: {fileID: 21300000"))
                {
                    int pFrom = aux.IndexOf("guid: ") + "guid: ".Length;
                    int pTo = aux.LastIndexOf(",");

                    string result = aux.Substring(pFrom, pTo - pFrom);
                    //Debug.Log(result);
                    framesLine.Add(result);
                }
            }

            List<string> newText = new List<string>();
            foreach (var aux in fileText)
            {
                newText.Add(aux);
            }

            foreach (var oldGuid in framesLine)
            {
                int indexFile = FindInOldRepository(oldGuid);
                if (indexFile != -1)
                {
                    string newGuid = FindFileInNewRepository(indexFile);
                    if (newGuid != "")
                    {
                        for (int i = 0; i < newText.Count; i++)
                        {
                            newText[i] = newText[i].Replace(oldGuid, newGuid);
                        }
                    }
                }
            }

            File.WriteAllLines(metaFile, newText);
        }
    }

    /*
    public static void processController()
    {
        string[] fileText = File.ReadAllLines(newFolder + "/Female_Archer.controller");
        List<string> guids = new List<string>();

        foreach(var line in fileText)
        {
            if (line.Trim().StartsWith("m_Motion: {fileID: 7400000"))
            {
                int pFrom = line.IndexOf("guid: ") + "guid: ".Length;
                int pTo = line.LastIndexOf(",");

                string result = line.Substring(pFrom, pTo - pFrom);
                guids.Add(result);
            }
        }

        List<string> newText = new List<string>();
        foreach (var aux in fileText)
        {
            newText.Add(aux);
        }

        foreach (var oldGuid in guids)
        {
            var path = FindPathInOldRepository(oldGuid);
            if (path != "")
            {
                var newGuid = GetNewGuid(path);
                if (newGuid != "")
                {
                    for (int i = 0; i < newText.Count; i++)
                    {
                        newText[i] = newText[i].Replace(oldGuid, newGuid);
                        newText[i] = newText[i].Replace(oldName, newName);
                    }
                }
            }
        }

        File.WriteAllLines(newFolder + "/Female_Archer.controller", newText);
    }*/

    public static int FindInOldRepository(string guidReference)
    {
        DirectoryInfo d = new DirectoryInfo(oldSprites_folder);

        FileInfo[] array = d.GetFiles("*meta");
        for (int i = 0; i < array.Length; i++)
        {
            FileInfo aux = array[i];
            string[] metaSprite = File.ReadAllLines(aux.FullName);

            foreach(var temp in metaSprite)
            {
                string guidString = "guid: ";
                if (temp.StartsWith(guidString))
                {
                    string guid = temp.Replace(guidString, "");
                    //Debug.Log(guid + " - " + guidReference);
                    if (guid == guidReference)
                    {
                        return i;
                    }
                    break;
                }
            }
        }

        return -1;
    }

    public static string FindFileInNewRepository(int index)
    {
        DirectoryInfo d = new DirectoryInfo(newSprites_folder);

        FileInfo[] array = d.GetFiles("*meta");
        FileInfo file = array[index];
        string[] metaSprite = File.ReadAllLines(file.FullName);

        foreach (var temp in metaSprite)
        {
            string guidString = "guid: ";
            if (temp.StartsWith(guidString))
            {
                string guid = temp.Replace(guidString, "");
                return guid;
            }
        }
        return "";
    }

    public static string FindPathInOldRepository(string guid)
    {
        foreach(var aux in metaFilesPath)
        {
            var txt = File.ReadAllText(aux.Replace(newName, oldName) + ".meta");
            if (txt.Contains(guid))
            {
                return aux;
            }
        }

        return "";
    }

    public static string GetNewGuid(string path)
    {
        string metaPath = path + ".meta";
        foreach(var aux in File.ReadAllLines(metaPath))
        {
            string guidString = "guid: ";
            if (aux.StartsWith(guidString))
            {
                string guid = aux.Replace(guidString, "");
                return guid;
            }
        }
        return "";
    }
}

