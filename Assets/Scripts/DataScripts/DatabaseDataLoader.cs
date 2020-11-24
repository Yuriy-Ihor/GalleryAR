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
    [SerializeField] private List<PaintingData> _data = new List<PaintingData>();

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
        string dataPath = Application.dataPath + "/Data/Paintings";

        _database.GetReference("Paintings").GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                foreach (DataSnapshot painting in snapshot.Children)
                {
                    string info = painting.GetRawJsonValue();

                    PaintingData newData = JsonUtility.FromJson<PaintingData>(info);
                    _data.Add(newData);

                    if (!System.IO.File.Exists(Path.Combine(dataPath, newData.Title + ".jpg")))
                    {
                        DownloadImage(newData.Title + ".jpg", newData.Src, dataPath);
                    }
                    else
                    {
                        Debug.Log("Picture " + newData.Title + ".jpg already exists");
                    }
                }
            }
        });
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

        string finalPath = Path.Combine(path, fileName);

        try
        {
            File.WriteAllBytes(finalPath, imageBytes);
            Debug.Log("Saved Data to: " + path.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Save Data to: " + finalPath.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
    }

}
