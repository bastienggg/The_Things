using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    [Header("Monster Settings")]
    public Transform player;
    public float moveSpeed = 2f;
    public float detectionAngle = 45f; // Angle de vue du joueur
    public float stopDistance = 2f; // Distance minimale avant d'arrêter
    
    [Header("Teleportation")]
    public bool canTeleport = true;
    public float teleportChance = 0.1f; // 10% de chance par seconde
    public float minTeleportDistance = 5f;
    public float maxTeleportDistance = 15f;
    public float teleportCooldown = 3f; // Temps minimum entre deux téléportations
    
    [Header("Detection")]
    public Camera playerCamera;
    public LayerMask monsterLayer; // Layer du monstre pour la détection
    
    [Header("Animation Names")]
    public string idleAnimationName = "MutantMonster2@idle1"; // Nom de l'animation idle
    public string walkAnimationName = "MutantMonster2@walk2"; // Nom de l'animation de marche
    public string frozenAnimationName = "MutantMonster2@idle2"; // Animation quand le monstre est figé (regardé)
    
    [Header("Sound Effects")]
    public AudioClip teleportSound; // Son quand il se téléporte
    public AudioClip attackSound; // Son d'attaque/jumpscare (SCREAMER)
    public AudioClip[] walkSounds; // Sons de pas
    private AudioSource audioSource;
    
    [Header("Jumpscare Settings")]
    public Transform monsterEyes; // Position des yeux du monstre (optionnel)
    public float jumpscareVolume = 2f; // Volume du screamer (peut dépasser 1)
    public float jumpscareDuration = 1.5f; // Durée avant game over
    public bool disablePlayerControls = true; // Désactiver les contrôles pendant le screamer
    
    private Animator animator;
    private bool isBeingWatched = false;
    private Vector3 lastPosition;
    private CharacterController characterController;
    private bool isMoving = false;
    private float lastTeleportTime = 0f;
    private bool jumpscareActive = false;
    private float jumpscareStartTime = 0f;
    private MonoBehaviour[] disabledPlayerScripts; // Scripts désactivés pendant le jumpscare
    private bool controlsDisabled = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        
        // Si pas d'AudioSource, en créer un
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // Son 3D
            audioSource.minDistance = 5f;
            audioSource.maxDistance = 50f;
        }
        
        lastPosition = transform.position;
        
        // Trouver le joueur automatiquement si non assigné
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
        
        // Trouver la caméra automatiquement
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        
        // Jouer l'animation idle au départ
        if (animator != null)
        {
            animator.Play(idleAnimationName);
        }
    }

    void Update()
    {
        if (player == null || playerCamera == null) return;
        
        // Gérer le jumpscare
        if (jumpscareActive)
        {
            HandleJumpscare();
            return; // Ne rien faire d'autre pendant le jumpscare
        }

        // Vérifier si le joueur regarde le monstre
        isBeingWatched = IsPlayerLookingAtMonster();

        // Distance entre le monstre et le joueur
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (!isBeingWatched && distanceToPlayer > stopDistance)
        {
            // Essayer de se téléporter aléatoirement
            if (canTeleport && Time.time - lastTeleportTime > teleportCooldown)
            {
                if (Random.value < teleportChance * Time.deltaTime)
                {
                    TryTeleport();
                }
            }
            
            // Le joueur ne regarde pas : le monstre avance
            Vector3 direction = (player.position - transform.position).normalized;
            
            // Bouger vers le joueur avec CharacterController si disponible
            if (characterController != null)
            {
                // Appliquer la gravité
                Vector3 movement = direction * moveSpeed * Time.deltaTime;
                movement.y = -9.81f * Time.deltaTime; // Gravité pour rester au sol
                characterController.Move(movement);
            }
            else
            {
                transform.position += direction * moveSpeed * Time.deltaTime;
            }
            
            // Regarder vers le joueur
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
            
            isMoving = true;
            
            // Animation de marche
            if (animator != null)
            {
                animator.Play(walkAnimationName);
            }
        }
        else
        {
            isMoving = false;
            
            // Le joueur regarde : le monstre se fige complètement (pas d'animation)
            if (animator != null)
            {
                // Mettre l'animator en pause pour figer complètement le monstre
                animator.speed = 0;
            }
        }

        // Vérifier si le monstre a atteint le joueur
        if (distanceToPlayer <= stopDistance)
        {
            OnReachPlayer();
        }
        
        // Réactiver l'animator si le monstre n'est pas regardé
        if (!isBeingWatched && animator != null)
        {
            animator.speed = 1;
        }
    }

    bool IsPlayerLookingAtMonster()
    {
        if (playerCamera == null) return false;

        // Direction de la caméra vers le monstre
        Vector3 directionToMonster = (transform.position - playerCamera.transform.position).normalized;
        
        // Direction dans laquelle la caméra regarde
        Vector3 cameraForward = playerCamera.transform.forward;

        // Calculer l'angle entre les deux directions
        float angle = Vector3.Angle(cameraForward, directionToMonster);

        // Vérifier si le monstre est dans le champ de vision (angle de détection)
        if (angle < detectionAngle)
        {
            // Distance au monstre
            float distanceToMonster = Vector3.Distance(playerCamera.transform.position, transform.position);
            
            // Faire plusieurs raycasts pour être sûr de détecter le monstre
            RaycastHit hit;
            
            // Raycast vers le centre du monstre
            if (Physics.Raycast(playerCamera.transform.position, directionToMonster, out hit, distanceToMonster + 1f))
            {
                // Vérifier si on a touché le monstre ou un de ses enfants
                if (IsHitMonster(hit.transform))
                {
                    return true;
                }
            }
            
            // Raycast vers le haut du monstre (cas où on voit juste la tête)
            Vector3 monsterTop = transform.position + Vector3.up * 1f;
            Vector3 directionToTop = (monsterTop - playerCamera.transform.position).normalized;
            
            if (Physics.Raycast(playerCamera.transform.position, directionToTop, out hit, distanceToMonster + 1f))
            {
                if (IsHitMonster(hit.transform))
                {
                    return true;
                }
            }
            
            // Si dans l'angle de vision mais pas de raycast hit, considérer visible
            // (cas où le monstre n'a pas de collider ou problème de raycast)
            if (angle < detectionAngle * 0.8f) // Plus strict pour éviter faux positifs
            {
                return true;
            }
        }

        return false;
    }
    
    bool IsHitMonster(Transform hitTransform)
    {
        // Vérifier si le transform touché est le monstre ou un de ses enfants
        Transform current = hitTransform;
        while (current != null)
        {
            if (current == transform)
            {
                return true;
            }
            current = current.parent;
        }
        return false;
    }
    
    void TryTeleport()
    {
        if (player == null || playerCamera == null) return;
        
        // Essayer plusieurs fois de trouver une position valide derrière le joueur
        for (int i = 0; i < 10; i++)
        {
            // Obtenir la direction où regarde le joueur
            Vector3 playerForward = playerCamera.transform.forward;
            playerForward.y = 0; // Garder sur le plan horizontal
            playerForward.Normalize();
            
            // Calculer une position derrière le joueur
            Vector3 behindPlayer = -playerForward; // Direction opposée
            
            // Ajouter une variation aléatoire (gauche/droite et distance)
            float randomAngle = Random.Range(-45f, 45f); // Variation de -45 à +45 degrés
            Vector3 randomBehind = Quaternion.Euler(0, randomAngle, 0) * behindPlayer;
            
            // Distance aléatoire derrière le joueur
            float randomDistance = Random.Range(minTeleportDistance, maxTeleportDistance);
            Vector3 potentialPosition = player.position + randomBehind * randomDistance;
            
            // Vérifier que la position n'est PAS dans le champ de vision du joueur
            if (!IsPositionInPlayerView(potentialPosition))
            {
                // Vérifier qu'il y a du sol à cette position
                RaycastHit hit;
                if (Physics.Raycast(potentialPosition + Vector3.up * 5f, Vector3.down, out hit, 10f))
                {
                    // Téléporter le monstre
                    Vector3 newPosition = hit.point;
                    
                    if (characterController != null)
                    {
                        characterController.enabled = false;
                        transform.position = newPosition;
                        characterController.enabled = true;
                    }
                    else
                    {
                        transform.position = newPosition;
                    }
                    
                    // Regarder vers le joueur
                    transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                    
                    lastTeleportTime = Time.time;
                    
                    // Jouer le son de téléportation
                    if (teleportSound != null && audioSource != null)
                    {
                        audioSource.PlayOneShot(teleportSound);
                    }
                    
                    Debug.Log("Monstre téléporté derrière le joueur !");
                    break;
                }
            }
        }
    }
    
    bool IsPositionInPlayerView(Vector3 position)
    {
        if (playerCamera == null) return false;
        
        // Direction de la caméra vers la position
        Vector3 directionToPosition = (position - playerCamera.transform.position).normalized;
        Vector3 cameraForward = playerCamera.transform.forward;
        
        // Calculer l'angle
        float angle = Vector3.Angle(cameraForward, directionToPosition);
        
        // Si l'angle est petit, la position est dans le champ de vision
        return angle < detectionAngle * 1.2f; // Un peu plus large pour éviter de spawn trop près du bord
    }
    
    void OnReachPlayer()
    {
        if (jumpscareActive) return; // Ne déclencher qu'une seule fois
        
        // Activer le jumpscare
        jumpscareActive = true;
        jumpscareStartTime = Time.time;
        
        // Jouer le son de jumpscare TRÈS FORT
        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound, jumpscareVolume);
        }
        
        // Jouer une animation d'attaque si disponible
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        
        Debug.Log("JUMPSCARE ! Le monstre a attrapé le joueur !");
    }
    
    void HandleJumpscare()
    {
        // Bloquer les contrôles du joueur UNE SEULE FOIS au début
        if (disablePlayerControls && !controlsDisabled)
        {
            controlsDisabled = true;
            
            // Désactiver TOUS les scripts sur le joueur et la caméra
            if (player != null)
            {
                MonoBehaviour[] playerScripts = player.GetComponents<MonoBehaviour>();
                foreach (var script in playerScripts)
                {
                    if (script != null && script.enabled && script != this)
                    {
                        script.enabled = false;
                    }
                }
            }
            
            // Désactiver les scripts de la caméra aussi
            if (playerCamera != null)
            {
                MonoBehaviour[] cameraScripts = playerCamera.GetComponents<MonoBehaviour>();
                foreach (var script in cameraScripts)
                {
                    if (script != null && script.enabled)
                    {
                        script.enabled = false;
                    }
                }
            }
            
            // Désactiver le CharacterController pour qu'il ne puisse pas bouger
            if (player != null)
            {
                CharacterController playerController = player.GetComponent<CharacterController>();
                if (playerController != null)
                {
                    playerController.enabled = false;
                }
            }
            
            // Verrouiller le curseur pour qu'il ne puisse pas bouger la caméra
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        // Forcer la caméra à regarder les yeux du monstre de manière TRÈS agressive
        if (playerCamera != null)
        {
            // Déterminer la cible (yeux du monstre ou position haute par défaut)
            Vector3 targetPosition = monsterEyes != null ? monsterEyes.position : transform.position + Vector3.up * 1.5f;
            
            // Calculer la direction vers les yeux
            Vector3 directionToEyes = (targetPosition - playerCamera.transform.position).normalized;
            
            // Rotation cible
            Quaternion targetRotation = Quaternion.LookRotation(directionToEyes);
            
            // Forcer la rotation de la caméra vers le monstre (TRÈS rapide et fluide)
            playerCamera.transform.rotation = Quaternion.Slerp(
                playerCamera.transform.rotation, 
                targetRotation, 
                Time.deltaTime * 25f // Vitesse très rapide
            );
            
            // Forcer aussi la rotation du parent (le joueur) pour être sûr
            if (player != null)
            {
                Vector3 directionToMonster = (transform.position - player.position).normalized;
                directionToMonster.y = 0; // Garder horizontal
                if (directionToMonster != Vector3.zero)
                {
                    Quaternion playerTargetRotation = Quaternion.LookRotation(directionToMonster);
                    player.rotation = Quaternion.Slerp(player.rotation, playerTargetRotation, Time.deltaTime * 20f);
                }
            }
        }
        
        // Vérifier si le jumpscare est terminé
        if (Time.time - jumpscareStartTime >= jumpscareDuration)
        {
            // Game Over
            Debug.Log("GAME OVER !");
            
            // Appeler le GameManager pour gérer le game over
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
            else
            {
                // Fallback si pas de GameManager
                Time.timeScale = 0;
            }
        }
    }

    // Visualisation dans l'éditeur
    void OnDrawGizmos()
    {
        if (player != null)
        {
            // Ligne vers le joueur
            Gizmos.color = isBeingWatched ? Color.red : Color.green;
            Gizmos.DrawLine(transform.position, player.position);
            
            // Sphere de distance d'arrêt
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, stopDistance);
        }
    }
}
