using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ReportesSyncManager : MonoBehaviour
{
    public static ReportesSyncManager Instance { get; private set; }

    private const string API_URL =
        "https://localhost:7240";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        RegistroPartidasManager.RegistroGuardado +=
            OnRegistroGuardado;
    }

    private void OnDisable()
    {
        RegistroPartidasManager.RegistroGuardado -=
            OnRegistroGuardado;
    }

    private void Start()
    {
        StartCoroutine(SincronizarPendientes());
    }

    private void OnRegistroGuardado(GameOverData data)
    {
        StartCoroutine(
            SincronizarReporte(data)
        );
    }

    private IEnumerator SincronizarPendientes()
    {
        // Damos un pequeño margen para que el juego
        // termine de inicializarse.
        yield return new WaitForSeconds(1f);

        if (ProfileManager.Instance == null)
            yield break;

        if (ProfileManager.Instance.profileData == null)
            yield break;

        foreach (PlayerProfile profile
                 in ProfileManager.Instance.profileData.profiles)
        {
            List<GameOverData> registros =
                RegistroPartidasManager.CargarRegistros(
                    profile.id
                );

            foreach (GameOverData reporte in registros)
            {
                if (!reporte.sincronizado)
                {
                    yield return StartCoroutine(
                        SincronizarReporte(reporte)
                    );
                }
            }
        }
    }

    private IEnumerator SincronizarReporte(
        GameOverData data
    )
    {
        string url =
            API_URL + "/api/reports";

        GameReportRequest requestData =
            new GameReportRequest(data);

        string json =
            JsonUtility.ToJson(requestData);

        using (UnityWebRequest request =
               new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw =
                System.Text.Encoding.UTF8.GetBytes(json);

            request.uploadHandler =
                new UploadHandlerRaw(bodyRaw);

            request.downloadHandler =
                new DownloadHandlerBuffer();

            request.SetRequestHeader(
                "Content-Type",
                "application/json"
            );

            yield return request.SendWebRequest();

            if (request.result ==
                UnityWebRequest.Result.Success)
            {
                RegistroPartidasManager
                    .MarcarComoSincronizado(
                        data.profileId,
                        data.reportId
                    );

                Debug.Log(
                    $"Reporte {data.reportId} " +
                    "sincronizado correctamente."
                );
            }
            else if (request.responseCode == 409)
            {
                // El reporte ya existe en MongoDB.
                // Por tanto, podemos considerarlo sincronizado.
                RegistroPartidasManager
                    .MarcarComoSincronizado(
                        data.profileId,
                        data.reportId
                    );

                Debug.Log(
                    $"El reporte {data.reportId} " +
                    "ya existía en MongoDB."
                );
            }
            else
            {
                Debug.LogWarning(
                    $"No se pudo sincronizar el reporte " +
                    $"{data.reportId}. " +
                    $"Quedará pendiente."
                );
            }
        }
    }
}
[System.Serializable]
public class GameReportRequest
{
    public string reportId;

    public string profileId;
    public string profileAlias;

    public string fechaPartida;
    public int ciclosJugados;
    public string razonFin;

    public float presupuestoInicial;
    public float presupuestoFinal;
    public float gananciaAcumulada;
    public float gastoAcumulado;

    public List<ReporteCicloRequest> historialCiclos;

    public GameReportRequest(GameOverData data)
    {
        reportId = data.reportId;

        profileId = data.profileId;
        profileAlias = data.profileAlias;

        fechaPartida = data.fechaPartida;
        ciclosJugados = data.ciclosJugados;
        razonFin = data.razonFin;

        presupuestoInicial = data.presupuestoInicial;
        presupuestoFinal = data.presupuestoFinal;
        gananciaAcumulada = data.gananciaAcumulada;
        gastoAcumulado = data.gastoAcumulado;

        historialCiclos =
            new List<ReporteCicloRequest>();

        foreach (ReporteCiclo ciclo
                 in data.historialCiclos)
        {
            historialCiclos.Add(
                new ReporteCicloRequest(ciclo)
            );
        }
    }
}

[System.Serializable]
public class ReporteCicloRequest
{
    public int numeroCiclo;

    public float presupuestoInicial;
    public float presupuestoFinal;
    public float gananciaTotal;
    public float gastoTotal;

    public int jornalesNecesarios;
    public int jornalesFamiliaresUsados;
    public int jornalesContratados;
    public float modificadorJornales;

    public bool esUltimoCiclo;
    public string razonFin;

    public List<GastoRegistradoRequest> detalleGastos;
    public List<string> eventosOcurridos;
    public List<string> cosechasRealizadas;

    public ReporteCicloRequest(ReporteCiclo ciclo)
    {
        numeroCiclo = ciclo.numeroCiclo;

        presupuestoInicial = ciclo.presupuestoInicial;
        presupuestoFinal = ciclo.presupuestoFinal;
        gananciaTotal = ciclo.gananciaTotal;
        gastoTotal = ciclo.gastoTotal;

        jornalesNecesarios = ciclo.jornalesNecesarios;
        jornalesFamiliaresUsados =
            ciclo.jornalesFamiliaresUsados;
        jornalesContratados =
            ciclo.jornalesContratados;
        modificadorJornales =
            ciclo.modificadorJornales;

        esUltimoCiclo = ciclo.esUltimoCiclo;
        razonFin = ciclo.razonFin;

        detalleGastos =
            new List<GastoRegistradoRequest>();

        foreach (GastoRegistrado gasto
                 in ciclo.detalleGastos)
        {
            detalleGastos.Add(
                new GastoRegistradoRequest(gasto)
            );
        }

        eventosOcurridos =
            new List<string>(
                ciclo.eventosOcurridos
            );

        cosechasRealizadas =
            new List<string>(
                ciclo.cosechasRealizadas
            );
    }
}

[System.Serializable]
public class GastoRegistradoRequest
{
    public string descripcion;
    public float monto;
    public CategoriaGasto categoria;

    public GastoRegistradoRequest(
        GastoRegistrado gasto
    )
    {
        descripcion = gasto.descripcion;
        monto = gasto.monto;
        categoria = gasto.categoria;
    }
}