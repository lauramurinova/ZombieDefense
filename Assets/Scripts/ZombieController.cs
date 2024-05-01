using System.Collections;
using System.Collections.Generic;
using BlackWhale.DestructibleMeshSystem;
using Meta.XR.MRUtilityKit;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _zombies;
    [SerializeField] private GameObject _bossZombie;
    [SerializeField] private GameObject ceiling;
    [SerializeField] private Material _window;
    [SerializeField] private Material _stencil;
    [SerializeField] private MRUKDestructibleMesh _destructibleMesh;
    [SerializeField] private int _zombiesToSpawn = 4;

    private List<Transform> _windows = new List<Transform>();
    private bool _loaded = false;
    private int _zombiesSpawned = 0;

    public void LoadWindows()
    {
        var room = FindObjectOfType<MRUKRoom>();

        foreach (var anchor in room.Anchors)
        {
            if (anchor.name.ToLower().Contains("window"))
            {
                _windows.Add(anchor.transform);
            }
            
            if (anchor.name.ToLower().Contains("floor"))
            {
                anchor.gameObject.layer = LayerMask.NameToLayer("Floor");
            }
        }

        _loaded = true;

    }

    private IEnumerator SpawnBossZombie()
    {
        var ceilingeff = CreateDestructiveCeiling();

        var zombie = _bossZombie.GetComponent<Zombie>();
        var agent = _bossZombie.GetComponent<NavMeshAgent>();
        
        // set the transform
        zombie.assignedWindow = ceilingeff.transform;
        _bossZombie.transform.parent = ceilingeff.transform;
        var position = Vector3.zero;
        _bossZombie.transform.position = Vector3.zero;
        _bossZombie.transform.localPosition = position;
        zombie.enabled = false;
        
        // enable the zombie and jump animation
        _bossZombie.SetActive(true);
        agent.baseOffset = 2.5f;
        zombie._animator.SetTrigger("Jump");
        yield return new WaitForSeconds(1.2f);
        zombie.enabled = true;
        agent.baseOffset = 0f;
    }

    private GameObject CreateDestructiveCeiling()
    {
        var ceilingeff = GameObject.Find("CEILING_EffectMesh");
        ceilingeff.GetComponent<MeshRenderer>().material = _window;
        _destructibleMesh.DestroyWall(GameObject.Find("CEILING_EffectMesh"), ceiling);
        return ceilingeff;
    }

    public void SpawnZombie()
    {
        if (_windows.Count == 0 || _zombies.Count == 0 || _zombiesSpawned >= _zombiesToSpawn)
        {
            if (_loaded)
            {
                StartCoroutine(SpawnBossZombie());
                return;
            }
            else
            {
                return;
            }
        }

        _zombiesSpawned++;
        StartCoroutine(LoadZombie());
    }

    private IEnumerator LoadZombie()
    {
        yield return new WaitForSeconds(2f);
        
        // select random window and zombie
        var randomWindowIndex = Random.Range(0, _windows.Count);
        var randomZombieIndex = Random.Range(0, _zombies.Count);
        
        // assign the random window
        var zombie = _zombies[randomZombieIndex];
        zombie.GetComponent<Zombie>().assignedWindow = _windows[randomWindowIndex];
        zombie.transform.parent = _windows[randomWindowIndex];
        SetObjectPosition(zombie);
        zombie.SetActive(true);

        // remove the window and the zombie from the list
        _zombies.Remove(_zombies[randomZombieIndex]);
        _windows.Remove(_windows[randomWindowIndex]);
    }

    private static void SetObjectPosition(GameObject zombie)
    {
        var position = Vector3.zero;
        position.y -= 1.6f;
        zombie.transform.position = Vector3.zero;
        zombie.transform.localPosition = position;
        zombie.transform.eulerAngles = Vector3.zero;
        zombie.transform.localEulerAngles = Vector3.zero;
        zombie.transform.position += -zombie.transform.forward * 7f;
    }
}
