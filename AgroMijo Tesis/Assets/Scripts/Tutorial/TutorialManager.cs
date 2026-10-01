using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instancia { get; private set; }

    [System.Serializable]
    public class TutorialStep
    {
        [TextArea(2, 5)]
        public string mensaje;

        [Header("Objetivo UI")]
        [Tooltip("Elemento UI a destacar. Dejar vacío si el objetivo está en el mundo.")]
        public RectTransform objetivo;

        [Header("Objetivo 3D")]
        [Tooltip("Elemento 3D del mundo a destacar. Usar solo si no hay objetivo UI.")]
        public Transform objetivoMundo;

        public TutorialCondicion condicion;
    }

    public List<TutorialStep> pasos = new List<TutorialStep>();

    private int pasoActual = -1;

    private void Awake()
    {
        Instancia = this;
    }

    private void Start()
    {
        AvanzarPaso();
    }

    public void AvanzarPaso()
    {
        pasoActual++;

        if (pasoActual >= pasos.Count)
        {
            TerminarTutorial();
            return;
        }

        TutorialStep paso = pasos[pasoActual];

        bool esManual = paso.condicion == TutorialCondicion.Manual;

        TutorialOverlayUI.Instancia.MostrarPaso(
            paso.mensaje,
            paso.objetivo,
            paso.objetivoMundo,
            esManual
        );
    }

    public void NotificarAccion(TutorialCondicion condicion)
    {
        if (pasoActual < 0 || pasoActual >= pasos.Count)
            return;

        if (pasos[pasoActual].condicion == condicion)
            AvanzarPaso();
    }

    private void TerminarTutorial()
    {
        TutorialOverlayUI.Instancia.Ocultar();
        LimpiarDatosTutorial();
        SceneLoader.Instance.LoadMainMenu();
    }

    public void SaltarTutorial()
    {
        TutorialOverlayUI.Instancia.Ocultar();
        LimpiarDatosTutorial();
        SceneLoader.Instance.LoadMainMenu();
    }

    private void LimpiarDatosTutorial()
    {
        string profileId = ProfileManager.Instance?.CurrentProfile?.id;

        if (!string.IsNullOrEmpty(profileId))
            SaveManager.Borrar(profileId);
    }
}