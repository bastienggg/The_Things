using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StartScreen : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI introText; // Texte principal
    public Button startButton; // Bouton "COMMENCER"
    public TextMeshProUGUI buttonText; // Texte du bouton
    
    [Header("Camera Settings")]
    public Camera playerCamera;
    public Transform monsterEyes; // Position des yeux du monstre
    public Transform monster; // Le monstre lui-même
    
    [Header("Text Settings")]
    public string[] textLines = new string[] {
        "Trouvez le portail pour vous échapper.",
        "Ne le perdez JAMAIS de vue..."
    };
    public float typewriterSpeed = 0.05f; // Vitesse d'apparition des lettres
    public float delayBetweenLines = 0.8f; // Délai entre les lignes
    
    [Header("Player References")]
    public GameObject player;
    public MonsterAI monsterAI; // Script du monstre
    
    [Header("Controls Settings")]
    public MonoBehaviour[] playerScriptsToDisable; // Scripts à désactiver pendant l'intro
    
    private bool introFinished = false;
    private bool gameStarted = false;

    void Start()
    {
        // S'assurer que le bouton est caché au début
        if (startButton != null)
        {
            startButton.gameObject.SetActive(false);
            startButton.onClick.AddListener(StartGame);
        }
        
        // Trouver automatiquement les références si non assignées
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        
        // Désactiver les contrôles du joueur au début
        DisablePlayerControls(true);
        
        // Désactiver le MonsterAI au début
        if (monsterAI != null)
        {
            monsterAI.enabled = false;
        }
        
        // Figer la caméra sur les yeux du monstre
        if (playerCamera != null && monsterEyes != null)
        {
            Vector3 direction = (monsterEyes.position - playerCamera.transform.position).normalized;
            playerCamera.transform.rotation = Quaternion.LookRotation(direction);
        }
        
        // Verrouiller le curseur
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Démarrer l'effet typewriter
        StartCoroutine(TypewriterEffect());
    }
    
    void Update()
    {
        // Pendant l'intro, forcer la caméra à regarder le monstre
        if (!gameStarted && playerCamera != null && monsterEyes != null)
        {
            Vector3 direction = (monsterEyes.position - playerCamera.transform.position).normalized;
            playerCamera.transform.rotation = Quaternion.LookRotation(direction);
        }
    }
    
    IEnumerator TypewriterEffect()
    {
        if (introText == null) yield break;
        
        introText.text = "";
        
        // Pour chaque ligne de texte
        foreach (string line in textLines)
        {
            // Afficher lettre par lettre
            foreach (char letter in line)
            {
                introText.text += letter;
                yield return new WaitForSeconds(typewriterSpeed);
            }
            
            // Ajouter un saut de ligne et attendre
            introText.text += "\n";
            yield return new WaitForSeconds(delayBetweenLines);
        }
        
        // Une fois le texte terminé, afficher le bouton
        yield return new WaitForSeconds(0.5f);
        
        if (startButton != null)
        {
            startButton.gameObject.SetActive(true);
        }
        
        // Déverrouiller le curseur pour cliquer sur le bouton
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        introFinished = true;
    }
    
    public void StartGame()
    {
        if (!introFinished || gameStarted) return;
        
        gameStarted = true;
        
        // Cacher l'UI
        if (introText != null)
        {
            introText.gameObject.SetActive(false);
        }
        if (startButton != null)
        {
            startButton.gameObject.SetActive(false);
        }
        
        // Réactiver les contrôles du joueur
        DisablePlayerControls(false);
        
        // Activer le MonsterAI
        if (monsterAI != null)
        {
            monsterAI.enabled = true;
        }
        
        // Reverrouiller le curseur pour le jeu
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Debug.Log("Le jeu commence !");
    }
    
    void DisablePlayerControls(bool disable)
    {
        if (player == null) return;
        
        // Désactiver/activer tous les scripts de mouvement
        MonoBehaviour[] scripts = player.GetComponentsInChildren<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != null && script != this)
            {
                // Ne désactiver que les scripts de contrôle (FirstPersonController, FirstPersonCamera, etc.)
                if (script.GetType().Name.Contains("FirstPerson") || 
                    script.GetType().Name.Contains("Player") ||
                    script.GetType().Name.Contains("Controller") ||
                    script.GetType().Name.Contains("Movement") ||
                    script.GetType().Name.Contains("Camera"))
                {
                    script.enabled = !disable;
                }
            }
        }
        
        // Désactiver aussi le CharacterController
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = !disable;
        }
    }
}
