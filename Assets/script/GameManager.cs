using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game State")]
    public bool gameStarted = false;
    public bool gameOver = false;
    
    [Header("Objective")]
    public GameObject portal; // Le portail à trouver
    
    [Header("References")]
    public GameObject player;
    public MonsterAI monsterAI;
    public GameOverScreen gameOverScreen; // Référence à l'écran de Game Over
    public WinScreen winScreen; // Référence à l'écran de victoire
    
    private static GameManager instance;
    
    public static GameManager Instance
    {
        get { return instance; }
    }
    
    void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    
    void Start()
    {
        // Trouver les références automatiquement si non assignées
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        
        if (monsterAI == null)
        {
            monsterAI = FindObjectOfType<MonsterAI>();
        }
        
        if (gameOverScreen == null)
        {
            gameOverScreen = FindObjectOfType<GameOverScreen>();
        }
        
        if (winScreen == null)
        {
            winScreen = FindObjectOfType<WinScreen>();
        }
        
        if (portal == null)
        {
            portal = GameObject.FindGameObjectWithTag("Portal");
        }
    }
    
    void Update()
    {
        // La détection du portail se fait maintenant via OnTriggerEnter
    }
    
    public void StartGame()
    {
        gameStarted = true;
        Debug.Log("Le jeu a démarré !");
    }
    
    public void GameOver()
    {
        if (gameOver) return;
        
        gameOver = true;
        Debug.Log("GAME OVER !");
        
        // Afficher l'écran de Game Over
        if (gameOverScreen != null)
        {
            gameOverScreen.ShowGameOver();
        }
        else
        {
            // Fallback : recharger après 2 secondes
            Debug.LogWarning("GameOverScreen non trouvé ! Redémarrage automatique...");
            Invoke("RestartGame", 2f);
        }
        
        // Note : On ne met plus Time.timeScale = 0 car on veut que l'UI soit interactive
    }
    
    public void WinGame()
    {
        if (gameOver) return;
        
        gameOver = true;
        Debug.Log("VOUS AVEZ GAGNÉ ! Vous avez trouvé le portail !");
        
        // Afficher l'écran de victoire
        if (winScreen != null)
        {
            winScreen.ShowWin();
        }
        else
        {
            // Fallback : recharger après 2 secondes
            Debug.LogWarning("WinScreen non trouvé ! Redémarrage automatique...");
            Invoke("RestartGame", 2f);
        }
    }
    
    public void RestartGame()
    {
        // Réinitialiser le timeScale
        Time.timeScale = 1;
        
        // Recharger la scène actuelle
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
