using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject profilesPanel;
    [SerializeField] private SceneLoader sceneLoader;
    public void OpenProfiles()
    {
        mainMenuPanel.SetActive(false);
        profilesPanel.SetActive(true);
    }
    public void Play()
    {
        if (ProfileManager.Instance.CurrentProfile == null)
        {
            OpenProfiles();
            return;
        }

        sceneLoader.LoadGame();
    }
    public void CloseProfiles()
    {
        profilesPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}