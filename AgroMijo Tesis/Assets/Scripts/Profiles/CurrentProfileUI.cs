using TMPro;
using UnityEngine;

public class CurrentProfileUI : MonoBehaviour
{
    [SerializeField] private TMP_Text profileText;

    private void Start()
    {
        ProfileManager.Instance.ProfilesLoaded += Refresh;

        Refresh();
    }

    private void OnDestroy()
    {
        if (ProfileManager.Instance != null)
        {
            ProfileManager.Instance.ProfilesLoaded -= Refresh;
        }
    }

    public void Refresh()
    {
        PlayerProfile profile = ProfileManager.Instance.CurrentProfile;

        if (profile == null)
        {
            profileText.text = "Perfil: Ninguno";
        }
        else
        {
            profileText.text = $"Perfil: {profile.alias}";
        }
    }
}