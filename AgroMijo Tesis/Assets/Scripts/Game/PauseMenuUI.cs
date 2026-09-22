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
    // El progreso ya está guardado (autosave después de cada ciclo),
    // así que solo hace falta cambiar de escena.
    public void OnClickSalirAlMenu()
    {
        SceneLoader.Instance.LoadMainMenu();
    }
}
