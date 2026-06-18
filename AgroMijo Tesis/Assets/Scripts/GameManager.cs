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
    public List<EventoAleatorioData> eventosPosibles;
    public List<Parcela> parcelas = new List<Parcela>();

    [Header("Estado del juego")]
    public int cicloActual = 0;
    public float presupuesto = 1000000f;

    public List<ReporteCiclo> historialCiclos = new List<ReporteCiclo>();

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
            presupuestoInicial = presupuesto
        };

        foreach (Parcela parcela in parcelas)
        {
            ResolverEventoAleatorio(parcela, reporte);

            if (parcela.ListaParaCosecha(cicloActual))
            {
                ResolverCosecha(parcela, reporte);
            }
        }

        reporte.presupuestoFinal = presupuesto;
        historialCiclos.Add(reporte);
        cicloActual++;

        // Se reinician las decisiones para que el jugador vuelva a elegir en el nuevo ciclo
        foreach (Parcela parcela in parcelas)
        {
            parcela.decisionTomada = false;
        }

        return reporte; // la UI usa esto para mostrar la pantalla de reporte (RF-12, RF-13)
    }

    private void ResolverEventoAleatorio(Parcela parcela, ReporteCiclo reporte)
    {
        foreach (EventoAleatorioData evento in eventosPosibles)
        {
            if (Random.value > evento.probabilidadPorCiclo) continue;

            presupuesto += evento.impactoEconomico;

            if (evento.impactoEconomico < 0)
                reporte.gastoTotal += Mathf.Abs(evento.impactoEconomico);
            else
                reporte.gananciaTotal += evento.impactoEconomico;

            reporte.eventosOcurridos.Add($"{evento.nombreEvento} en {parcela.nombreParcela}");
        }
    }

    private void ResolverCosecha(Parcela parcela, ReporteCiclo reporte)
    {
        CultivoData cultivo = parcela.cultivoActual;
        float modificadorSuelo = cultivo.ObtenerModificadorPorSuelo(parcela.tipoSuelo);
        float modificadorAgua = Mathf.Lerp(0.5f, 1f, parcela.disponibilidadAgua);

        float resultado = cultivo.rendimientoBase * modificadorSuelo * modificadorAgua;

        presupuesto += resultado;
        reporte.gananciaTotal += resultado;
        reporte.cosechasRealizadas.Add($"{cultivo.nombreCultivo} en {parcela.nombreParcela}: ${resultado:N0}");

        parcela.Cosechar();
    }

    // Llamado desde la UI cuando el jugador elige "Plantar" en una parcela (RF-02, RF-03)
    public bool IntentarPlantar(Parcela parcela, CultivoData cultivo)
    {
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
}
