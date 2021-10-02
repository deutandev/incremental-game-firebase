using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UserDataManager
{
    private const string PROGRESS_KEY = "Progress";
    public static UserProgressData Progress;

    public static void Load()
    {
        // Cek apakah ada data yang tersimpan sebagai PROGRESS_KEY
        if (!PlayerPrefs.HasKey(PROGRESS_KEY))
        {
            // Jika tidak ada, maka buat data baru
            Progress = new UserProgressData();
            Save();
        }
        else
        {
            // Jika ada, maka timpa progress dengan yang sebelumnya
            string json = PlayerPrefs.GetString(PROGRESS_KEY);
            Progress = JsonUtility.FromJson<UserProgressData>(json);
        }
    }

    public static void Save()
    {
        string json = JsonUtility.ToJson(Progress);
        PlayerPrefs.SetString(PROGRESS_KEY, json);
    }
}
