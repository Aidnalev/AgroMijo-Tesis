using UnityEngine;
using TMPro;
using System.Text;

// Coloca este script en el panel de reporte (inicialmente activo,
// el propio script lo oculta en Awake()).
public class ReportePanelUI : MonoBehaviour
{
    public static ReportePanelUI Instancia { get; private set; }

    public GameObject panelPrincipal;
    public TMP_Text textoResumen;

    private void Awake()
    {
        Instancia = this;
        panelPrincipal.SetActive(false);
    }

    public void Mostrar(ReporteCiclo reporte)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Resultado del ciclo {reporte.numeroCiclo}");
        sb.AppendLine($"Presupuesto inicial: ${reporte.presupuestoInicial:N0}");
        sb.AppendLine($"Presupuesto final: ${reporte.presupuestoFinal:N0}");
        sb.AppendLine($"Ganancias: ${reporte.gananciaTotal:N0}");
        sb.AppendLine($"Gastos: ${reporte.gastoTotal:N0}");

        if (reporte.cosechasRealizadas.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("Cosechas:");
            foreach (string cosecha in reporte.cosechasRealizadas) sb.AppendLine("- " + cosecha);
        }

        if (reporte.eventosOcurridos.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("Eventos:");
            foreach (string evento in reporte.eventosOcurridos) sb.AppendLine("- " + evento);
        }

        textoResumen.text = sb.ToString();
        panelPrincipal.SetActive(true);
    }

    // Conecta esto al botón "Continuar" del panel de reporte
    public void Cerrar()
    {
        panelPrincipal.SetActive(false);
    }
}
