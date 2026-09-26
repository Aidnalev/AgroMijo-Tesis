using System.Text;
using UnityEngine;
using TMPro;

// Estructura del panel:
//   Txt_RazonFin      -> cómo terminó la partida
//   Txt_Resumen       -> estadísticas generales
//   Txt_ResumenCiclos -> tabla de ciclos (dentro de un Scroll View)
//   Btn_Exportar      -> OnClickExportar()
//   Btn_VolverMenu    -> OnClickVolverAlMenu()
public class PantallaFinPartidaUI : MonoBehaviour
{
    public static PantallaFinPartidaUI Instancia { get; private set; }

    public GameObject panelPrincipal;
    public TMP_Text   textoRazonFin;
    public TMP_Text   textoResumen;
    public TMP_Text   textoResumenCiclos; // dentro de un Scroll View

    private void Awake()
    {
        Instancia = this;
        panelPrincipal.SetActive(false);
    }

    public void Mostrar(GameOverData datos)
    {
        textoRazonFin.text = datos.razonFin;

        // ── Resumen general ───────────────────────────────────────────────
        float  diferencia = datos.presupuestoFinal - datos.presupuestoInicial;
        string signo      = diferencia >= 0 ? "+" : "";

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Perfil: {datos.profileAlias}");
        sb.AppendLine($"Fecha:  {datos.fechaPartida}");
        sb.AppendLine($"Ciclos jugados: {datos.ciclosJugados}");
        sb.AppendLine();
        sb.AppendLine($"Presupuesto inicial: ${datos.presupuestoInicial:N0}");
        sb.AppendLine($"Presupuesto final:   ${datos.presupuestoFinal:N0}");
        sb.AppendLine($"Diferencia:          {signo}${System.Math.Abs(diferencia):N0}");
        sb.AppendLine();
        sb.AppendLine($"Ganancias totales: ${datos.gananciaAcumulada:N0}");
        sb.AppendLine($"Gastos totales:    ${datos.gastoAcumulado:N0}");

        textoResumen.text = sb.ToString();

        // ── Resumen por ciclo ─────────────────────────────────────────────
        StringBuilder sbCiclos = new StringBuilder();
        sbCiclos.AppendLine("RESUMEN POR CICLO");
        sbCiclos.AppendLine(new string('-', 60));

        foreach (ReporteCiclo ciclo in datos.historialCiclos)
        {
            float  difCiclo    = ciclo.presupuestoFinal - ciclo.presupuestoInicial;
            string signoCiclo  = difCiclo >= 0 ? "+" : "-";
            string estadoCiclo = ciclo.esUltimoCiclo ? " [FIN]" : "";

            sbCiclos.AppendLine(
                $"Ciclo {ciclo.numeroCiclo + 1}{estadoCiclo}");
            sbCiclos.AppendLine(
                $"  Presupuesto: ${ciclo.presupuestoInicial:N0} -> ${ciclo.presupuestoFinal:N0}  " +
                $"({signoCiclo}${System.Math.Abs(difCiclo):N0})");
            sbCiclos.AppendLine(
                $"  Ganancias: ${ciclo.gananciaTotal:N0}  |  Gastos: ${ciclo.gastoTotal:N0}");

            if (ciclo.jornalesNecesarios > 0)
                sbCiclos.AppendLine(
                    $"  Jornales: {ciclo.jornalesNecesarios} necesarios" +
                    (ciclo.jornalesContratados > 0
                        ? $", {ciclo.jornalesContratados} contratados"
                        : "") +
                    (ciclo.modificadorJornales < 1f
                        ? $"  [Rendimiento: {ciclo.modificadorJornales * 100f:F0}%]"
                        : ""));

            if (ciclo.cosechasRealizadas.Count > 0)
            {
                sbCiclos.AppendLine("  Cosechas:");
                foreach (string c in ciclo.cosechasRealizadas)
                    sbCiclos.AppendLine($"    · {c}");
            }

            if (ciclo.detalleGastos.Count > 0)
            {
                sbCiclos.AppendLine("  Gastos:");
                foreach (GastoRegistrado g in ciclo.detalleGastos)
                    sbCiclos.AppendLine($"    · {g.descripcion}: ${g.monto:N0}");
            }

            if (ciclo.eventosOcurridos.Count > 0)
            {
                sbCiclos.AppendLine("  Eventos:");
                foreach (string e in ciclo.eventosOcurridos)
                    sbCiclos.AppendLine($"    · {e}");
            }

            sbCiclos.AppendLine();
        }

        textoResumenCiclos.text = sbCiclos.ToString();
        panelPrincipal.SetActive(true);
    }

    // Conecta al botón "Exportar datos"
    public void OnClickExportar()
    {
        string profileId = ProfileManager.Instance?.CurrentProfile?.id;
        if (string.IsNullOrEmpty(profileId)) return;

        string ruta = ExportManager.ExportarCSV(profileId);

        PanelNotificacionUI.Instancia.Mostrar(ruta != null
            ? "Exportacion completada.\n\nArchivo guardado en:\n" + ruta
            : "No hay partidas para exportar aun.");
    }

    // Conecta al botón "Volver al menú"
    public void OnClickVolverAlMenu()
    {
        SceneLoader.Instance.LoadMainMenu();
    }
}
