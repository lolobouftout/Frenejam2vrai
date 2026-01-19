using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private TextMeshProUGUI instructionsText;
    [SerializeField] private TextMeshProUGUI statusText;
    //[SerializeField] private Image dashCooldownBar;

    [Header("Instructions")]
    [SerializeField] private string initialInstructions = "ESPACE : Dash | Pendant Dash : Dash Jump | Maintenir au sol : Accroupir | Maintenir en l'air : Descendre vite";
    [SerializeField] private string returnInstructions = "RETOUR ! Direction inversée - Survivez si vous pouvez...";

    //[Header("Cooldown Settings")]
    //[SerializeField] private float maxCooldownDisplay = 0.8f;
    //[SerializeField] private Color cooldownActiveColor = Color.red;
    //[SerializeField] private Color cooldownReadyColor = Color.green;

    private bool hasShownReturnMessage = false;

    void Start()
    {
        ValidateReferences();
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
        //UpdateCooldownBar();
    }

    void ValidateReferences()
    {
        if (player == null)
        {
            Debug.LogError("GameManager: Player reference is missing! Please assign it in the Inspector.");
        }

        if (instructionsText == null)
        {
            Debug.LogWarning("GameManager: Instructions Text is not assigned.");
        }

        if (statusText == null)
        {
            Debug.LogWarning("GameManager: Status Text is not assigned.");
        }

        //if (dashCooldownBar == null)
        //{
        //    Debug.LogWarning("GameManager: Dash Cooldown Bar is not assigned.");
        //}
    }

    void UpdateUI()
    {
        if (player == null) return;

        if (instructionsText != null && !player.HasKey())
        {
            instructionsText.text = initialInstructions;
        }

        if (instructionsText != null && player.HasKey() && !hasShownReturnMessage)
        {
            instructionsText.text = returnInstructions;
            hasShownReturnMessage = true;
        }

        if (statusText != null)
        {
            string dashStatus = player.IsDashing() ? " [DASH!]" : "";

            if (!player.HasKey())
            {
                statusText.text = "Objectif : Récupérer la clé " + dashStatus;
            }
            else
            {
                statusText.text = "Objectif : Retourner au départ  (IMPOSSIBLE)" + dashStatus;
            }
        }
    }

    //void UpdateCooldownBar()
    //{
    //    if (dashCooldownBar == null || player == null) return;

    //    float cooldown = player.GetDashCooldown();

    //    if (cooldown > 0)
    //    {
    //        dashCooldownBar.fillAmount = cooldown / maxCooldownDisplay;
    //        dashCooldownBar.color = cooldownActiveColor;
    //    }
    //    else
    //    {
    //        dashCooldownBar.fillAmount = 1f;
    //        dashCooldownBar.color = cooldownReadyColor;
    //    }
    //}
}