using System.Collections.Generic;

// Objeto raíz que se serializa a JSON.
// Contiene todo lo necesario para restaurar una partida completa.
[System.Serializable]
public class GameSaveData
{
    public string profileId;
    public string fechaGuardado;       // para mostrar "última vez jugado"

    public int   cicloActual;
    public float presupuesto;
    public float presupuestoAlIniciarCiclo;

    public List<ParcelaSaveData>    parcelas       = new List<ParcelaSaveData>();
    public List<EventoActivoSaveData> eventosActivos = new List<EventoActivoSaveData>();

    // ReporteCiclo ya es [Serializable] y solo tiene primitivos + listas simples
    public List<ReporteCiclo> historialCiclos = new List<ReporteCiclo>();
}
