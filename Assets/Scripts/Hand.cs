using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private AudioSource _hit;
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name.ToLower().Contains("zombie"))
        {
            _hit.Play();
        }
    }
}
