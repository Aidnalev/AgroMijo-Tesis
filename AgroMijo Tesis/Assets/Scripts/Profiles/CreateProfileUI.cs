using TMPro;
using UnityEngine;

public class CreateProfileUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField aliasInput;
    [SerializeField] private GameObject createProfilePanel;
    [SerializeField] private ProfilesPanelUI profilesPanelUI;

    public void Open()
    {
        createProfilePanel.SetActive(true);
        aliasInput.Select();
    }

    public void CreateProfile()
    {
        string alias = aliasInput.text.Trim();

        if (string.IsNullOrEmpty(alias))
        {
            Debug.Log("El alias no puede estar vacío.");
            return;
        }

        ProfileManager.Instance.CreateProfile(alias);

        aliasInput.text = "";

        createProfilePanel.SetActive(false);

        profilesPanelUI.RefreshProfiles();
    }

    public void Cancel()
    {
        aliasInput.text = "";
        createProfilePanel.SetActive(false);
    }
}