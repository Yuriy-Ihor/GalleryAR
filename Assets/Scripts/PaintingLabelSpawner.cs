using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PaintingLabelSpawner : MonoBehaviour
{
    [SerializeField] private PaintingLabel _paintingLabelPrefab;

    [SerializeField]  private ARTrackedImageManager _arTrackedImageManager;

    private List<PaintingLabel> _spawnedLabels;

    private void Start()
    {
        if (_arTrackedImageManager == null)
            _arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        _arTrackedImageManager.trackedImagesChanged += OnPaintingIdentified;
    }

    private void OnDisable()
    {
        _arTrackedImageManager.trackedImagesChanged -= OnPaintingIdentified;
    }

    private void OnPaintingIdentified(ARTrackedImagesChangedEventArgs args)
    {
        for (int i = 0; i < args.added.Count; i++)
        {
            PaintingLabel newInfo = Instantiate(_paintingLabelPrefab);
            newInfo.Init(args.added[i]);

            _spawnedLabels.Add(newInfo);
        }
    }
}
