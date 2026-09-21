// Los tipos de acción que el jugador puede elegir para una parcela en un ciclo.
// "Ninguna" significa que aún no tomó decisión.
public enum TipoDecision
{
    Ninguna,
    Plantar,
    Estudiar,
    Mejorar,
    Esperar
}

// Guarda la decisión que el jugador eligió para una parcela,
// pero sin aplicarla todavía. Se aplica al presionar "Avanzar ciclo".
[System.Serializable]
public class DecisionPendiente
{
    public TipoDecision tipo = TipoDecision.Ninguna;
    public CultivoData cultivoSeleccionado; // solo se usa cuando tipo == Plantar
}
