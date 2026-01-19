using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private TextMeshProUGUI instructionsText;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Instructions")]
    [SerializeField] private string initialInstructions = "ESPACE : Avancer/Sauter | Maintenir : Accroupir/Descendre vite";
    [SerializeField] private string returnInstructions = "RETOUR ! Direction inversée - Bonne chance...";

    private bool hasShownReturnMessage = false;

    void Start()
    {
        // Trouve le joueur si non assigné
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerController>();
        }

        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (player == null) return;

        // Affiche les instructions initiales
        if (instructionsText != null && !player.HasKey())
        {
            instructionsText.text = initialInstructions;
        }

        // Affiche le message de retour
        if (instructionsText != null && player.HasKey() && !hasShownReturnMessage)
        {
            instructionsText.text = returnInstructions;
            hasShownReturnMessage = true;
        }

        // Affiche le statut
        if (statusText != null)
        {
            if (!player.HasKey())
            {
                statusText.text = "Objectif : Récupérer la clé ";
            }
            else
            {
                statusText.text = "Objectif : Retourner au départ  (IMPOSSIBLE)";
            }
        }
    }
}