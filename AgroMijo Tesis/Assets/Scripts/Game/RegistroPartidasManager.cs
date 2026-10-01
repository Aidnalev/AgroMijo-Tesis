using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Guarda los registros de partidas terminadas en:
// persistentDataPath/registros/{profileId}_registros.json
//
// Los registros son permanentes y se conservan localmente.
// Cada partida tiene su propio reportId y un estado de sincronización.
public static class RegistroPartidasManager
{
    public static event Action<GameOverData> RegistroGuardado;

    private static string GetRuta(string profileId)
    {
        string carpeta = Path.Combine(
            Application.persistentDataPath,
            "registros"
        );

        Directory.CreateDirectory(carpeta);

        return Path.Combine(
            carpeta,
            $"{profileId}_registros.json"
        );
    }

    public static void GuardarRegistro(GameOverData data)
    {
        try
        {
            if (string.IsNullOrEmpty(data.reportId))
            {
                data.reportId = "REP-" +
                    Guid.NewGuid()
                        .ToString("N")
                        .ToUpper();
            }

            data.sincronizado = false;

            List<GameOverData> existentes =
                CargarRegistros(data.profileId);

            existentes.Add(data);

            GuardarLista(
                data.profileId,
                existentes
            );

            // Avisamos al sistema de sincronización
            // de que hay un nuevo reporte.
            RegistroGuardado?.Invoke(data);
        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Error al guardar registro: {e.Message}"
            );
        }
    }

    public static List<GameOverData> CargarRegistros(
        string profileId
    )
    {
        string ruta = GetRuta(profileId);

        if (!File.Exists(ruta))
            return new List<GameOverData>();

        try
        {
            string json = File.ReadAllText(ruta);

            PerfilRegistros wrapper =
                JsonUtility.FromJson<PerfilRegistros>(json);

            return wrapper?.registros ??
                   new List<GameOverData>();
        }
        catch
        {
            return new List<GameOverData>();
        }
    }

    public static void MarcarComoSincronizado(
        string profileId,
        string reportId
    )
    {
        List<GameOverData> registros =
            CargarRegistros(profileId);

        GameOverData reporte =
            registros.Find(r => r.reportId == reportId);

        if (reporte == null)
            return;

        reporte.sincronizado = true;

        GuardarLista(
            profileId,
            registros
        );
    }

    private static void GuardarLista(
        string profileId,
        List<GameOverData> registros
    )
    {
        string json = JsonUtility.ToJson(
            new PerfilRegistros
            {
                registros = registros
            },
            true
        );

        File.WriteAllText(
            GetRuta(profileId),
            json
        );
    }
}