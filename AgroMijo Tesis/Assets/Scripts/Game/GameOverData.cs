using System;
using System.Collections.Generic;

// Registro permanente de una partida completada.
// Se guarda en disco vinculado al perfil y nunca se borra
// (a diferencia del save activo que se borra al terminar).
[Serializable]
public class GameOverData
{
    // Identificador único de este reporte/partida
    public string reportId;

    // Identificación del perfil
    public string profileId;
    public string profileAlias;

    // Información de la partida
    public string fechaPartida;
    public int ciclosJugados;
    public string razonFin;

    // Estado de sincronización con el servidor
    public bool sincronizado = false;

    // Resumen económico
    public float presupuestoInicial;
    public float presupuestoFinal;
    public float gananciaAcumulada;
    public float gastoAcumulado;

    // Detalle de cada ciclo
    public List<ReporteCiclo> historialCiclos =
        new List<ReporteCiclo>();
}

// Contenedor de todos los registros de un perfil (para serializar con JsonUtility)
[Serializable]
public class PerfilRegistros
{
    public List<GameOverData> registros = new List<GameOverData>();
}
