using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class PaintingsDataStorage : UnitySingleton<PaintingsDataStorage>
{
    private Dictionary<string, PaintingData> _data = new Dictionary<string, PaintingData>();

    private DatabaseDataLoader _databaseDataLoader;

    private void Start()
    {
        _databaseDataLoader = DatabaseDataLoader.GetInstance;

        _databaseDataLoader.OnAllDataLoaded.AddListener(loadDataFromJsons);
    }

    private void loadDataFromJsons()
    {
        var info = new DirectoryInfo(PaintingsDataSaver.PathToPaintingsData);
        var fileInfo = info.GetFiles();
        foreach (var file in fileInfo)
        {
            Debug.Log(file.ToString());
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
