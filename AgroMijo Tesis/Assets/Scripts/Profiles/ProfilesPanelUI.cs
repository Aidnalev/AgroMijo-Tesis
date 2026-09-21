using UnityEngine;

public class ProfilesPanelUI : MonoBehaviour
{
    [SerializeField] private Transform profileList;
    [SerializeField] private GameObject profileButtonPrefab;
    [SerializeField] private DeleteProfileUI deleteProfileUI;
    private void OnEnable()
    {
        RefreshProfiles();
    }

    public void RefreshProfiles()
    {
        foreach (Transform child in profileList)
        {
            Destroy(child.gameObject);
        }

        foreach (PlayerProfile profile in ProfileManager.Instance.profileData.profiles)
        {
            GameObject profileObject =
                Instantiate(profileButtonPrefab, profileList);

            ProfileButtonUI profileButton =
                profileObject.GetComponent<ProfileButtonUI>();

            profileButton.Setup(profile, this);

            bool isSelected =
                ProfileManager.Instance.CurrentProfile != null &&
                ProfileManager.Instance.CurrentProfile.id == profile.id;

            profileButton.SetSelected(isSelected);
        }
    }
    public void RequestDelete(PlayerProfile profile)
    {
        deleteProfileUI.Open(profile);
    }
}