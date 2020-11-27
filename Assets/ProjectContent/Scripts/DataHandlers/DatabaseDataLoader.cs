using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using UnityEngine.Events;

public class DatabaseDataLoader : UnitySingleton<DatabaseDataLoader>
{
    public OnAllDataLoadAttempt OnAllDataLoadAttemptEvent = new OnAllDataLoadAttempt();

    private FirebaseDatabase _database;
    private PaintingsDataSaver _dataSaver;

    private void Start()
    {
        _dataSaver = PaintingsDataSaver.GetInstance; 
        _database = FirebaseDatabase.DefaultInstance;

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            OnAllDataLoadAttemptEvent.Invoke();
        }
        else
        {
            InitializeFirebase();
        }
    }

    private void InitializeFirebase()
    {
        _database.SetPersistenceEnabled(false);

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            LoadAndSaveData();
        });
    }

    private void LoadAndSaveData()
    {
        _database.GetReference("Paintings").GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                long totalImages = snapshot.ChildrenCount;

                foreach (DataSnapshot painting in snapshot.Children)
                {
                    _dataSaver.SaveLoadedDataFromSnapshot(painting);
                }

                StartCoroutine(WaitUntilAllImagesSaved(totalImages));
            }
        });
    }
    
    private IEnumerator WaitUntilAllImagesSaved(long totalImages)
    {
        yield return new WaitUntil( () => totalImages <= _dataSaver.TotalImagesDownloaded );

        OnAllDataLoadAttemptEvent.Invoke();
    }

}

[System.Serializable]
public class OnAllDataLoadAttempt : UnityEvent
{ 
}