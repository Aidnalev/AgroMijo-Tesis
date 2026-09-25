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
    // Con perfil → guarda registro de la partida anterior si existe, luego empieza desde cero
    public void NuevaPartida()
    {
        if (ProfileManager.Instance.CurrentProfile == null)
        {
            OpenProfiles();
            return;
        }

        string profileId = ProfileManager.Instance.CurrentProfile.id;

        if (SaveManager.ExisteGuardado(profileId))
        {
            GameSaveData saveData = SaveManager.Cargar(profileId);
            if (saveData != null)
                RegistroPartidasManager.GuardarRegistro(CrearRegistroDesdeSave(saveData));

            SaveManager.Borrar(profileId);
        }

        sceneLoader.LoadGame();
    }

    private GameOverData CrearRegistroDesdeSave(GameSaveData save)
    {
        float gananciaTotal = 0f;
        float gastoTotal    = 0f;
        foreach (ReporteCiclo r in save.historialCiclos)
        {
            gananciaTotal += r.gananciaTotal;
            gastoTotal    += r.gastoTotal;
        }

        float presupuestoInicio = save.historialCiclos.Count > 0
            ? save.historialCiclos[0].presupuestoInicial
            : save.presupuesto;

        return new GameOverData
        {
            profileId          = save.profileId,
            profileAlias       = ProfileManager.Instance.CurrentProfile.alias,
            fechaPartida       = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
            ciclosJugados      = save.cicloActual,
            razonFin           = "Partida abandonada al iniciar una nueva.",
            presupuestoInicial = presupuestoInicio,
            presupuestoFinal   = save.presupuesto,
            gananciaAcumulada  = gananciaTotal,
            gastoAcumulado     = gastoTotal,
            historialCiclos    = save.historialCiclos
        };
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
