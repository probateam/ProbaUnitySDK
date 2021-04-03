using System;
using UnityEngine;
using UnityEditor;


[InitializeOnLoad]
public class Startup
{
    static Startup()
    {
        try
        {
            var dbPath = Application.dataPath + "/Proba/DataBase/EditorDatabase.s3db";
            var newdbPath = Application.dataPath + "/Proba/DataBase/.EditorDatabase.s3db";
            dbPath = dbPath.Replace(Application.dataPath, "Assets");
            newdbPath = newdbPath.Replace(Application.dataPath, "Assets");
            System.IO.File.Move(dbPath, newdbPath);
        }
        catch (System.IO.FileNotFoundException)
        {

        }
    }
}