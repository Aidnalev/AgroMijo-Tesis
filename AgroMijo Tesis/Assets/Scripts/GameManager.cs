using System.Collections.Generic;
using UnityEngine;

// Coloca este script en un GameObject vacío llamado "GameManager" en tu escena.
// Es el corazón del bucle de juego: la UI llama a sus métodos públicos
// cuando el jugador toma decisiones, y AvanzarCiclo() resuelve el ciclo completo.
public class GameManager : MonoBehaviour
{
    public static GameManager Instancia { get; private set; }

    [Header("Configuración inicial (arrastra tus 4 cultivos y eventos aquí)")]
    public List<CultivoData> cultivosDisponibles;
    public List<EventoGlobalData> eventosPosibles;
    public List<Parcela> parcelas = new List<Parcela>();

    [Header("Estado del juego")]
    public int cicloActual = 0;
    public float presupuesto = 1000000f;

    // Se guarda al inicio de cada ciclo, ANTES de que el jugador gaste en semillas.
    // Es el valor correcto para mostrar en el reporte como "presupuesto inicial".
    [HideInInspector] public float presupuestoAlIniciarCiclo = 1000000f;

    public List<ReporteCiclo> historialCiclos = new List<ReporteCiclo>();
    public List<EventoGlobalActivo> eventosActivosPersistentes = new List<EventoGlobalActivo>();

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
    }

    // Llamado por el botón "Avanzar ciclo" de la UI, una vez el jugador
    // ya decidió qué hacer en cada parcela.
    public ReporteCiclo AvanzarCiclo()
    {
        ReporteCiclo reporte = new ReporteCiclo
        {
            numeroCiclo = cicloActual,
            presupuestoInicial = presupuestoAlIniciarCiclo // lo que había al empezar el ciclo, antes de gastos
        };

        ResolverEventosGlobales(reporte); // una sola vez por ciclo, no por parcela

        foreach (Parcela parcela in parcelas)
        {
            if (parcela.ListaParaCosecha(cicloActual))
            {
                ResolverCosecha(parcela, reporte);
            }
        }

        reporte.presupuestoFinal = presupuesto;
        historialCiclos.Add(reporte);
        cicloActual++;

        presupuestoAlIniciarCiclo = presupuesto; // se guarda DESPUÉS de eventos y cosechas, para el ciclo siguiente

        // Se reinician las decisiones para que el jugador vuelva a elegir en el nuevo ciclo
        foreach (Parcela parcela in parcelas)
        {
            parcela.decisionTomada = false;
        }

        return reporte; // la UI usa esto para mostrar la pantalla de reporte (RF-12, RF-13)
    }

    private void ResolverEventosGlobales(ReporteCiclo reporte)
    {
        // 1. Avanzar los eventos persistentes que ya estaban activos
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

        // 2. Evaluar si ocurre algún evento nuevo este ciclo
        foreach (EventoGlobalData evento in eventosPosibles)
        {
            if (Random.value > evento.probabilidadPorCiclo) continue;

            EventoGlobalActivo nuevo = new EventoGlobalActivo
            {
                datos = evento,
                cicloEnQueOcurrio = cicloActual
            };

            if (evento.esPersistente)
                eventosActivosPersistentes.Add(nuevo);

            reporte.eventosOcurridos.Add($"[Nuevo] {evento.nombreEvento}: {evento.descripcionAlOcurrir}");
        }
    }

    private void ResolverCosecha(Parcela parcela, ReporteCiclo reporte)
    {
        CultivoData cultivo = parcela.cultivoActual;
        float modificadorSuelo = cultivo.ObtenerModificadorPorSuelo(parcela.tipoSuelo);
        float modificadorAgua = Mathf.Lerp(0.5f, 1f, parcela.disponibilidadAgua);

        // Acumular modificadores de todos los eventos activos que apliquen
        float modificadorEventos = 1f;
        foreach (EventoGlobalActivo activo in eventosActivosPersistentes)
        {
            if (activo.resuelto) continue;

            // modificadorPrecio aplica si el evento es del cultivo cosechado o es global (null)
            if (activo.datos.cultivoAfectado == null || activo.datos.cultivoAfectado == cultivo)
                modificadorEventos += activo.datos.modificadorPrecio;

            // modificadorRendimientoGlobal siempre aplica (clima, etc.)
            modificadorEventos += activo.datos.modificadorRendimientoGlobal;
        }
        modificadorEventos = Mathf.Clamp(modificadorEventos, 0f, 2f);

        float resultado = cultivo.rendimientoBase * modificadorSuelo * modificadorAgua * modificadorEventos;

        presupuesto += resultado;
        reporte.gananciaTotal += resultado;
        reporte.cosechasRealizadas.Add($"{cultivo.nombreCultivo} en {parcela.nombreParcela}: ${resultado:N0}");

        parcela.Cosechar();
    }

    // Llamado desde la UI cuando el jugador elige "Plantar" en una parcela (RF-02, RF-03)
    public bool IntentarPlantar(Parcela parcela, CultivoData cultivo)
    {
        if (parcela.estado == EstadoParcela.Plantada) return false; // ya hay un cultivo creciendo ahí
        if (presupuesto < cultivo.costoSemilla) return false;

        presupuesto -= cultivo.costoSemilla;
        parcela.Plantar(cultivo, cicloActual);
        parcela.decisionTomada = true;
        return true;
    }

    // Llamado desde la UI cuando el jugador elige "Estudiar terreno" (RF-04)
    public bool IntentarEstudiarTerreno(Parcela parcela, float costoEstudio)
    {
        if (presupuesto < costoEstudio) return false;

        presupuesto -= costoEstudio;
        parcela.estudiada = true;
        parcela.decisionTomada = true;
        return true;
    }

    // Llamado desde la UI cuando el jugador elige "Mejorar drenaje" en una parcela (RF-08)
    public bool IntentarMejorarDrenaje(Parcela parcela, float costoMejora)
    {
        if (presupuesto < costoMejora) return false;

        presupuesto -= costoMejora;
        parcela.decisionTomada = true;
        return true;
    }

    // Llamado desde la UI cuando el jugador elige "Esperar" (no hacer nada esta parcela este ciclo)
    public void IntentarEsperar(Parcela parcela)
    {
        parcela.decisionTomada = true;
    }

    // Usado por el HUD para saber si debe advertir antes de avanzar de ciclo
    public bool TodasLasParcelasTienenDecision()
    {
        foreach (Parcela parcela in parcelas)
        {
            if (!parcela.decisionTomada) return false;
        }
        return true;
    }
}