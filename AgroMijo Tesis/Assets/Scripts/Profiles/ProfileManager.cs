using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance { get; private set; }

    private string savePath;

    public ProfileData profileData;
    public PlayerProfile CurrentProfile { get; private set; }
    public event Action ProfilesLoaded;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(
            Application.persistentDataPath,
            "profiles.json"
        );

        LoadProfiles();
    }

    public void CreateProfile(string alias)
    {
        string id = GenerateUniqueId();

        PlayerProfile newProfile = new PlayerProfile(id, alias);

        profileData.profiles.Add(newProfile);

        CurrentProfile = newProfile;
        profileData.currentProfileId = newProfile.id;

        SaveProfiles();

        ProfilesLoaded?.Invoke();
    }

    public void SelectProfile(string id)
    {
        CurrentProfile = profileData.profiles
            .FirstOrDefault(profile => profile.id == id);

        if (CurrentProfile != null)
        {
            profileData.currentProfileId = CurrentProfile.id;
            SaveProfiles();
        }
        ProfilesLoaded?.Invoke();
    }

    public void DeleteProfile(string id)
    {
        PlayerProfile profile = profileData.profiles
            .FirstOrDefault(profile => profile.id == id);

        if (profile == null)
            return;

        if (CurrentProfile == profile)
        {
            CurrentProfile = null;
            profileData.currentProfileId = null;
        }

        profileData.profiles.Remove(profile);

        SaveProfiles();
        ProfilesLoaded?.Invoke();
    }

    private void SaveProfiles()
    {
        string json = JsonUtility.ToJson(profileData, true);
        File.WriteAllText(savePath, json);
    }

    private void LoadProfiles()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            profileData = JsonUtility.FromJson<ProfileData>(json);

            if (profileData == null)
            {
                profileData = new ProfileData();
            }
        }
        else
        {
            profileData = new ProfileData();
        }

        if (!string.IsNullOrEmpty(profileData.currentProfileId))
        {
            CurrentProfile = profileData.profiles
                .FirstOrDefault(profile => profile.id == profileData.currentProfileId);
        }
        ProfilesLoaded?.Invoke();
    }

    private string GenerateUniqueId()
    {
        string id;

        do
        {
            id = "RF-" + Guid.NewGuid()
                .ToString("N")
                .Substring(0, 4)
                .ToUpper();

        } while (profileData.profiles.Any(profile => profile.id == id));

        return id;
    }
}