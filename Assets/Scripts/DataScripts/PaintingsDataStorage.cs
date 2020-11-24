using System;
using System.Collections.Generic;
using UnityEngine;

public class PaintingsDataStorage : UnitySingleton<PaintingsDataStorage>
{
    [SerializeField] private TextAsset[] _paintingDatas;
    
    private List<PaintingData> _data = new List<PaintingData>();

    private void Start()
    {
        for (int i = 0; i < _paintingDatas.Length; i++)
        {
            string get = _paintingDatas[i].ToString();
            PaintingData newData = JsonUtility.FromJson<PaintingData>(get);
            _data.Add(newData);
        }
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
