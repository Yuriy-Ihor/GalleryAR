using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class ImageLibraryInitializer : UnitySingleton<ImageLibraryInitializer>
{
    [SerializeField] private ARTrackedImageManager _trackedImageManager;
    [SerializeField] private Dictionary<string, Texture2D> _paintings = new Dictionary<string, Texture2D>();
    [SerializeField] private XRReferenceImageLibrary _referenceLibrary;

    private MutableRuntimeReferenceImageLibrary _mutableLibrary;

    public OnLibraryCreated OnLibraryCreatedEvent = new OnLibraryCreated();
    public int TotalImagesAdded = 0;

    private void Awake()
    {
        _trackedImageManager = gameObject.AddComponent<ARTrackedImageManager>();

#if !UNITY_EDITOR
        _trackedImageManager.referenceLibrary = _trackedImageManager.CreateRuntimeLibrary(_referenceLibrary);
#endif

        _trackedImageManager.enabled = true;
    }

    public void InitializeImageLibrary()
    {
        loadPaintings();

#if !UNITY_EDITOR
        foreach (var painting in _paintings)
        {
            AddNewImage(painting.Key, painting.Value);
        }
#endif

        OnLibraryCreatedEvent.Invoke();
    }

    public void AddNewImage(string name, Texture2D image)
    {
        try
        {
            _mutableLibrary = _trackedImageManager.referenceLibrary as MutableRuntimeReferenceImageLibrary;

            var jobHandle = _mutableLibrary.ScheduleAddImageJob(image, name, 1);

            while(!jobHandle.IsCompleted)
            {

            }

            jobHandle.Complete();
            TotalImagesAdded++;
            Debug.Log("Image added!");
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    private void loadPaintings()
    {
        Debug.Log("Loading paintings...");

        var paintings = new DirectoryInfo(PaintingsDataSaver.PathToPaintings);
        var fileInfo = paintings.GetFiles();

        foreach (var file in fileInfo)
        {
            if (file.Extension == ".jpg")
            {
                var texture = LoadPNG(file.ToString());
                Debug.Log(Path.GetFileNameWithoutExtension(file.ToString()));
                _paintings.Add(Path.GetFileNameWithoutExtension(file.ToString()), texture);
            }
        }
    }

    public static Texture2D LoadPNG(string filePath)
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


