using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Vérifie si c'est le joueur qui entre dans le trigger
        if (other.CompareTag("Player"))
        {
            Debug.Log("Le joueur a atteint le portail !");
            
            // Appeler la méthode WinGame du GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.WinGame();
            }
        }
    }
}
