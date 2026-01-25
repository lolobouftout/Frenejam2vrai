using UnityEngine;

public class SkyColorChanger : MonoBehaviour
{
    [Header("Camera Reference")]
    [SerializeField] private Camera mainCamera;

    [Header("Sky Colors")]
    [SerializeField] private Color normalSkyColor = new Color(0.4f, 0.7f, 1f); // Bleu clair
    [SerializeField] private Color returnSkyColor = Color.red;
    [SerializeField] private float transitionSpeed = 2f;

    [Header("Player Reference")]
    [SerializeField] private PlayerMovement playerMovement;

    private Color targetColor;
    private Color currentColor;

    void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        {
            Debug.LogError("SkyColorChanger: No camera found!");
        }
    }

    void Start()
    {
        if (playerMovement == null)
        {
            playerMovement = FindFirstObjectByType<PlayerMovement>();
            if (playerMovement == null)
            {
                Debug.LogError("SkyColorChanger: PlayerMovement not found! Assign it manually.");
            }
        }

        if (mainCamera != null)
        {
            currentColor = normalSkyColor;
            mainCamera.backgroundColor = currentColor;
            targetColor = currentColor;
        }
    }

    void Update()
    {
        if (mainCamera == null || playerMovement == null)
            return;

        // Déterminer la couleur cible en fonction de si le joueur a la clé
        if (playerMovement.HasKey())
        {
            targetColor = returnSkyColor;
        }
        else
        {
            targetColor = normalSkyColor;
        }

        // Transition douce vers la couleur cible
        currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * transitionSpeed);
        mainCamera.backgroundColor = currentColor;
    }

    public void ResetSkyColor()
    {
        if (mainCamera != null)
        {
            currentColor = normalSkyColor;
            mainCamera.backgroundColor = currentColor;
            targetColor = currentColor;
        }
    }
}