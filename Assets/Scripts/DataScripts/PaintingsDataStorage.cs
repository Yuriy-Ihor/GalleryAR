﻿using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class PaintingsDataStorage : UnitySingleton<PaintingsDataStorage>
{
    private Dictionary<string, PaintingData> _paintingsData = new Dictionary<string, PaintingData>();

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

        StreamReader reader;

        foreach (var file in fileInfo)
        {
            if(file.Extension == ".json")
            {
                reader = new StreamReader(file.ToString());
                string key = Path.GetFileNameWithoutExtension(file.Name);

                PaintingData newPaintingData = JsonUtility.FromJson<PaintingData>(reader.ReadToEnd());
                _paintingsData.Add(key, newPaintingData);
            }
        }
    }

    public PaintingData GetPaintingData(string key)
    {
        try
        {
            return _paintingsData[key];
        }
        catch(Exception e)
        {
            Debug.LogError(e.ToString());
        }

        return new PaintingData();
    }
}
