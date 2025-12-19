using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject gameOverPanel; // Panel contenant tout l'UI de game over
    public TextMeshProUGUI gameOverText; // Texte "GAME OVER"
    public TextMeshProUGUI messageText; // Message "Il ne fallait pas me perdre de vue"
    public Button restartButton; // Bouton "RECOMMENCER"
    public TextMeshProUGUI restartButtonText; // Texte du bouton
    
    [Header("Animation Settings")]
    public float textFadeInDuration = 1f; // Durée d'apparition du texte
    public float delayBeforeButton = 1.5f; // Délai avant d'afficher le bouton
    
    private CanvasGroup panelCanvasGroup;
    private bool isShowing = false;
    private float showStartTime = 0f;

    void Start()
    {
        // Cacher le panel au début
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            
            // Ajouter un CanvasGroup pour le fade in
            panelCanvasGroup = gameOverPanel.GetComponent<CanvasGroup>();
            if (panelCanvasGroup == null)
            {
                panelCanvasGroup = gameOverPanel.AddComponent<CanvasGroup>();
            }
        }
        
        // Configurer le bouton
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
            restartButton.gameObject.SetActive(false);
        }
        
        // Cacher les textes au début
        if (gameOverText != null)
        {
            gameOverText.alpha = 0;
        }
        if (messageText != null)
        {
            messageText.alpha = 0;
        }
    }

    void Update()
    {
        // Animer l'apparition du texte
        if (isShowing)
        {
            float elapsedTime = Time.realtimeSinceStartup - showStartTime;
            
            // Fade in du texte principal
            if (gameOverText != null && elapsedTime < textFadeInDuration)
            {
                float alpha = Mathf.Clamp01(elapsedTime / textFadeInDuration);
                gameOverText.alpha = alpha;
            }
            else if (gameOverText != null)
            {
                gameOverText.alpha = 1;
            }
            
            // Fade in du message (commence un peu après)
            float messageStartDelay = 0.5f;
            if (messageText != null && elapsedTime > messageStartDelay)
            {
                float messageElapsed = elapsedTime - messageStartDelay;
                if (messageElapsed < textFadeInDuration)
                {
                    float alpha = Mathf.Clamp01(messageElapsed / textFadeInDuration);
                    messageText.alpha = alpha;
                }
                else
                {
                    messageText.alpha = 1;
                }
            }
            
            // Afficher le bouton après un délai
            if (restartButton != null && !restartButton.gameObject.activeSelf && elapsedTime > delayBeforeButton)
            {
                restartButton.gameObject.SetActive(true);
            }
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel == null) return;
        
        // Activer le panel
        gameOverPanel.SetActive(true);
        
        // Démarrer l'animation
        isShowing = true;
        showStartTime = Time.realtimeSinceStartup;
        
        // Réinitialiser les alphas
        if (gameOverText != null)
        {
            gameOverText.alpha = 0;
        }
        if (messageText != null)
        {
            messageText.alpha = 0;
        }
        
        // Afficher le curseur pour cliquer sur le bouton
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        Debug.Log("Affichage de l'écran de Game Over");
    }

    public void RestartGame()
    {
        Debug.Log("Redémarrage du jeu...");
        
        // Réinitialiser le timeScale au cas où
        Time.timeScale = 1;
        
        // Recharger la scène actuelle
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void QuitGame()
    {
        Debug.Log("Quitter le jeu...");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
