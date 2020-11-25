using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Events;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using Firebase.Storage;
using Firebase.Unity.Editor;

public class DatabaseDataLoader : UnitySingleton<DatabaseDataLoader>
{
    private Dictionary<string, PaintingData> _data = new Dictionary<string, PaintingData>();

    private FirebaseDatabase _database;


    private void Start()
    {
        _database = FirebaseDatabase.DefaultInstance;

        _database.SetPersistenceEnabled(false);

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            Debug.Log("Firebase initialized");
            LoadData();
        });
    }

    private void LoadData()
    {
        _database.GetReference("Paintings").GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                foreach (DataSnapshot painting in snapshot.Children)
                {
                    saveLoadedData(painting);
                }
            }
        });
    }

    private void saveLoadedData(DataSnapshot painting)
    {
        string info = painting.GetRawJsonValue();

        PaintingData newData = JsonUtility.FromJson<PaintingData>(info);

        DownloadImage(painting.Key + ".jpg", newData.Src, Application.dataPath + "/Data/Paintings");
        File.WriteAllText(Path.Combine(Application.dataPath + "/Data/PaintingsInfo", painting.Key + ".json"), info);

        _data.Add(painting.Key, newData);
    }

    public void DownloadImage(string fileName, string url, string pathToSaveImage)
    {
        WWW www = new WWW(url);
        StartCoroutine(downloadImage(fileName, www, pathToSaveImage));
    }

    private IEnumerator downloadImage(string fileName, WWW www, string savePath)
    {
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            Debug.Log("Success");

            saveImage(fileName, savePath, www.bytes);
        }
        else
        {
            Debug.LogError("Error: " + www.error);
        }
    }

    private void saveImage(string fileName, string path, byte[] imageBytes)
    {
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

        string pathWithFileName = Path.Combine(path, fileName);

        try
        {
            File.WriteAllBytes(pathWithFileName, imageBytes);
            Debug.Log("Saved Data to: " + path.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Save Data to: " + pathWithFileName.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
    }

}
