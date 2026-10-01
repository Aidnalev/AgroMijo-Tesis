using TMPro;
using UnityEngine;

public class LoadProfileUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField idInput;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private ProfilesPanelUI profilesPanelUI;

    public void OpenPanel()
    {
        panel.SetActive(true);

        idInput.text = "";
        messageText.text = "";
    }

    public void LoadProfile()
    {
        string id = idInput.text.Trim().ToUpper();

        if (string.IsNullOrEmpty(id))
        {
            messageText.text = "Ingresa un ID.";
            return;
        }

        messageText.text = "Buscando perfil...";

        ProfileManager.Instance.LoadProfileFromServer(
            id,
            OnLoadComplete
        );
    }

    private void OnLoadComplete(bool success, string message)
    {
        if (success)
        {
            profilesPanelUI.RefreshProfiles();

            panel.SetActive(false);
            return;
        }

        messageText.text = message;
    }

    public void ClosePanel()
    {
        panel.SetActive(false);

        idInput.text = "";
        messageText.text = "";
    }
}