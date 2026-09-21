using UnityEngine;

public enum EstadoParcela
{
    Vacia,
    Plantada
}

[System.Serializable]
public class Parcela
{
    public string nombreParcela = "Parcela 1";
    public TipoSuelo tipoSuelo;

    [Range(0f, 1f)]
    public float aguaBase = 0.5f; // único valor configurable en Inspector; la mejora de riego lo sube

    // Valor runtime — no aparece en el Inspector porque se calcula cada ciclo.
    // Se inicializa igual a aguaBase al arrancar el juego.
    [System.NonSerialized]
    public float disponibilidadAgua;

    [Range(0f, 1f)]
    public float accesoVial = 0.5f;

    public bool estudiada = false;

    // ── Sistema de mejoras ─────────────────────────────────────────────────────
    // Nivel 0: sin mejoras
    // Nivel 1: Acceso vial      → reduce impacto de eventos de infraestructura
    // Nivel 2: Sistema de riego → sube aguaBase permanentemente
    // Nivel 3: Fertilización    → +20% al rendimiento de cada cosecha aquí
    [Range(0, 3)]
    public int nivelMejora = 0;

    // Propiedades calculadas — la lógica lee estas, no el nivelMejora directamente
    public bool TieneAccesoVial    => nivelMejora >= 1;
    public bool TieneSistemaRiego  => nivelMejora >= 2;
    public bool TieneFertilizacion => nivelMejora >= 3;

    public bool decisionTomada = false;
    public DecisionPendiente decisionPendiente = new DecisionPendiente();

    public EstadoParcela estado = EstadoParcela.Vacia;
    public CultivoData cultivoActual;
    public int cicloEnQueSePlanto = -1;

    // ── Variación de agua por ciclo ────────────────────────────────────────────
    // Llamado desde GameManager al inicio de cada ciclo.
    // fluctuacion: valor entre -0.05 y 0.05 generado por el GameManager.
    // Llamado una vez al inicio del juego para sincronizar el valor runtime con el base
    public void InicializarAgua()
    {
        disponibilidadAgua = aguaBase;
    }

    public void AplicarVariacionAgua(float fluctuacion)
    {
        disponibilidadAgua = Mathf.Clamp(aguaBase + fluctuacion, 0.1f, 1f);
    }

    public bool ListaParaCosecha(int cicloActual)
    {
        if (estado != EstadoParcela.Plantada || cultivoActual == null) return false;
        return (cicloActual - cicloEnQueSePlanto) >= cultivoActual.duracionCiclos;
    }

    public void Plantar(CultivoData cultivo, int cicloActual)
    {
        cultivoActual      = cultivo;
        cicloEnQueSePlanto = cicloActual;
        estado             = EstadoParcela.Plantada;
    }

    public void Cosechar()
    {
        cultivoActual      = null;
        cicloEnQueSePlanto = -1;
        estado             = EstadoParcela.Vacia;
    }
}
