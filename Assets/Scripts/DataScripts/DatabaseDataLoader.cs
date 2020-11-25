using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using UnityEngine.Events;

public class DatabaseDataLoader : UnitySingleton<DatabaseDataLoader>
{
    public OnAllDataLoaded OnAllDataLoadedEvent = new OnAllDataLoaded();
    public bool AllDataLoaded { get; private set; } = false;

    private FirebaseDatabase _database;
    private PaintingsDataSaver _dataSaver;

    private void Start()
    {
        _database = FirebaseDatabase.DefaultInstance;
        _dataSaver = PaintingsDataSaver.GetInstance;

        _database.SetPersistenceEnabled(false);

        OnAllDataLoadedEvent.AddListener(() => { AllDataLoaded = true; });

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

                foreach (DataSnapshot painting in snapshot.Children)
                {
                    Debug.Log("Saving data with key = " + painting.Key);
                    _dataSaver.SaveLoadedSnapshotData(painting);
                }

                OnAllDataLoadedEvent.Invoke();
            }
        });
    }
    

}

[System.Serializable]
public class OnAllDataLoaded : UnityEvent
{

}