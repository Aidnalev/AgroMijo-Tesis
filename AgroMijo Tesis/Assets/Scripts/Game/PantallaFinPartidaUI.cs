using System.Text;
using UnityEngine;
using TMPro;

// Coloca este script en Panel_FinPartida (inicialmente activo, se oculta en Awake).
// Se muestra automáticamente cuando termina la partida.
// Estructura:
//   Txt_RazonFin   → cómo terminó la partida
//   Txt_Resumen    → estadísticas finales
//   Btn_VolverMenu → OnClickVolverAlMenu()
public class PantallaFinPartidaUI : MonoBehaviour
{
    public static PantallaFinPartidaUI Instancia { get; private set; }

    public GameObject panelPrincipal;
    public TMP_Text   textoRazonFin;
    public TMP_Text   textoResumen;

    private void Awake()
    {
        Instancia = this;
        panelPrincipal.SetActive(false);
    }

    public void Mostrar(GameOverData datos)
    {
        textoRazonFin.text = datos.razonFin;

        float diferencia = datos.presupuestoFinal - datos.presupuestoInicial;
        string signo     = diferencia >= 0 ? "+" : "";

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Perfil: {datos.profileAlias}");
        sb.AppendLine($"Ciclos jugados: {datos.ciclosJugados}");
        sb.AppendLine();
        sb.AppendLine($"Presupuesto inicial: ${datos.presupuestoInicial:N0}");
        sb.AppendLine($"Presupuesto final:   ${datos.presupuestoFinal:N0}");
        sb.AppendLine($"Diferencia:          {signo}${diferencia:N0}");
        sb.AppendLine();
        sb.AppendLine($"Ganancias totales: ${datos.gananciaAcumulada:N0}");
        sb.AppendLine($"Gastos totales:    ${datos.gastoAcumulado:N0}");
        sb.AppendLine();
        sb.AppendLine($"Partida guardada el {datos.fechaPartida}");

        textoResumen.text = sb.ToString();
        panelPrincipal.SetActive(true);
    }

    // Conecta al botón "Volver al menú"
    public void OnClickVolverAlMenu()
    {
        SceneLoader.Instance.LoadMainMenu();
    }
}
