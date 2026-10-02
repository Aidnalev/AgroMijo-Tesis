using System;
using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

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

        StartCoroutine(SyncPendingProfiles());
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

        StartCoroutine(SyncProfile(newProfile));
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

    public void LoadProfileFromServer(
        string id,
        Action<bool, string> onComplete)
    {
        StartCoroutine(
            LoadProfileFromServerCoroutine(id, onComplete)
        );
    }

    private IEnumerator LoadProfileFromServerCoroutine(
        string id,
        Action<bool, string> onComplete)
    {
        string url = $"{ApiConfig.API_URL}/api/profiles/{id}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                PlayerProfile profile =
                    JsonUtility.FromJson<PlayerProfile>(
                        request.downloadHandler.text
                    );

                // Este perfil ya sabemos que existe en MongoDB.
                profile.sincronizado = true;

                PlayerProfile localProfile =
                    profileData.profiles
                        .FirstOrDefault(p => p.id == profile.id);

                if (localProfile != null)
                {
                    SelectProfile(profile.id);

                    // Aseguramos que la copia local se considere sincronizada.
                    localProfile.sincronizado = true;
                    SaveProfiles();

                    onComplete?.Invoke(
                        true,
                        "Este perfil ya estaba cargado y ha sido seleccionado."
                    );

                    yield break;
                }

                profileData.profiles.Add(profile);

                CurrentProfile = profile;
                profileData.currentProfileId = profile.id;

                SaveProfiles();

                ProfilesLoaded?.Invoke();

                onComplete?.Invoke(
                    true,
                    "Perfil cargado correctamente."
                );
            }
            else if (request.responseCode == 404)
            {
                onComplete?.Invoke(
                    false,
                    "No existe un perfil con ese ID."
                );
            }
            else
            {
                Debug.LogError(
                    $"Error al cargar el perfil: {request.error}"
                );

                onComplete?.Invoke(
                    false,
                    "No se pudo conectar con el servidor."
                );
            }
        }
    }

    private IEnumerator SyncProfile(PlayerProfile profile)
    {
        string url = $"{ApiConfig.API_URL}/api/profiles";

        ProfileRequest profileRequest =
            new ProfileRequest(profile.id, profile.alias);

        string json = JsonUtility.ToJson(profileRequest);

        using (UnityWebRequest request =
               new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw =
                System.Text.Encoding.UTF8.GetBytes(json);

            request.uploadHandler =
                new UploadHandlerRaw(bodyRaw);

            request.downloadHandler =
                new DownloadHandlerBuffer();

            request.SetRequestHeader(
                "Content-Type",
                "application/json"
            );

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                profile.sincronizado = true;

                SaveProfiles();

                Debug.Log(
                    $"Perfil {profile.id} sincronizado con MongoDB."
                );
            }
            else if (request.responseCode == 409)
            {
                // Ya existe en MongoDB.
                // Para nuestro propósito, está sincronizado.
                profile.sincronizado = true;

                SaveProfiles();

                Debug.Log(
                    $"El perfil {profile.id} ya existía en MongoDB."
                );
            }
            else
            {
                Debug.LogWarning(
                    $"No se pudo sincronizar el perfil " +
                    $"{profile.id}. Quedará pendiente."
                );
            }
        }
    }

    private IEnumerator SyncPendingProfiles()
    {
        yield return new WaitForSeconds(1f);

        foreach (PlayerProfile profile in profileData.profiles)
        {
            if (!profile.sincronizado)
            {
                yield return StartCoroutine(
                    SyncProfile(profile)
                );
            }
        }
    }

    private void SaveProfiles()
    {
        string json = JsonUtility.ToJson(
            profileData,
            true
        );

        File.WriteAllText(
            savePath,
            json
        );
    }

    private void LoadProfiles()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);

            profileData =
                JsonUtility.FromJson<ProfileData>(json);

            if (profileData == null)
            {
                profileData = new ProfileData();
            }
        }
        else
        {
            profileData = new ProfileData();
        }

        if (profileData.profiles == null)
        {
            profileData.profiles =
                new System.Collections.Generic.List<PlayerProfile>();
        }

        if (!string.IsNullOrEmpty(
            profileData.currentProfileId))
        {
            CurrentProfile =
                profileData.profiles.FirstOrDefault(
                    profile =>
                        profile.id ==
                        profileData.currentProfileId
                );
        }

        ProfilesLoaded?.Invoke();
    }

    private string GenerateUniqueId()
    {
        string id;

        do
        {
            id = "RF-" +
                 Guid.NewGuid()
                     .ToString("N")
                     .Substring(0, 4)
                     .ToUpper();

        } while (
            profileData.profiles
                .Any(profile => profile.id == id)
        );

        return id;
    }
}

[Serializable]
public class ProfileRequest
{
    public string id;
    public string alias;

    public ProfileRequest(string id, string alias)
    {
        this.id = id;
        this.alias = alias;
    }
}