using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Coloca este script en el panel superior (HUD) que muestra
// el ciclo actual, el presupuesto y el botón de avanzar ciclo.
public class HUDManager : MonoBehaviour
{
    [Header("Referencias de UI")]
    public TMP_Text textoCiclo;
    public TMP_Text textoPresupuesto;
    public ParcelaUI[] todasLasParcelasUI; // arrastra aquí todas las tarjetas de parcela

    private void Start()
    {
        ActualizarHUD();
    }

    // Conecta esto al botón "Avanzar ciclo"
    public void OnClickAvanzarCiclo()
    {
        ReporteCiclo reporte = GameManager.Instancia.AvanzarCiclo();

        ActualizarHUD();

        foreach (ParcelaUI parcelaUI in todasLasParcelasUI)
        {
            parcelaUI.ActualizarVisual(); // todos los checks vuelven a apagarse
        }

        ReportePanelUI.Instancia.Mostrar(reporte);
    }

    private void ActualizarHUD()
    {
        textoCiclo.text = $"Ciclo {GameManager.Instancia.cicloActual}";
        textoPresupuesto.text = $"${GameManager.Instancia.presupuesto:N0}";
    }
}
