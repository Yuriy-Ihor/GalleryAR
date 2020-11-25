using System;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class ImageLibraryInitializer : MonoBehaviour
{
    private DatabaseDataLoader _databaseDataLoader;

    private void Start()
    {
        _databaseDataLoader = DatabaseDataLoader.GetInstance;

        RuntimeReferenceImageLibrary library;
        MutableRuntimeReferenceImageLibrary library2; 
    }
}
