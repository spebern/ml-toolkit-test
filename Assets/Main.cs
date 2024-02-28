using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

public class Main : MonoBehaviour
{
    private FirebaseApp m_app;
    private FirebaseFirestore m_db;

    private void Start()
    {
        Debug.Log("Initializing Firebase");
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                m_app = FirebaseApp.DefaultInstance;
                Debug.Log("Firebase is ready to use!");
                m_db = FirebaseFirestore.DefaultInstance;
                ListenForUserChanges();
                UpsertUser();
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }

    private void UpsertUser()
    {
        m_db.Collection("user").Document("1").SetAsync(new
        {
            id = "1",
            name = "John Doe"
        });
    }

    private void ListenForUserChanges()
    {
        var query = m_db.Collection("user");
        query.Listen(snapshot =>
        {
            foreach (var doc in snapshot.Documents)
            {
                if (doc is not { Exists: true })
                    continue;

                var name = doc.GetValue<string>("name");
                Debug.Log("User Id: " + doc.Id + " , Name: " + name);
            }
        });
    }
}