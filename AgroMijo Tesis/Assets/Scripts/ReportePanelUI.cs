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
        sb.AppendLine($"Gastos:    ${reporte.gastoTotal:N0}");

        // Jornales
        sb.AppendLine();
        sb.AppendLine($"Jornales necesarios: {reporte.jornalesNecesarios}");
        sb.AppendLine($"  Familiares: {reporte.jornalesFamiliaresUsados}");
        if (reporte.jornalesContratados > 0)
            sb.AppendLine($"  Contratados: {reporte.jornalesContratados}");
        if (reporte.modificadorJornales < 1f)
            sb.AppendLine($"  Rendimiento por jornales: {reporte.modificadorJornales * 100f:F0}%");

        if (reporte.detalleGastos.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("Desglose de gastos:");
            foreach (GastoRegistrado gasto in reporte.detalleGastos)
                sb.AppendLine($"  - {gasto.descripcion}: ${gasto.monto:N0}");
        }

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
