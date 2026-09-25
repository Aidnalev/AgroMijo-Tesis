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

    [Header("Pool de parcelas")]
    [Tooltip("Todos los ScriptableObjects de parcela posibles (apunta a 12 assets).")]
    public List<ParcelaTemplateSO> parcelasPool = new List<ParcelaTemplateSO>();
    [Tooltip("Cuántas parcelas se seleccionan del pool al iniciar cada partida")]
    public int cantidadParcelasASeleccionar = 6;

    [Header("Condiciones de fin de partida")]
    [Tooltip("La partida termina al completar este número de ciclos")]
    public int ciclosMaximos = 12;

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

        // Sincronizar presupuestoAlIniciarCiclo con el valor configurado en Inspector
        presupuestoAlIniciarCiclo = presupuesto;

        // En Awake para que esté listo antes de que cualquier Start() lo lea
        foreach (Parcela parcela in parcelas)
            parcela.InicializarAgua();

        CargarSiExiste();
    }

    private void CargarSiExiste()
    {
        string profileId = ProfileManager.Instance?.CurrentProfile?.id;

        if (!string.IsNullOrEmpty(profileId))
        {
            GameSaveData data = SaveManager.Cargar(profileId);
            if (data != null)
            {
                AplicarEstadoGuardado(data);
                return;
            }
        }

        // Sin save: seleccionar parcelas del pool si está configurado
        if (parcelasPool.Count > 0)
            InicializarParcelasDesdePool();
    }

    private void InicializarParcelasDesdePool()
    {
        // Mezclar el pool (Fisher-Yates)
        List<ParcelaTemplateSO> mezcladas = new List<ParcelaTemplateSO>(parcelasPool);
        for (int i = mezcladas.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            ParcelaTemplateSO tmp = mezcladas[i];
            mezcladas[i]         = mezcladas[j];
            mezcladas[j]         = tmp;
        }

        int cantidad = Mathf.Min(cantidadParcelasASeleccionar, mezcladas.Count);
        parcelas.Clear();

        for (int i = 0; i < cantidad; i++)
        {
            ParcelaTemplateSO template = mezcladas[i];

            // Crear instancia de runtime a partir de la plantilla
            Parcela nueva = new Parcela
            {
                nombreParcela     = template.nombreParcela,
                tipoSuelo         = template.tipoSuelo,
                aguaBase          = template.aguaBase,
                accesoVial        = template.accesoVial,
                estudiada         = false,
                nivelMejora       = 0,
                estado            = EstadoParcela.Vacia,
                decisionTomada    = false,
                decisionPendiente = new DecisionPendiente()
            };
            nueva.InicializarAgua();
            parcelas.Add(nueva);
        }
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

        VerificarFinDeCiclo(reporte);

        // Autosave: guarda después de cada ciclo vinculado al perfil activo
        string profileId = ProfileManager.Instance?.CurrentProfile?.id;
        if (!string.IsNullOrEmpty(profileId))
            SaveManager.Guardar(CrearEstadoGuardado());

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
    // FIN DE PARTIDA
    // ══════════════════════════════════════════════════════════════════════════

    private void VerificarFinDeCiclo(ReporteCiclo reporte)
    {
        if (cicloActual >= ciclosMaximos)
        {
            reporte.esUltimoCiclo = true;
            reporte.razonFin      = "Completaste todos los ciclos de la partida.";
        }
        else if (presupuesto <= 0f && !HayCultivosCreciendo())
        {
            reporte.esUltimoCiclo = true;
            reporte.razonFin      = "Sin presupuesto y sin cosechas pendientes. La UAF no puede continuar.";
        }
    }

    private bool HayCultivosCreciendo()
    {
        foreach (Parcela p in parcelas)
            if (p.estado == EstadoParcela.Plantada) return true;
        return false;
    }

    public GameOverData CrearRegistroFinal(string razon)
    {
        float gananciaTotal = 0f;
        float gastoTotal    = 0f;
        foreach (ReporteCiclo r in historialCiclos)
        {
            gananciaTotal += r.gananciaTotal;
            gastoTotal    += r.gastoTotal;
        }

        float presupuestoInicio = historialCiclos.Count > 0
            ? historialCiclos[0].presupuestoInicial
            : presupuesto;

        return new GameOverData
        {
            profileId          = ProfileManager.Instance?.CurrentProfile?.id ?? "",
            profileAlias       = ProfileManager.Instance?.CurrentProfile?.alias ?? "",
            fechaPartida       = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
            ciclosJugados      = cicloActual,
            razonFin           = razon,
            presupuestoInicial = presupuestoInicio,
            presupuestoFinal   = presupuesto,
            gananciaAcumulada  = gananciaTotal,
            gastoAcumulado     = gastoTotal,
            historialCiclos    = new System.Collections.Generic.List<ReporteCiclo>(historialCiclos)
        };
    }

    // ══════════════════════════════════════════════════════════════════════════
    // GUARDADO Y CARGADO
    // ══════════════════════════════════════════════════════════════════════════

    public GameSaveData CrearEstadoGuardado()
    {
        string profileId = ProfileManager.Instance?.CurrentProfile?.id ?? "sin_perfil";

        List<ParcelaSaveData> parcelasSave = new List<ParcelaSaveData>();
        foreach (Parcela p in parcelas)
        {
            parcelasSave.Add(new ParcelaSaveData
            {
                nombreParcela       = p.nombreParcela,
                tipoSuelo           = (int)p.tipoSuelo,
                aguaBase            = p.aguaBase,
                disponibilidadAgua  = p.disponibilidadAgua,
                accesoVial          = p.accesoVial,
                estudiada           = p.estudiada,
                nivelMejora         = p.nivelMejora,
                estado              = (int)p.estado,
                cultivoActualNombre = p.cultivoActual?.nombreCultivo,
                cicloEnQueSePlanto  = p.cicloEnQueSePlanto
            });
        }

        List<EventoActivoSaveData> eventosSave = new List<EventoActivoSaveData>();
        foreach (EventoGlobalActivo e in eventosActivosPersistentes)
        {
            eventosSave.Add(new EventoActivoSaveData
            {
                eventoNombre        = e.datos.nombreEvento,
                cicloEnQueOcurrio   = e.cicloEnQueOcurrio,
                resuelto            = e.resuelto,
                ciclosActivo        = e.ciclosActivo,
                resolucionPendiente = e.resolucionPendiente
            });
        }

        return new GameSaveData
        {
            profileId                = profileId,
            fechaGuardado            = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            cicloActual              = cicloActual,
            presupuesto              = presupuesto,
            presupuestoAlIniciarCiclo = presupuestoAlIniciarCiclo,
            parcelas                 = parcelasSave,
            eventosActivos           = eventosSave,
            historialCiclos          = new List<ReporteCiclo>(historialCiclos)
        };
    }

    public void AplicarEstadoGuardado(GameSaveData data)
    {
        cicloActual               = data.cicloActual;
        presupuesto               = data.presupuesto;
        presupuestoAlIniciarCiclo = data.presupuestoAlIniciarCiclo;
        historialCiclos           = data.historialCiclos ?? new List<ReporteCiclo>();

        // Restaurar parcelas — si la lista está vacía (sistema de pool), se crean desde el save
        if (parcelas.Count == 0)
        {
            foreach (ParcelaSaveData ps in data.parcelas)
            {
                Parcela nueva = new Parcela
                {
                    nombreParcela     = ps.nombreParcela,
                    tipoSuelo         = (TipoSuelo)ps.tipoSuelo,
                    aguaBase          = ps.aguaBase,
                    disponibilidadAgua = ps.disponibilidadAgua,
                    accesoVial        = ps.accesoVial,
                    estudiada         = ps.estudiada,
                    nivelMejora       = ps.nivelMejora,
                    estado            = (EstadoParcela)ps.estado,
                    cicloEnQueSePlanto = ps.cicloEnQueSePlanto,
                    decisionPendiente = new DecisionPendiente()
                };
                nueva.cultivoActual = string.IsNullOrEmpty(ps.cultivoActualNombre)
                    ? null
                    : cultivosDisponibles.Find(c => c.nombreCultivo == ps.cultivoActualNombre);
                parcelas.Add(nueva);
            }
        }
        else
        {
            // Lista ya poblada (sistema legado): modificar en sitio por índice
            for (int i = 0; i < parcelas.Count && i < data.parcelas.Count; i++)
            {
                ParcelaSaveData ps = data.parcelas[i];
                Parcela p          = parcelas[i];

                p.tipoSuelo          = (TipoSuelo)ps.tipoSuelo;
                p.aguaBase           = ps.aguaBase;
                p.disponibilidadAgua = ps.disponibilidadAgua;
                p.accesoVial         = ps.accesoVial;
                p.estudiada          = ps.estudiada;
                p.nivelMejora        = ps.nivelMejora;
                p.estado             = (EstadoParcela)ps.estado;
                p.cicloEnQueSePlanto = ps.cicloEnQueSePlanto;
                p.cultivoActual      = string.IsNullOrEmpty(ps.cultivoActualNombre)
                    ? null
                    : cultivosDisponibles.Find(c => c.nombreCultivo == ps.cultivoActualNombre);
            }
        }

        // Restaurar eventos activos por nombre
        eventosActivosPersistentes.Clear();
        foreach (EventoActivoSaveData es in data.eventosActivos)
        {
            EventoGlobalData eventoData = eventosPosibles
                .Find(e => e.nombreEvento == es.eventoNombre);
            if (eventoData == null) continue;

            eventosActivosPersistentes.Add(new EventoGlobalActivo
            {
                datos               = eventoData,
                cicloEnQueOcurrio   = es.cicloEnQueOcurrio,
                resuelto            = es.resuelto,
                ciclosActivo        = es.ciclosActivo,
                resolucionPendiente = es.resolucionPendiente
            });
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
