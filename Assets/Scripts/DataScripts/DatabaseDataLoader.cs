using System.Collections.Generic;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using UnityEngine.Events;

public class DatabaseDataLoader : UnitySingleton<DatabaseDataLoader>
{
    private FirebaseDatabase _database;
    private PaintingsDataSaver _dataSaver;

    public UnityEvent OnAllDataLoaded;

    private void Start()
    {
        _database = FirebaseDatabase.DefaultInstance;
        _dataSaver = PaintingsDataSaver.GetInstance;

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

                foreach (DataSnapshot painting in snapshot.Children)
                {
                    _dataSaver.SaveLoadedSnapshotData(painting);
                }

                OnAllDataLoaded.Invoke();
            }
        });
    }
}
