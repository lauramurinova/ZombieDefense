using UnityEngine;
using UnityEngine.Events;

public class DestroyablePlaneInteractor : MonoBehaviour
{
     [SerializeField] private bool _hasRigidbody = false;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private GameObject _glassDebris;
        [SerializeField] private GameObject _wallDebris;
        [SerializeField] private float destroyRadius = 0.1f;
        [SerializeField] private Material _stencil;
    
        private bool _destroyedCells = false;

        public UnityEvent onDestroyed = new UnityEvent();

        public float _debrisTimeSpawn = 0.1f;
        private float _debrisTimer = 0f;

        void Update()
        {
            _debrisTimer += Time.deltaTime;
            if(_hasRigidbody) return;
            TryDestroyObjectInPath();
        }

        private void OnCollisionEnter(Collision other)
        {
            _destroyedCells = false;
            if (other.gameObject.layer == LayerMask.NameToLayer("Destroy"))
            {
                MakeRealWorldVisible(other);
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("NotDestroy"))
            {
                MakeVRWorldVisible(other);
            }

            if (_destroyedCells)
            {
                onDestroyed.Invoke();
            }
        }

        private void TryDestroyObjectInPath()
        {
            _destroyedCells = false;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, destroyRadius, _layerMask);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Destroy"))
                {
                    MakeRealWorldVisible(hitCollider);
                }
                else if (hitCollider.gameObject.layer == LayerMask.NameToLayer("NotDestroy"))
                {
                    MakeVRWorldVisible(hitCollider);
                }
            }

            if (_destroyedCells)
            {
                onDestroyed.Invoke();
            }
        }

        private void MakeRealWorldVisible(Collider hitCollider)
        {
            hitCollider.gameObject.SetActive(false);
            _destroyedCells = true;

            CreateDebris(hitCollider.transform.position, _glassDebris);
        }

        private void MakeVRWorldVisible(Collider hitCollider)
        {
            hitCollider.GetComponent<MeshRenderer>().material = _stencil;
            hitCollider.gameObject.layer = LayerMask.NameToLayer("Wall");
            _destroyedCells = true;
            CreateDebris(hitCollider.transform.position, _wallDebris);
        }
        
        private void MakeRealWorldVisible(Collision hitCollider)
        {
            hitCollider.gameObject.SetActive(false);
            _destroyedCells = true;

            CreateDebris(hitCollider.contacts[0].point, _glassDebris);
        }

        private void MakeVRWorldVisible(Collision hitCollider)
        {
            hitCollider.gameObject.GetComponent<MeshRenderer>().material = _stencil;
            hitCollider.gameObject.layer = LayerMask.NameToLayer("Wall");
            _destroyedCells = true;
            CreateDebris(hitCollider.contacts[0].point, _wallDebris);
        }

        private void CreateDebris(Vector3 hitPosition, GameObject debrisPrefab)
        {
            if (_debrisTimer > _debrisTimeSpawn)
            {
                _debrisTimer = 0f;
                GameObject debris = Instantiate(debrisPrefab, hitPosition, Quaternion.identity);
                Destroy(debris, 3f);
            }
        }
}
