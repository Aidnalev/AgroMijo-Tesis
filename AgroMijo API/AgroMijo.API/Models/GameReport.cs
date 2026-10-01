using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace AgroMijo.API.Models;

public class GameReport
{
    [BsonId]
    public string ReportId { get; set; } = string.Empty;

    public string ProfileId { get; set; } = string.Empty;
    public string ProfileAlias { get; set; } = string.Empty;

    public string FechaPartida { get; set; } = string.Empty;
    public int CiclosJugados { get; set; }
    public string RazonFin { get; set; } = string.Empty;

    public float PresupuestoInicial { get; set; }
    public float PresupuestoFinal { get; set; }
    public float GananciaAcumulada { get; set; }
    public float GastoAcumulado { get; set; }

    public List<ReporteCiclo> HistorialCiclos { get; set; } = new();
}

public class ReporteCiclo
{
    public int NumeroCiclo { get; set; }

    public float PresupuestoInicial { get; set; }
    public float PresupuestoFinal { get; set; }
    public float GananciaTotal { get; set; }
    public float GastoTotal { get; set; }

    public int JornalesNecesarios { get; set; }
    public int JornalesFamiliaresUsados { get; set; }
    public int JornalesContratados { get; set; }
    public float ModificadorJornales { get; set; }

    public bool EsUltimoCiclo { get; set; }
    public string RazonFin { get; set; } = string.Empty;

    public List<GastoRegistrado> DetalleGastos { get; set; } = new();
    public List<string> EventosOcurridos { get; set; } = new();
    public List<string> CosechasRealizadas { get; set; } = new();
}

public class GastoRegistrado
{
    public string Descripcion { get; set; } = string.Empty;
    public float Monto { get; set; }
    public CategoriaGasto Categoria { get; set; }
}

public enum CategoriaGasto
{
    Semilla,
    Estudio,
    Mejora,
    ResolucionEvento,
    Jornal
}