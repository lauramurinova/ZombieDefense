using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject, 0.2f);
    }
}
