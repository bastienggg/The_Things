using UnityEngine;

public class HorrorSoundManager : MonoBehaviour
{
    [Header("Ambient Sounds")]
    public AudioClip[] atmosphereSounds;
    public AudioSource atmosphereSource;
    
    [Header("Random Scary Sounds")]
    public AudioClip[] randomScareClips;
    public float minTimeBetweenScares = 20f;
    public float maxTimeBetweenScares = 60f;
    private float nextScareTime;
    
    [Header("Footstep Sounds")]
    public AudioClip[] playerFootsteps;
    public float footstepInterval = 0.5f;
    private float lastFootstepTime;
    
    private AudioSource randomSoundSource;
    private FirstPersonController playerController;
    
    void Start()
    {
        // Configuration de l'ambiance atmosphérique
        if (atmosphereSource == null)
        {
            atmosphereSource = gameObject.AddComponent<AudioSource>();
        }
        atmosphereSource.loop = true;
        atmosphereSource.volume = 0.3f;
        atmosphereSource.spatialBlend = 0f; // Son 2D
        
        // Jouer une ambiance aléatoire
        if (atmosphereSounds != null && atmosphereSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, atmosphereSounds.Length);
            if (atmosphereSounds[randomIndex] != null)
            {
                atmosphereSource.clip = atmosphereSounds[randomIndex];
                atmosphereSource.Play();
            }
        }
        
        // Audio source pour les sons aléatoires
        randomSoundSource = gameObject.AddComponent<AudioSource>();
        randomSoundSource.spatialBlend = 0f;
        randomSoundSource.volume = 0.5f;
        
        // Planifier le premier son effrayant
        ScheduleNextScare();
        
        // Trouver le contrôleur du joueur
        playerController = FindObjectOfType<FirstPersonController>();
    }
    
    void Update()
    {
        // Jouer des sons effrayants aléatoires
        if (Time.time >= nextScareTime && randomScareClips != null && randomScareClips.Length > 0)
        {
            PlayRandomScareSound();
            ScheduleNextScare();
        }
        
        // Sons de pas du joueur (optionnel)
        PlayFootstepSounds();
    }
    
    void PlayRandomScareSound()
    {
        int randomIndex = Random.Range(0, randomScareClips.Length);
        if (randomScareClips[randomIndex] != null)
        {
            randomSoundSource.PlayOneShot(randomScareClips[randomIndex]);
            Debug.Log("Son effrayant joué !");
        }
    }
    
    void ScheduleNextScare()
    {
        nextScareTime = Time.time + Random.Range(minTimeBetweenScares, maxTimeBetweenScares);
    }
    
    void PlayFootstepSounds()
    {
        if (playerController == null || playerFootsteps == null || playerFootsteps.Length == 0)
            return;
        
        // Vérifier si le joueur se déplace (simplification)
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || 
            Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if (Time.time - lastFootstepTime > footstepInterval)
            {
                int randomIndex = Random.Range(0, playerFootsteps.Length);
                if (playerFootsteps[randomIndex] != null)
                {
                    randomSoundSource.PlayOneShot(playerFootsteps[randomIndex], 0.3f);
                }
                lastFootstepTime = Time.time;
            }
        }
    }
}
