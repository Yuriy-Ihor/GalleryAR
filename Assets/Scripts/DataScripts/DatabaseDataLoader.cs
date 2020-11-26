using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using UnityEngine.Events;

public class DatabaseDataLoader : UnitySingleton<DatabaseDataLoader>
{
    public OnAllDataLoadAttempt OnAllDataLoadAttemptEvent = new OnAllDataLoadAttempt();
    public bool AllDataLoaded { get; private set; } = false;

    private FirebaseDatabase _database;
    private PaintingsDataSaver _dataSaver;

    private void Start()
    {
        _dataSaver = PaintingsDataSaver.GetInstance; 
        _database = FirebaseDatabase.DefaultInstance;

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("No internet connection!");
            OnAllDataLoadAttemptEvent.Invoke();
        }
        else
        {
            initializeFirebase();
        }
    }

    private void initializeFirebase()
    {
        _database.SetPersistenceEnabled(false);

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            LoadAndSaveData();
        });
    }

    private IEnumerator coroutine;

    private void LoadAndSaveData()
    {
        _database.GetReference("Paintings").GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                long totalImages = snapshot.ChildrenCount;

                foreach (DataSnapshot painting in snapshot.Children)
                {
                    Debug.Log("Saving data with key = " + painting.Key);
                    _dataSaver.SaveLoadedSnapshotData(painting);
                }

                coroutine = WaitUntilAllImagesSaved(totalImages);

                StartCoroutine(coroutine);
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