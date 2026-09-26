using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

// Exporta los registros de partidas terminadas de un perfil a un archivo CSV.
// Ruta: Application.persistentDataPath/exports/{profileId}_{fecha}.csv
// Una fila por ciclo por partida — apto para análisis en Excel.
public static class ExportManager
{
    public static string ExportarCSV(string profileId)
    {
        List<GameOverData> registros = RegistroPartidasManager.CargarRegistros(profileId);

        if (registros.Count == 0) return null;

        StringBuilder sb = new StringBuilder();

        // Encabezado
        sb.AppendLine(
            "ProfileId,ProfileAlias,FechaPartida,CiclosJugados,RazonFin," +
            "Ciclo,PresupuestoInicial,PresupuestoFinal,Ganancias,Gastos," +
            "JornalesNecesarios,JornalesFamiliares,JornalesContratados,RendimientoJornales," +
            "Eventos,Cosechas,DetalleGastos");

        foreach (GameOverData registro in registros)
        {
            foreach (ReporteCiclo ciclo in registro.historialCiclos)
            {
                string eventos  = string.Join("; ", ciclo.eventosOcurridos);
                string cosechas = string.Join("; ", ciclo.cosechasRealizadas);

                List<string> gastosStr = new List<string>();
                foreach (GastoRegistrado g in ciclo.detalleGastos)
                    gastosStr.Add($"{g.descripcion}: ${g.monto:F0}");
                string gastos = string.Join("; ", gastosStr);

                sb.AppendLine(
                    $"{E(registro.profileId)},"   +
                    $"{E(registro.profileAlias)}," +
                    $"{E(registro.fechaPartida)}," +
                    $"{registro.ciclosJugados},"   +
                    $"{E(registro.razonFin)},"     +
                    $"{ciclo.numeroCiclo + 1},"    +
                    $"{ciclo.presupuestoInicial:F0},"  +
                    $"{ciclo.presupuestoFinal:F0},"    +
                    $"{ciclo.gananciaTotal:F0},"        +
                    $"{ciclo.gastoTotal:F0},"            +
                    $"{ciclo.jornalesNecesarios},"       +
                    $"{ciclo.jornalesFamiliaresUsados}," +
                    $"{ciclo.jornalesContratados},"      +
                    $"{ciclo.modificadorJornales:F2},"   +
                    $"{E(eventos)},"  +
                    $"{E(cosechas)}," +
                    $"{E(gastos)}");
            }
        }

        string carpeta = Path.Combine(Application.persistentDataPath, "exports");
        Directory.CreateDirectory(carpeta);

        string nombre = $"{profileId}_{System.DateTime.Now:yyyyMMdd_HHmm}.csv";
        string ruta   = Path.Combine(carpeta, nombre);

        File.WriteAllText(ruta, sb.ToString(), Encoding.UTF8);
        return ruta;
    }

    // Escapa campos que puedan contener comas, comillas o saltos de línea
    private static string E(string valor)
    {
        if (string.IsNullOrEmpty(valor)) return "";
        if (valor.Contains(",") || valor.Contains("\"") || valor.Contains("\n"))
            return $"\"{valor.Replace("\"", "\"\"")}\"";
        return valor;
    }
}
