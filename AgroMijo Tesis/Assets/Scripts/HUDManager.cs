using UnityEngine;
using TMPro;

// Coloca este script en el panel superior (HUD).
public class HUDManager : MonoBehaviour
{
    [Header("Referencias de UI")]
    public TMP_Text textoCiclo;
    public TMP_Text textoPresupuesto;
    public ParcelaUI[] todasLasParcelasUI;

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
            parcelaUI.ActualizarVisual();
        }

        ReportePanelUI.Instancia.Mostrar(reporte);
    }

    private void ActualizarHUD()
    {
        textoCiclo.text = $"Ciclo {GameManager.Instancia.cicloActual}";
        textoPresupuesto.text = $"${GameManager.Instancia.presupuesto:N0}";
    }
}
