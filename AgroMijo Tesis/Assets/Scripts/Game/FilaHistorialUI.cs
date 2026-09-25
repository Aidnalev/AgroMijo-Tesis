using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Prefab simplificado — ya no necesita panel de detalle propio.
// Al hacer clic en "Ver detalle" abre el ReportePanelUI existente.
// Estructura del prefab:
//   Txt_Encabezado → "Ciclo 3"
//   Txt_Resumen    → números del ciclo en una línea
//   Btn_Detalle    → "Ver detalle"
public class FilaHistorialUI : MonoBehaviour
{
    public TMP_Text textoEncabezado;
    public TMP_Text textoResumen;
    public Button   botonDetalle;

    private ReporteCiclo reporteAsociado;

    public void Setup(ReporteCiclo reporte)
    {
        reporteAsociado = reporte;

        textoEncabezado.text = $"Ciclo {reporte.numeroCiclo + 1}";

        textoResumen.text =
            $"Inicio: ${reporte.presupuestoInicial:N0}  →  " +
            $"Final: ${reporte.presupuestoFinal:N0}  |  " +
            $"Ganancias: ${reporte.gananciaTotal:N0}  |  " +
            $"Gastos: ${reporte.gastoTotal:N0}";

        botonDetalle.onClick.AddListener(VerDetalle);
    }

    private void VerDetalle()
    {
        PanelHistorialUI.Instancia.Cerrar();
        ReportePanelUI.Instancia.Mostrar(reporteAsociado);
    }
}
