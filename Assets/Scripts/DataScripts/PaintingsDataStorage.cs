using System;
using System.Collections.Generic;
using UnityEngine;

public class PaintingsDataStorage : UnitySingleton<PaintingsDataStorage>
{   
    private List<PaintingData> _data = new List<PaintingData>();

    private void Start()
    {
        
    }

    public PaintingData GetPaintingData(string paintingName)
    {
        try
        {
            return _data.Find(x => x.Title == paintingName);
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
