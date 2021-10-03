// Packages for cloud save/load
using Firebase.Storage;
using System.Collections;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public static class UserDataManager
{
    private const string PROGRESS_KEY = "Progress";
    public static UserProgressData Progress = new UserProgressData ();

    public static void LoadFromLocal()
    {
        // Cek apakah ada data yang tersimpan sebagai PROGRESS_KEY
        if (!PlayerPrefs.HasKey(PROGRESS_KEY))
        {
            // Progress = new UserProgressData();
            // Jika tidak ada, maka buat data baru dan upload ke cloud
            Save(true);
        }
        else
        {
            // Jika ada, maka timpa progress dengan yang sebelumnya
            string json = PlayerPrefs.GetString(PROGRESS_KEY);
            Progress = JsonUtility.FromJson<UserProgressData>(json);
        }
    }

    // Load from cloud
    public static IEnumerator LoadFromCloud (System.Action onComplete)
    {
        StorageReference targetStorage = GetTargetCloudStorage ();

        bool isCompleted = false;
        bool isSuccessfull = false;
        const long maxAllowedSize = 1024 * 1024; // Sama dengan 1 MB
        targetStorage.GetBytesAsync (maxAllowedSize).ContinueWith ((Task<byte[]> task) =>
        {
            if (!task.IsFaulted)
            {
                string json = Encoding.Default.GetString (task.Result);
                Progress = JsonUtility.FromJson<UserProgressData> (json);
                isSuccessfull = true;
            }

            isCompleted = true;
        });

        while (!isCompleted)
        {
            yield return null;
        }

        // Jika sukses mendownload, maka simpan data hasil download
        if (isSuccessfull)
        {
            Save ();
        }
        else
        {
            // Jika tidak ada data di cloud, maka load data dari local
            LoadFromLocal ();
        }

        onComplete?.Invoke ();
    }


    // Cloud save
    public static void Save(bool uploadToCloud = false)
    {
        string json = JsonUtility.ToJson(Progress);
        PlayerPrefs.SetString(PROGRESS_KEY, json);

        // Cloud save
        if (uploadToCloud)
        {
            // Track user propertiy
            AnalyticsManager.SetUserProperties ("gold", Progress.Gold.ToString ());
            
            byte[] data = Encoding.Default.GetBytes (json);
            StorageReference targetStorage = GetTargetCloudStorage ();

            targetStorage.PutBytesAsync (data);
        }
    }

    // Cloud save/load
    private static StorageReference GetTargetCloudStorage ()
    {
        // Gunakan Device ID sebagai nama file yang akan disimpan di cloud
        string deviceID = SystemInfo.deviceUniqueIdentifier;
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;

        return storage.GetReferenceFromUrl ($"{storage.RootReference}/{deviceID}");
    }

    public static bool HasResources (int index)
    {
        return index + 1 <= Progress.ResourcesLevels.Count;
    }
}
