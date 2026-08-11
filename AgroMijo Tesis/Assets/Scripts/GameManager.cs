using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia { get; private set; }

    [Header("Configuración inicial")]
    public List<CultivoData> cultivosDisponibles;
    public List<EventoGlobalData> eventosPosibles;
    public List<Parcela> parcelas = new List<Parcela>();

    [Header("Costos de acciones por ciclo (ajustar con datos reales de la Provincia Comunera)")]
    public float costoEstudio = 30000f;
    public float costoMejora  = 80000f;

    [Header("Estado del juego")]
    public int   cicloActual  = 0;
    public float presupuesto  = 1000000f;

    // Se guarda al final de cada ciclo (después de cosechas y eventos),
    // para usarlo como "presupuesto inicial" del reporte del ciclo siguiente.
    [HideInInspector] public float presupuestoAlIniciarCiclo = 1000000f;

    public List<ReporteCiclo>       historialCiclos            = new List<ReporteCiclo>();
    public List<EventoGlobalActivo> eventosActivosPersistentes = new List<EventoGlobalActivo>();

    private void Awake()
    {
        if (Instancia != null && Instancia != this) { Destroy(gameObject); return; }
        Instancia = this;
    }

    // ══════════════════════════════════════════════════════════════════════════
    // GUARDAR DECISIONES
    // Estos métodos solo registran la intención del jugador.
    // No descuentan dinero ni modifican el estado de la parcela todavía.
    // Todo se aplica en AplicarDecisionesPendientes() al avanzar el ciclo.
    // ══════════════════════════════════════════════════════════════════════════

    public void GuardarDecisionPlantar(Parcela parcela, CultivoData cultivo)
    {
        if (parcela.estado == EstadoParcela.Plantada) return; // ya hay algo creciendo

        parcela.decisionPendiente.tipo              = TipoDecision.Plantar;
        parcela.decisionPendiente.cultivoSeleccionado = cultivo;
        parcela.decisionTomada                      = true;
    }

    public void GuardarDecisionEstudiar(Parcela parcela)
    {
        // Se puede estudiar aunque haya un cultivo creciendo (una acción por ciclo)
        parcela.decisionPendiente.tipo              = TipoDecision.Estudiar;
        parcela.decisionPendiente.cultivoSeleccionado = null;
        parcela.decisionTomada                      = true;
    }

    public void GuardarDecisionMejorar(Parcela parcela)
    {
        // Se puede mejorar aunque haya un cultivo creciendo (una acción por ciclo)
        parcela.decisionPendiente.tipo              = TipoDecision.Mejorar;
        parcela.decisionPendiente.cultivoSeleccionado = null;
        parcela.decisionTomada                      = true;
    }

    public void GuardarDecisionEsperar(Parcela parcela)
    {
        parcela.decisionPendiente.tipo              = TipoDecision.Esperar;
        parcela.decisionPendiente.cultivoSeleccionado = null;
        parcela.decisionTomada                      = true;
    }

    // ══════════════════════════════════════════════════════════════════════════
    // AVANZAR CICLO
    // Orden: decisiones → eventos → cosechas → cierre del ciclo
    // ══════════════════════════════════════════════════════════════════════════

    public ReporteCiclo AvanzarCiclo()
    {
        ReporteCiclo reporte = new ReporteCiclo
        {
            numeroCiclo      = cicloActual,
            presupuestoInicial = presupuestoAlIniciarCiclo
        };

        AplicarDecisionesPendientes(reporte);   // 1. cobra y ejecuta lo que el jugador decidió en parcelas
        AplicarResolucionesPendientes(reporte); // 2. cobra y resuelve eventos marcados por el jugador
        ResolverEventosGlobales(reporte);       // 3. eventos del entorno (uno por ciclo)
        ResolverCosechas(reporte);              // 4. cosecha lo que ya maduró

        reporte.presupuestoFinal = presupuesto;
        historialCiclos.Add(reporte);
        cicloActual++;

        presupuestoAlIniciarCiclo = presupuesto; // base para el reporte del ciclo siguiente

        return reporte;
    }

    private void AplicarDecisionesPendientes(ReporteCiclo reporte)
    {
        foreach (Parcela parcela in parcelas)
        {
            switch (parcela.decisionPendiente.tipo)
            {
                case TipoDecision.Plantar:
                    CultivoData cultivo = parcela.decisionPendiente.cultivoSeleccionado;
                    if (cultivo != null
                        && parcela.estado == EstadoParcela.Vacia
                        && presupuesto >= cultivo.costoSemilla)
                    {
                        presupuesto -= cultivo.costoSemilla;
                        reporte.gastoTotal += cultivo.costoSemilla;
                        reporte.detalleGastos.Add(new GastoRegistrado
                        {
                            descripcion = $"Semilla: {cultivo.nombreCultivo} en {parcela.nombreParcela}",
                            monto       = cultivo.costoSemilla,
                            categoria   = CategoriaGasto.Semilla
                        });
                        parcela.Plantar(cultivo, cicloActual);
                    }
                    break;

                case TipoDecision.Estudiar:
                    if (!parcela.estudiada && presupuesto >= costoEstudio)
                    {
                        presupuesto -= costoEstudio;
                        reporte.gastoTotal += costoEstudio;
                        reporte.detalleGastos.Add(new GastoRegistrado
                        {
                            descripcion = $"Estudio de suelo: {parcela.nombreParcela}",
                            monto       = costoEstudio,
                            categoria   = CategoriaGasto.Estudio
                        });
                        parcela.estudiada = true;
                    }
                    break;

                case TipoDecision.Mejorar:
                    if (presupuesto >= costoMejora)
                    {
                        presupuesto -= costoMejora;
                        reporte.gastoTotal += costoMejora;
                        reporte.detalleGastos.Add(new GastoRegistrado
                        {
                            descripcion = $"Mejora de drenaje: {parcela.nombreParcela}",
                            monto       = costoMejora,
                            categoria   = CategoriaGasto.Mejora
                        });
                        // lógica concreta de mejora se define cuando desarrollemos ese sistema
                    }
                    break;

                // TipoDecision.Esperar y Ninguna: no hacen nada, no cobran nada
            }

            // Limpiar para el ciclo siguiente
            parcela.decisionPendiente.tipo              = TipoDecision.Ninguna;
            parcela.decisionPendiente.cultivoSeleccionado = null;
            parcela.decisionTomada                      = false;
        }
    }

    private void ResolverEventosGlobales(ReporteCiclo reporte)
    {
        // Avanzar eventos persistentes ya activos
        foreach (EventoGlobalActivo activo in eventosActivosPersistentes)
        {
            if (activo.resuelto) continue;
            activo.ciclosActivo++;

            if (activo.datos.ciclosHastaAutoResolver > 0
                && activo.ciclosActivo >= activo.datos.ciclosHastaAutoResolver)
            {
                activo.resuelto = true;
                reporte.eventosOcurridos.Add($"[Resuelto] {activo.datos.nombreEvento}");
            }
            else
            {
                reporte.eventosOcurridos.Add($"[Continúa] {activo.datos.nombreEvento}");
            }
        }

        // Evaluar si ocurre algún evento nuevo este ciclo
        foreach (EventoGlobalData evento in eventosPosibles)
        {
            if (Random.value > evento.probabilidadPorCiclo) continue;

            EventoGlobalActivo nuevo = new EventoGlobalActivo
            {
                datos            = evento,
                cicloEnQueOcurrio = cicloActual
            };

            if (evento.esPersistente)
                eventosActivosPersistentes.Add(nuevo);

            reporte.eventosOcurridos.Add($"[Nuevo] {evento.nombreEvento}: {evento.descripcionAlOcurrir}");
        }
    }

    private void ResolverCosechas(ReporteCiclo reporte)
    {
        foreach (Parcela parcela in parcelas)
        {
            if (!parcela.ListaParaCosecha(cicloActual)) continue;

            CultivoData cultivo          = parcela.cultivoActual;
            float modificadorSuelo       = cultivo.ObtenerModificadorPorSuelo(parcela.tipoSuelo);
            float modificadorAgua        = Mathf.Lerp(0.5f, 1f, parcela.disponibilidadAgua);
            float modificadorEventos     = 1f;

            foreach (EventoGlobalActivo activo in eventosActivosPersistentes)
            {
                if (activo.resuelto) continue;

                // modificadorPrecio: aplica si el evento es de este cultivo o global (null)
                if (activo.datos.cultivoAfectado == null || activo.datos.cultivoAfectado == cultivo)
                    modificadorEventos += activo.datos.modificadorPrecio;

                // modificadorRendimientoGlobal: siempre aplica (clima, vía, etc.)
                modificadorEventos += activo.datos.modificadorRendimientoGlobal;
            }

            modificadorEventos = Mathf.Clamp(modificadorEventos, 0f, 2f);

            float resultado = cultivo.rendimientoBase
                            * modificadorSuelo
                            * modificadorAgua
                            * modificadorEventos;

            presupuesto           += resultado;
            reporte.gananciaTotal += resultado;
            reporte.cosechasRealizadas.Add(
                $"{cultivo.nombreCultivo} en {parcela.nombreParcela}: ${resultado:N0}");

            parcela.Cosechar();
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // EVENTOS: RESOLUCIÓN STAGED
    // Toggle: marca o desmarca la intención de resolver. El dinero se cobra
    // en AplicarResolucionesPendientes() al avanzar el ciclo.
    // ══════════════════════════════════════════════════════════════════════════

    public void ToggleResolucionEvento(EventoGlobalActivo activo)
    {
        if (!activo.datos.esResolvible) return;
        activo.resolucionPendiente = !activo.resolucionPendiente;
    }

    private void AplicarResolucionesPendientes(ReporteCiclo reporte)
    {
        foreach (EventoGlobalActivo activo in eventosActivosPersistentes)
        {
            if (!activo.resolucionPendiente || activo.resuelto) continue;

            if (presupuesto >= activo.datos.costoResolucion)
            {
                presupuesto -= activo.datos.costoResolucion;
                reporte.gastoTotal += activo.datos.costoResolucion;
                reporte.detalleGastos.Add(new GastoRegistrado
                {
                    descripcion = $"Resolver evento: {activo.datos.nombreEvento}",
                    monto       = activo.datos.costoResolucion,
                    categoria   = CategoriaGasto.ResolucionEvento
                });
                activo.resuelto = true;
            }

            // Si no había presupuesto suficiente, se cancela la intención sin cobrar
            activo.resolucionPendiente = false;
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // UTILIDADES
    // ══════════════════════════════════════════════════════════════════════════

    public bool TodasLasParcelasTienenDecision()
    {
        foreach (Parcela parcela in parcelas)
            if (!parcela.decisionTomada) return false;
        return true;
    }
}
