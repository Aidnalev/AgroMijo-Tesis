using UnityEngine;
using TMPro;
using System;

// Coloca este script en un panel nuevo, Panel_Advertencia (estructura igual
// a Panel_Reporte: un fondo + texto + dos botones). Inicialmente activo en
// la jerarquía, el propio script lo oculta en Awake().
public class PanelAdvertenciaUI : MonoBehaviour
{
    public static PanelAdvertenciaUI Instancia { get; private set; }

    public GameObject panelPrincipal;
    public TMP_Text textoMensaje;

    private Action accionSiConfirma;

    private void Awake()
    {
        Instancia = this;
        panelPrincipal.SetActive(false);
    }

    public void Mostrar(Action alConfirmar,
        string mensaje = "Hay parcelas sin decision este ciclo. Avanzar de todas formas?")
    {
        accionSiConfirma  = alConfirmar;
        textoMensaje.text = mensaje;
        panelPrincipal.SetActive(true);
    }

    // Conecta esto al botón "Sí, avanzar"
    public void OnClickConfirmar()
    {
        panelPrincipal.SetActive(false);
        accionSiConfirma?.Invoke();
    }

    // Conecta esto al botón "Cancelar"
    public void OnClickCancelar()
    {
        panelPrincipal.SetActive(false);
    }
}
