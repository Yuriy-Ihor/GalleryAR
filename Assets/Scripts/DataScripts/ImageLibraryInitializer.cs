using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageLibraryInitializer : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager _trackedImageManager;
    [SerializeField] private List<Texture2D> _paintings = new List<Texture2D>();

    private void Awake()
    {
        if (_trackedImageManager == null)
            _trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    public void LoadDataToImageLibrary()
    {
        loadPaintings();

        //RuntimeReferenceImageLibrary runtimeLibrary = _trackedImageManager.CreateRuntimeLibrary();

        //MutableRuntimeReferenceImageLibrary mutableLibrary = runtimeLibrary as MutableRuntimeReferenceImageLibrary;
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
                _paintings.Add(texture);
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
}
