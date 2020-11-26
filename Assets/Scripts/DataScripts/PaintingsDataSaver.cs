
using System;
using System.IO;
using System.Collections;
using UnityEngine;
using Firebase.Database;

public class PaintingsDataSaver : UnitySingleton<PaintingsDataSaver>
{
    public static string PathToPaintings => Application.persistentDataPath + "/Data/Paintings";
    public static string PathToPaintingsData => Application.persistentDataPath + "/Data/PaintingsData";

    public int TotalImagesDownloaded { get; private set; } = 0;

    public void SaveLoadedSnapshotData(DataSnapshot painting)
    {
        string info = painting.GetRawJsonValue();

        PaintingData newData = JsonUtility.FromJson<PaintingData>(info);

        DownloadImage(painting.Key + ".jpg", newData.Src, PathToPaintings);

        try
        {
            if (!Directory.Exists(PathToPaintingsData))
            {
                Directory.CreateDirectory(PathToPaintingsData);
            }

            File.WriteAllText(Path.Combine(PathToPaintingsData, painting.Key + ".json"), info);
        }
        catch(Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    public void DownloadImage(string fileName, string url, string pathToSaveImage)
    {
        WWW www = new WWW(url);
        StartCoroutine(downloadImage(fileName, www, pathToSaveImage));
    }

    private IEnumerator downloadImage(string fileName, WWW www, string savePath)
    {
        yield return www;

        TotalImagesDownloaded++;

        if (string.IsNullOrEmpty(www.error))
        {
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
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to save image to: " + pathWithFileName.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
    }
}
