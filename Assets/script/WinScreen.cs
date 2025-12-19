using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject winPanel; // Panel avec fond noir
    public TextMeshProUGUI winText; // Texte "VOUS AVEZ GAGNÉ !"
    
    [Header("Settings")]
    public float displayDuration = 2f; // Durée d'affichage avant redémarrage
    
    private bool isShowing = false;
    private float showStartTime = 0f;

    void Start()
    {
        // Cacher le panel au début
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Attendre la durée d'affichage puis redémarrer
        if (isShowing)
        {
            float elapsedTime = Time.realtimeSinceStartup - showStartTime;
            
            if (elapsedTime >= displayDuration)
            {
                RestartGame();
            }
        }
    }

    public void ShowWin()
    {
        if (winPanel == null) return;
        
        // Afficher le panel
        winPanel.SetActive(true);
        isShowing = true;
        showStartTime = Time.realtimeSinceStartup;
        
        Debug.Log("Écran de victoire affiché !");
    }

    void RestartGame()
    {
        // Réinitialiser le timeScale au cas où
        Time.timeScale = 1;
        
        // Recharger la scène
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
