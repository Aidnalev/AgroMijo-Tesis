using UnityEngine;

public enum EstadoParcela
{
    Vacia,
    Plantada
}

// Una parcela es cada terreno de la UAF que el jugador gestiona.
// No es un MonoBehaviour: es un dato puro que el GameManager administra,
// y que tu vista aérea simplemente representa visualmente.
[System.Serializable]
public class Parcela
{
    public string nombreParcela = "Parcela 1";
    public TipoSuelo tipoSuelo;

    [Range(0f, 1f)]
    public float disponibilidadAgua = 0.5f; // RF-07

    [Range(0f, 1f)]
    public float accesoVial = 0.5f; // RF-06

    public bool estudiada = false; // RF-04: ¿el jugador ya pagó por conocer el suelo?

    // true cuando el jugador ya decidió qué hacer en esta parcela durante el ciclo actual.
    // La UI usa esto para mostrar el check. El GameManager lo reinicia al avanzar de ciclo.
    public bool decisionTomada = false;

    public EstadoParcela estado = EstadoParcela.Vacia;
    public CultivoData cultivoActual;
    public int cicloEnQueSePlanto = -1;

    public bool ListaParaCosecha(int cicloActual)
    {
        if (estado != EstadoParcela.Plantada || cultivoActual == null) return false;
        return (cicloActual - cicloEnQueSePlanto) >= cultivoActual.duracionCiclos;
    }

    public void Plantar(CultivoData cultivo, int cicloActual)
    {
        cultivoActual = cultivo;
        cicloEnQueSePlanto = cicloActual;
        estado = EstadoParcela.Plantada;
    }

    public void Cosechar()
    {
        cultivoActual = null;
        cicloEnQueSePlanto = -1;
        estado = EstadoParcela.Vacia;
    }
}
