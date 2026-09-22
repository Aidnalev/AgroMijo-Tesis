using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject profilesPanel;
    [SerializeField] private SceneLoader sceneLoader;

    [Header("Botones")]
    [SerializeField] private Button botonContinuar; // visible solo si hay guardado activo

    private void Start()
    {
        ProfileManager.Instance.ProfilesLoaded += RefrescarBotones;
        RefrescarBotones();
    }

    private void OnDestroy()
    {
        if (ProfileManager.Instance != null)
            ProfileManager.Instance.ProfilesLoaded -= RefrescarBotones;
    }

    // "Nueva Partida" — siempre visible
    // Sin perfil → va a selección de perfiles
    // Con perfil → borra guardado si existe y empieza desde cero
    public void NuevaPartida()
    {
        if (ProfileManager.Instance.CurrentProfile == null)
        {
            OpenProfiles();
            return;
        }

        SaveManager.Borrar(ProfileManager.Instance.CurrentProfile.id);
        sceneLoader.LoadGame();
    }

    // "Continuar" — solo visible si hay guardado activo
    // GameManager lo cargará automáticamente al entrar a la escena
    public void Continuar()
    {
        if (ProfileManager.Instance.CurrentProfile == null) return;
        sceneLoader.LoadGame();
    }

    private void RefrescarBotones()
    {
        if (botonContinuar == null) return;

        bool hayPerfil   = ProfileManager.Instance.CurrentProfile != null;
        bool hayGuardado = hayPerfil &&
            SaveManager.ExisteGuardado(ProfileManager.Instance.CurrentProfile.id);

        botonContinuar.gameObject.SetActive(hayPerfil && hayGuardado);
    }

    public void OpenProfiles()
    {
        mainMenuPanel.SetActive(false);
        profilesPanel.SetActive(true);
    }

    public void CloseProfiles()
    {
        profilesPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        RefrescarBotones();
    }
}
