using System.Collections;
using UnityEngine;

public class MissileLedBlink : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int blinkDelay = 1;

    [ColorUsage(true, true)]
    [SerializeField] private Color _onEmissionColor;
    [ColorUsage(true, true)]
    [SerializeField] private Color _offEmissionColor = Color.black;

    [Header("Reference")]
    [SerializeField] private GameObject _objet;

    private MeshRenderer meshRenderer;

    // Start est appelé avant la première mise à jour de la frame
    void Start()
    {
        meshRenderer = this.gameObject.GetComponent<MeshRenderer>(); // Stockage de la référence du MeshRenderer
        StartCoroutine(LightSequences());
    }

    IEnumerator LightSequences()
    {
        while (true)
        {

            meshRenderer.material.EnableKeyword("_EMISSION");
            meshRenderer.material.SetColor("_EmissionColor", _onEmissionColor);
            yield return new WaitForSeconds(blinkDelay);

            meshRenderer.material.DisableKeyword("_EMISSION");
            meshRenderer.material.SetColor("_EmissionColor", _offEmissionColor);
            yield return new WaitForSeconds(blinkDelay);
        }
    }
}
