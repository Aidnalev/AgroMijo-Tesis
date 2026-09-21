using TMPro;
using UnityEngine;

public class DeleteProfileUI : MonoBehaviour
{
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private ProfilesPanelUI profilesPanelUI;

    private PlayerProfile profileToDelete;

    public void Open(PlayerProfile profile)
    {
        profileToDelete = profile;

        messageText.text = $"¿Seguro que quieres eliminar el perfil \"{profile.alias}\"?";

        confirmationPanel.SetActive(true);
    }

    public void ConfirmDelete()
    {
        if (profileToDelete == null)
            return;

        ProfileManager.Instance.DeleteProfile(profileToDelete.id);

        profileToDelete = null;

        confirmationPanel.SetActive(false);

        profilesPanelUI.RefreshProfiles();
    }

    public void Cancel()
    {
        profileToDelete = null;
        confirmationPanel.SetActive(false);
    }
}