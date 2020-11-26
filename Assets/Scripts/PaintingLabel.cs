using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PaintingLabel : MonoBehaviour
{
    [SerializeField] private TextMeshPro _paintingTitle;
    [SerializeField] private TextMeshPro _paintingAuthor;
    [SerializeField] private TextMeshPro _paintingInfo;

    private PaintingData _paintingData;

    public void Init(ARTrackedImage image)
    {
        PaintingsDataStorage paintingsDataStorage = PaintingsDataStorage.GetInstance;
        _paintingData = paintingsDataStorage.GetPaintingData(image.referenceImage.name);

        _paintingTitle.text = _paintingData.Title;
        _paintingAuthor.text = _paintingData.Author;
        _paintingInfo.text = _paintingData.Info;

        transform.parent = image.transform;
        transform.localPosition = new Vector3(image.referenceImage.size.x, 0, 0);

        transform.localScale *= image.referenceImage.size.x;
    }
}
