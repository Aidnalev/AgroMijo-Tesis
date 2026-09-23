using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instancia { get; private set; }

    [Header("Referencias de UI")]
    public TMP_Text    textoCiclo;
    public TMP_Text    textoPresupuesto;
    public TMP_Text    textoJornales;   // muestra "Jornales: X / Y" en tiempo real
    public ParcelaUI[] todasLasParcelasUI;

    private void Awake()
    {
        Instancia = this;
    }

    private void Start()
    {
        ActualizarHUD();
    }

    // Conecta al botón "Avanzar ciclo"
    // Flujo: cerrar panel → revisar decisiones → revisar jornales → ejecutar
    public void OnClickAvanzarCiclo()
    {
        PanelDecisionUI.Instancia.Cerrar();

        if (!GameManager.Instancia.TodasLasParcelasTienenDecision())
            PanelAdvertenciaUI.Instancia.Mostrar(VerificarJornales);
        else
            VerificarJornales();
    }

    private void VerificarJornales()
    {
        int necesarios = GameManager.Instancia.CalcularJornalesNecesarios();
        int deficit    = necesarios - GameManager.Instancia.jornalesFamiliares;

        if (deficit > 0)
            PanelJornalesUI.Instancia.Mostrar(deficit, EjecutarAvanceCiclo);
        else
            EjecutarAvanceCiclo(0);
    }

    private void EjecutarAvanceCiclo(int jornalesContratados)
    {
        ReporteCiclo reporte = GameManager.Instancia.AvanzarCiclo(jornalesContratados);

        ActualizarHUD();

        foreach (ParcelaUI parcelaUI in todasLasParcelasUI)
            parcelaUI.ActualizarVisual();

        if (reporte.esUltimoCiclo)
            ReportePanelUI.Instancia.Mostrar(reporte, onCerrar: MostrarPantallaFin);
        else
            ReportePanelUI.Instancia.Mostrar(reporte);
    }

    private void MostrarPantallaFin()
    {
        GameOverData registro = GameManager.Instancia.CrearRegistroFinal(
            GameManager.Instancia.historialCiclos[^1].razonFin);

        RegistroPartidasManager.GuardarRegistro(registro);

        string profileId = ProfileManager.Instance?.CurrentProfile?.id;
        if (!string.IsNullOrEmpty(profileId))
            SaveManager.Borrar(profileId);

        PantallaFinPartidaUI.Instancia.Mostrar(registro);
    }

    public void ActualizarHUD()
    {
        textoCiclo.text       = $"Ciclo {GameManager.Instancia.cicloActual}";
        textoPresupuesto.text = $"${GameManager.Instancia.presupuesto:N0}";

        int comprometidos = GameManager.Instancia.CalcularJornalesNecesarios();
        int disponibles   = GameManager.Instancia.jornalesFamiliares;
        textoJornales.text  = $"Jornales: {comprometidos} / {disponibles}";

        // Rojo si hay déficit, blanco si alcanza
        textoJornales.color = comprometidos > disponibles
            ? new UnityEngine.Color(1f, 0.35f, 0.35f)
            : UnityEngine.Color.white;
    }
}
