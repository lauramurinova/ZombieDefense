using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name.ToLower().Contains("voronoi"))
        {
            other.gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
