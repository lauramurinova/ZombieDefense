using UnityEngine;

public class Cup : MonoBehaviour
{
   [SerializeField] private GameObject _cupDebris;
   
   private bool _broken;
   private float _breakTime = 0.15f;
   private float _breakTimer = 0f;

   private void Start()
   {
      GetComponent<DestroyablePlaneInteractor>().onDestroyed.AddListener(delegate { Break(transform.position); });
   }

   private void Update()
   {
      _breakTimer += Time.deltaTime;
   }

   public void Break(Vector3 hitPosition)
   {
      if(_broken) return;

      if (_breakTimer > _breakTime)
      {
         _breakTimer = 0f;
         GameObject debris = Instantiate(_cupDebris, hitPosition, Quaternion.identity);
         Destroy(debris, 3f);
         Destroy(gameObject);
      }
   }
}
