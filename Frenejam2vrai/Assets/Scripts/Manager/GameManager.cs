using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerDash playerDash;
    [SerializeField] private TextMeshProUGUI instructionsText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Image dashCooldownBar;

    [Header("Instructions")]
    [SerializeField] private string initialInstructions = "ESPACE : Dash pour commencer";
    [SerializeField] private string runningInstructions = "ESPACE : Dash | Pendant Dash : Dash Jump | Maintenir en l'air : Descendre vite";
    [SerializeField] private string returnInstructions = "RETOUR ! Direction inversée - Survivez si vous pouvez...";
    [SerializeField] private string waitingAfterKeyInstructions = "ESPACE : Reprendre la course vers le départ...";

    private void Update()
    {
        UpdateInstructions();
        UpdateStatus();
    }

    private void UpdateInstructions()
    {
        if (instructionsText == null) return;

        if (!playerMovement.IsRunning)
        {
            instructionsText.text = initialInstructions;
        }
        else if (playerMovement.IsWaitingAfterKey)
        {
            instructionsText.text = waitingAfterKeyInstructions;
        }
        else if (playerMovement.HasKey())
        {
            instructionsText.text = returnInstructions;
        }
        else
        {
            instructionsText.text = runningInstructions;
        }
    }

    private void UpdateStatus()
    {
        if (statusText == null) return;

        if (!playerMovement.IsRunning)
        {
            statusText.text = "Prêt à commencer";
        }
        else if (playerMovement.IsWaitingAfterKey)
        {
            statusText.text = "CLÉ RÉCUPÉRÉE ! Appuyez sur ESPACE pour retourner au départ";
        }
        else if (playerMovement.HasKey())
        {
            statusText.text = "RETOUR AU DÉPART - SURVIVEZ !";
        }
        else
        {
            statusText.text = "Objectif : Récupérer la clé ?";
        }
    }
}