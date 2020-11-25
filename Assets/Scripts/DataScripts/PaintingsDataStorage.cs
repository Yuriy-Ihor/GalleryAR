using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class PaintingsDataStorage : UnitySingleton<PaintingsDataStorage>
{
    public List<PaintingData> Data = new List<PaintingData>();
    private Dictionary<string, PaintingData> _data = new Dictionary<string, PaintingData>();

    private DatabaseDataLoader _databaseDataLoader;

    private void Start()
    {
        _databaseDataLoader = DatabaseDataLoader.GetInstance;

        _databaseDataLoader.OnAllDataLoaded.AddListener(loadDataFromJsons);
    }

    private void loadDataFromJsons()
    {
        Debug.Log("Reading data from jsons");

        var info = new DirectoryInfo(PaintingsDataSaver.PathToPaintingsData);
        var fileInfo = info.GetFiles();

        StreamReader reader;

        foreach (var file in fileInfo)
        {
            if(file.Extension == ".json")
            {
                reader = new StreamReader(file.ToString());

                PaintingData newPaintingData = JsonUtility.FromJson<PaintingData>(reader.ReadToEnd());
                Data.Add(newPaintingData);
            }
        }
    }

    public PaintingData GetPaintingData(string key)
    {
        try
        {
            return _data[key];
        }
        catch(Exception e)
        {
            Debug.LogException(e);
        }

        return new PaintingData();
    }
}

[Serializable]
public struct PaintingData
{
    public string Title, Author, Info, Src;
}
