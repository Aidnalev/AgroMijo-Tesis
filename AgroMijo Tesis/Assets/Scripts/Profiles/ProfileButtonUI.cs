using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileButtonUI : MonoBehaviour
{
    [SerializeField] private TMP_Text aliasText;
    [SerializeField] private TMP_Text idText;
    [SerializeField] private Button button;
    [SerializeField] private Button deleteButton;
    [SerializeField] private GameObject selectedIndicator;

    private PlayerProfile profile;
    private ProfilesPanelUI panelUI; // referencia directa, sin FindAnyObjectByType

    public void Setup(PlayerProfile profile, ProfilesPanelUI panel)
    {
        this.profile = profile;
        this.panelUI = panel;

        aliasText.text = profile.alias;
        idText.text    = profile.id;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(SelectProfile);

        deleteButton.onClick.RemoveAllListeners();
        deleteButton.onClick.AddListener(DeleteProfile);
    }

    public void SetSelected(bool selected)
    {
        selectedIndicator.SetActive(selected);
        button.interactable = !selected;
    }

    private void SelectProfile()
    {
        ProfileManager.Instance.SelectProfile(profile.id);
        panelUI.RefreshProfiles();
    }

    private void DeleteProfile()
    {
        panelUI.RequestDelete(profile);
    }
}
