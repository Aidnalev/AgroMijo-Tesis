using UnityEngine;
using UnityEngine.InputSystem;

// Coloca este script en un Panel_Pausa (inicialmente activo, se oculta en Awake).
// No necesita botón para abrirse — detecta ESC automáticamente.
// Estructura del panel:
//   - Btn_Reanudar  → OnClickReanudar()
//   - Btn_Salir     → OnClickSalirAlMenu()
public class PauseMenuUI : MonoBehaviour
{
    public static PauseMenuUI Instancia { get; private set; }

    public GameObject panelPrincipal;

    private bool pausado = false;

    private void Awake()
    {
        Instancia = this;
        panelPrincipal.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (pausado) Reanudar();
            else Pausar();
        }
    }

    private void Pausar()
    {
        pausado = true;

        // Cerrar paneles que puedan estar abiertos
        PanelDecisionUI.Instancia.Cerrar();
        PanelHistorialUI.Instancia.Cerrar();
        PanelCatalogoCultivosUI.Instancia.Cerrar();
        PanelEventosUI.Instancia.Cerrar();

        panelPrincipal.SetActive(true);
    }

    // Conecta al botón "Reanudar"
    public void OnClickReanudar()
    {
        Reanudar();
    }

    private void Reanudar()
    {
        pausado = false;
        panelPrincipal.SetActive(false);
    }

    // Conecta al botón "Salir al menú"
    // El progreso ya está guardado (autosave después de cada ciclo)
    public void OnClickSalirAlMenu()
    {
        SceneLoader.Instance.LoadMainMenu();
    }

    // Conecta al botón "Terminar Partida"
    // Pide confirmación, guarda el registro final y muestra la pantalla de fin
    public void OnClickTerminarPartida()
    {
        PanelAdvertenciaUI.Instancia.Mostrar(
            alConfirmar: () =>
            {
                string razon   = "Partida terminada manualmente.";
                GameOverData r = GameManager.Instancia.CrearRegistroFinal(razon);
                RegistroPartidasManager.GuardarRegistro(r);

                string profileId = ProfileManager.Instance?.CurrentProfile?.id;
                if (!string.IsNullOrEmpty(profileId))
                    SaveManager.Borrar(profileId);

                Reanudar(); // cierra el menú de pausa
                PantallaFinPartidaUI.Instancia.Mostrar(r);
            },
            mensaje: "Terminar la partida ahora guardara el registro pero no podras continuar desde este punto. Continuar?"
        );
    }
}
