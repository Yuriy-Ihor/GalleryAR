using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class ImageLibraryInitializer : UnitySingleton<ImageLibraryInitializer>
{
    private ARTrackedImageManager _trackedImageManager;
    private Dictionary<string, Texture2D> _paintings = new Dictionary<string, Texture2D>();

    private XRReferenceImageLibrary _xrReferenceImageLibrary;
    private MutableRuntimeReferenceImageLibrary _mutableLibrary;

    public OnLibraryCreated OnLibraryCreatedEvent = new OnLibraryCreated();

    private void Awake()
    {
        _trackedImageManager = gameObject.AddComponent<ARTrackedImageManager>();

#if !UNITY_EDITOR
        _trackedImageManager.referenceLibrary = _trackedImageManager.CreateRuntimeLibrary(_xrReferenceImageLibrary);
#endif

        _trackedImageManager.enabled = true;
    }

    public void InitializeImageLibrary()
    {
        loadPaintings();

        foreach (var painting in _paintings)
        {
            AddNewImage(painting.Key, painting.Value);
        }

        OnLibraryCreatedEvent.Invoke();
    }

    public void AddNewImage(string name, Texture2D image)
    {

 #if !UNITY_EDITOR
        _mutableLibrary = _trackedImageManager.referenceLibrary as MutableRuntimeReferenceImageLibrary;

        try
        {
            var jobHandle = _mutableLibrary.ScheduleAddImageJob(image, name, 0.5f);
            jobHandle.Complete();
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
#endif

    }

    private void loadPaintings()
    {
        var paintings = new DirectoryInfo(PaintingsDataSaver.PathToPaintings);
        var fileInfo = paintings.GetFiles();

        foreach (var file in fileInfo)
        {
            if (file.Extension == ".jpg")
            {
                var texture = loadPNG(file.ToString());

                _paintings.Add(Path.GetFileNameWithoutExtension(file.ToString()), texture);
            }
        }
    }

    private Texture2D loadPNG(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
        }
        return tex;
    }

    [Serializable]
    public class OnLibraryCreated : UnityEvent
    { }
}


