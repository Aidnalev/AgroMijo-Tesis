using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia { get; private set; }

    [Header("Configuración inicial")]
    public List<CultivoData> cultivosDisponibles;
    public List<EventoGlobalData> eventosPosibles;
    public List<Parcela> parcelas = new List<Parcela>();

    [Header("Costos de acciones")]
    public float costoEstudio           = 30000f;
    public float costoMejora            = 80000f;
    public float costoJornalContratado  = 45000f;

    [Header("Jornales familiares por ciclo")]
    [Tooltip("Jornales gratuitos que aporta la familia cada ciclo")]
    public int jornalesFamiliares = 20;

    [Header("Estado del juego")]
    public int   cicloActual = 0;
    public float presupuesto = 1000000f;
    [HideInInspector] public float presupuestoAlIniciarCiclo = 1000000f;

    public List<ReporteCiclo>       historialCiclos            = new List<ReporteCiclo>();
    public List<EventoGlobalActivo> eventosActivosPersistentes = new List<EventoGlobalActivo>();

    private void Awake()
    {
        if (Instancia != null && Instancia != this) { Destroy(gameObject); return; }
        Instancia = this;

        // En Awake para que esté listo antes de que cualquier Start() lo lea
        foreach (Parcela parcela in parcelas)
            parcela.InicializarAgua();
    }

    // ══════════════════════════════════════════════════════════════════════════
    // GUARDAR DECISIONES (staged: no aplican hasta AvanzarCiclo)
    // ══════════════════════════════════════════════════════════════════════════

    public void GuardarDecisionPlantar(Parcela parcela, CultivoData cultivo)
    {
        if (parcela.estado == EstadoParcela.Plantada) return;
        parcela.decisionPendiente.tipo               = TipoDecision.Plantar;
        parcela.decisionPendiente.cultivoSeleccionado = cultivo;
        parcela.decisionTomada                       = true;
    }

    public void GuardarDecisionEstudiar(Parcela parcela)
    {
        parcela.decisionPendiente.tipo               = TipoDecision.Estudiar;
        parcela.decisionPendiente.cultivoSeleccionado = null;
        parcela.decisionTomada                       = true;
    }

    public void GuardarDecisionMejorar(Parcela parcela)
    {
        if (parcela.nivelMejora >= 3) return;
        parcela.decisionPendiente.tipo               = TipoDecision.Mejorar;
        parcela.decisionPendiente.cultivoSeleccionado = null;
        parcela.decisionTomada                       = true;
    }

    public void GuardarDecisionEsperar(Parcela parcela)
    {
        parcela.decisionPendiente.tipo               = TipoDecision.Esperar;
        parcela.decisionPendiente.cultivoSeleccionado = null;
        parcela.decisionTomada                       = true;
    }

    // ══════════════════════════════════════════════════════════════════════════
    // JORNALES
    // ══════════════════════════════════════════════════════════════════════════

    // Calcula cuántos jornales se necesitan este ciclo según las decisiones
    // pendientes y el estado actual de los cultivos.
    public int CalcularJornalesNecesarios()
    {
        int total = 0;
        foreach (Parcela parcela in parcelas)
        {
            // Plantar
            if (parcela.decisionPendiente.tipo == TipoDecision.Plantar
                && parcela.decisionPendiente.cultivoSeleccionado != null)
            {
                total += parcela.decisionPendiente.cultivoSeleccionado.jornalesParaPlantar;
            }

            if (parcela.estado == EstadoParcela.Plantada && parcela.cultivoActual != null)
            {
                // Cosechar (si ya está listo) o mantener (si aún está creciendo)
                if (parcela.ListaParaCosecha(cicloActual))
                    total += parcela.cultivoActual.jornalesParaCosechar;
                else
                    total += parcela.cultivoActual.jornalesMantenimientoPorCiclo;
            }
        }
        return total;
    }

    // ══════════════════════════════════════════════════════════════════════════
    // AVANZAR CICLO
    // Orden: agua → decisiones → resoluciones → eventos → cosechas → cierre
    // ══════════════════════════════════════════════════════════════════════════

    public ReporteCiclo AvanzarCiclo(int jornalesContratados = 0)
    {
        int   jornalesNecesarios  = CalcularJornalesNecesarios();
        int   jornalesDisponibles = jornalesFamiliares + jornalesContratados;
        float modJornales         = jornalesNecesarios > 0
            ? Mathf.Clamp01((float)jornalesDisponibles / jornalesNecesarios)
            : 1f;

        ReporteCiclo reporte = new ReporteCiclo
        {
            numeroCiclo             = cicloActual,
            presupuestoInicial      = presupuestoAlIniciarCiclo,
            jornalesNecesarios      = jornalesNecesarios,
            jornalesFamiliaresUsados = Mathf.Min(jornalesFamiliares, jornalesNecesarios),
            jornalesContratados     = jornalesContratados,
            modificadorJornales     = modJornales
        };

        // Cobrar jornales contratados
        if (jornalesContratados > 0)
        {
            float costoJornales = jornalesContratados * costoJornalContratado;
            presupuesto          -= costoJornales;
            reporte.gastoTotal   += costoJornales;
            reporte.detalleGastos.Add(new GastoRegistrado
            {
                descripcion = $"Jornales contratados ({jornalesContratados})",
                monto       = costoJornales,
                categoria   = CategoriaGasto.Jornal
            });
        }

        AplicarVariacionAgua();
        AplicarDecisionesPendientes(reporte);
        AplicarResolucionesPendientes(reporte);
        ResolverEventosGlobales(reporte);
        ResolverCosechas(reporte, modJornales);

        reporte.presupuestoFinal   = presupuesto;
        historialCiclos.Add(reporte);
        cicloActual++;
        presupuestoAlIniciarCiclo = presupuesto;

        return reporte;
    }

    private void AplicarVariacionAgua()
    {
        foreach (Parcela parcela in parcelas)
            parcela.AplicarVariacionAgua(Random.Range(-0.05f, 0.05f));
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
                        presupuesto        -= cultivo.costoSemilla;
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
                        presupuesto        -= costoEstudio;
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
                    if (parcela.nivelMejora < 3 && presupuesto >= costoMejora)
                    {
                        presupuesto        -= costoMejora;
                        reporte.gastoTotal += costoMejora;
                        parcela.nivelMejora++;

                        string nombreMejora = parcela.nivelMejora switch
                        {
                            1 => "Acceso vial",
                            2 => "Sistema de riego",
                            3 => "Fertilizacion",
                            _ => "Mejora"
                        };

                        if (parcela.nivelMejora == 2)
                            parcela.aguaBase = Mathf.Clamp(parcela.aguaBase + 0.25f, 0f, 1f);

                        reporte.detalleGastos.Add(new GastoRegistrado
                        {
                            descripcion = $"{nombreMejora} (Niv.{parcela.nivelMejora}): {parcela.nombreParcela}",
                            monto       = costoMejora,
                            categoria   = CategoriaGasto.Mejora
                        });
                    }
                    break;
            }

            parcela.decisionPendiente.tipo               = TipoDecision.Ninguna;
            parcela.decisionPendiente.cultivoSeleccionado = null;
            parcela.decisionTomada                       = false;
        }
    }

    private void AplicarResolucionesPendientes(ReporteCiclo reporte)
    {
        foreach (EventoGlobalActivo activo in eventosActivosPersistentes)
        {
            if (!activo.resolucionPendiente || activo.resuelto) continue;

            if (presupuesto >= activo.datos.costoResolucion)
            {
                presupuesto        -= activo.datos.costoResolucion;
                reporte.gastoTotal += activo.datos.costoResolucion;
                reporte.detalleGastos.Add(new GastoRegistrado
                {
                    descripcion = $"Resolver evento: {activo.datos.nombreEvento}",
                    monto       = activo.datos.costoResolucion,
                    categoria   = CategoriaGasto.ResolucionEvento
                });
                activo.resuelto = true;
            }

            activo.resolucionPendiente = false;
        }
    }

    private void ResolverEventosGlobales(ReporteCiclo reporte)
    {
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
                reporte.eventosOcurridos.Add($"[Continua] {activo.datos.nombreEvento}");
            }
        }

        foreach (EventoGlobalData evento in eventosPosibles)
        {
            if (Random.value > evento.probabilidadPorCiclo) continue;
            EventoGlobalActivo nuevo = new EventoGlobalActivo
            {
                datos             = evento,
                cicloEnQueOcurrio = cicloActual
            };
            if (evento.esPersistente) eventosActivosPersistentes.Add(nuevo);
            reporte.eventosOcurridos.Add($"[Nuevo] {evento.nombreEvento}: {evento.descripcionAlOcurrir}");
        }
    }

    private void ResolverCosechas(ReporteCiclo reporte, float modificadorJornales)
    {
        foreach (Parcela parcela in parcelas)
        {
            if (!parcela.ListaParaCosecha(cicloActual)) continue;

            CultivoData cultivo      = parcela.cultivoActual;
            float modSuelo           = cultivo.ObtenerModificadorPorSuelo(parcela.tipoSuelo);
            float modAgua            = Mathf.Lerp(0.5f, 1f, parcela.disponibilidadAgua);
            float modFertilizacion   = parcela.TieneFertilizacion ? 1.2f : 1f;
            float modEventos         = 1f;

            foreach (EventoGlobalActivo activo in eventosActivosPersistentes)
            {
                if (activo.resuelto) continue;
                if (activo.datos.cultivoAfectado == null || activo.datos.cultivoAfectado == cultivo)
                    modEventos += activo.datos.modificadorPrecio;

                float impactoVial = activo.datos.modificadorRendimientoGlobal;
                if (activo.datos.categoria == CategoriaEvento.Infraestructura && parcela.TieneAccesoVial)
                    impactoVial *= 0.5f;

                modEventos += impactoVial;
            }
            modEventos = Mathf.Clamp(modEventos, 0f, 2f);

            float resultado = cultivo.rendimientoBase
                            * modSuelo
                            * modAgua
                            * modEventos
                            * modFertilizacion
                            * modificadorJornales;

            presupuesto           += resultado;
            reporte.gananciaTotal += resultado;
            reporte.cosechasRealizadas.Add(
                $"{cultivo.nombreCultivo} en {parcela.nombreParcela}: ${resultado:N0}");

            parcela.Cosechar();
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // EVENTOS: RESOLUCIÓN STAGED
    // ══════════════════════════════════════════════════════════════════════════

    public void ToggleResolucionEvento(EventoGlobalActivo activo)
    {
        if (!activo.datos.esResolvible) return;
        activo.resolucionPendiente = !activo.resolucionPendiente;
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
