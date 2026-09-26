using UnityEngine;
using TMPro;

// Panel simple para mostrar un mensaje informativo con un botón de cierre.
// Estructura:
//   Txt_Mensaje  → el mensaje a mostrar
//   Btn_Cerrar   → OnClickCerrar()
public class PanelNotificacionUI : MonoBehaviour
{
    public static PanelNotificacionUI Instancia { get; private set; }

    public GameObject panelPrincipal;
    public TMP_Text   textoMensaje;

    private void Awake()
    {
        Instancia = this;
        panelPrincipal.SetActive(false);
    }

    public void Mostrar(string mensaje)
    {
        textoMensaje.text = mensaje;
        panelPrincipal.SetActive(true);
    }

    // Conecta al botón "Cerrar" o "Entendido"
    public void OnClickCerrar()
    {
        panelPrincipal.SetActive(false);
    }
}
