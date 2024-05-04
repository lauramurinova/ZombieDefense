using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum ZombieState
{
    Idle = 0,
    Walk  = 1,
    Attack = 2,
    Stunned = 3,
    Die = 3
}

public class Zombie : MonoBehaviour
{
    [HideInInspector] public Transform assignedWindow;
    
    [SerializeField] private DestroyablePlaneInteractor[] _destroyablePlaneInteractors;
    [SerializeField] public Animator _animator;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private ZombieController _zombieController;
    [SerializeField] private GameObject _body;
    [SerializeField] private OVRPassthroughLayer _attackLayer;
    [SerializeField] private OVRPassthroughLayer _aliveLayer;

    public ZombieState _state = ZombieState.Attack;
    private Vector3 _targetPosition;
    private GameObject _player;
    private GameObject _target;
    
    void Start()
    {
        GetComponent<AudioSource>().Play();
        _player = GameObject.Find("CenterEyeAnchor");
        
        if (name.Contains("Boss"))
        {
            WalkToPlayer();
        }
        else
        {
            GoToPlayer();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name.ToLower().Contains("bullet"))
        {
            Die();
        }

        if (other.gameObject.name.ToLower().Contains("cup") || other.transform.root.name.Contains("Camera"))
        {
            Stunned(other);
        }
    }

    private void Stunned(Collision other)
    {
        if(_state.Equals(ZombieState.Stunned)) return;

        if (other.gameObject.name.ToLower().Contains("cup"))
        {
            other.gameObject.GetComponent<Cup>().Break(other.contacts[0].point);
        }

        _aliveLayer.gameObject.SetActive(true);
        _attackLayer.gameObject.SetActive(false);
        var previousState = _state;
        _state = ZombieState.Stunned;
        _animator.SetTrigger("Stun");
        _agent.isStopped = true;
        StartCoroutine(Stun(previousState));
    }

    private IEnumerator Stun(ZombieState previousState)
    {
        yield return new WaitForSeconds(1.3f);
        // GoToIdle();
        switch (previousState)
        {
            case ZombieState.Attack:
            {
                GoToAttack();
                break;
            }
            case ZombieState.Walk:
            {
                WalkToPlayer();
                break;
            }
            case ZombieState.Idle:
            {
                GoToIdle();
                break;
            }
        }
    }

    private void GoToPlayer()
    {
        foreach (var controller in _destroyablePlaneInteractors)
        {
            controller.onDestroyed.RemoveAllListeners();
        }

        if (name.Contains("Boss"))
        {
            WalkToPlayer();
        }
        else
        {
            StartCoroutine(ClimbWindow());
        }
    }

    private void WalkToPlayer()
    {
        _aliveLayer.gameObject.SetActive(true);
        _attackLayer.gameObject.SetActive(false);
        transform.LookAt(_player.transform.position);
        _agent.isStopped = false;
        _agent.SetDestination(_player.transform.position);
        _state = ZombieState.Walk;
        _targetPosition = transform.position;
        _targetPosition = _player.transform.position;
        _target = _player;
        _animator.SetTrigger("Walk");
    }

    private void GoToWindow()
    {
        transform.LookAt(assignedWindow);
        _agent.isStopped = false;
        _agent.SetDestination(assignedWindow.position);
        _state = ZombieState.Walk;
        _targetPosition = assignedWindow.position;
        _target = assignedWindow.gameObject;
        _animator.SetTrigger("Walk");
    }

    private IEnumerator ClimbWindow()
    {
        transform.LookAt(_player.transform.position);
        _agent.isStopped = true;
        _animator.SetTrigger("Climb");
        yield return new WaitForSeconds(3f);

        WalkToPlayer();
    }

    private void GoToIdle()
    {
        _agent.isStopped = true;
        _state = ZombieState.Idle;
        _animator.SetTrigger("Idle");
    }
    
    public void GoToAttack()
    {
        _aliveLayer.gameObject.SetActive(false);
        _attackLayer.gameObject.SetActive(true);
        _agent.isStopped = true;
        transform.LookAt(_player.transform);
        _state = ZombieState.Attack;
        _animator.SetTrigger("Attack");
    }

    public void Die()
    {
        if (_state.Equals(ZombieState.Die)) return;
        
        _aliveLayer.gameObject.SetActive(true);
        _attackLayer.gameObject.SetActive(false);
        StopAllCoroutines();
        _agent.isStopped = true;
        _state = ZombieState.Die;
        _animator.SetTrigger("Die");
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        _zombieController.SpawnZombie();
        Invoke(nameof(DisableAnimator), 2f);
        enabled = false;
        foreach (var controller in _destroyablePlaneInteractors)
        {
            controller.onDestroyed.RemoveAllListeners();
        }
    }
    
    private void DisableAnimator()
    {
        GetComponent<AudioSource>().Stop();
        _animator.enabled = false;
    }

    void Update()
    {
        // if player is dead, dont do anything
        if (_state.Equals(ZombieState.Die)) return;
        
        if (_state.Equals(ZombieState.Walk))
        {
            HandleWalkUpdate();
        }

        if (_state.Equals(ZombieState.Idle))
        {
            HandleIdleUpdate();

            if (Vector3.Distance(transform.position, _player.transform.position) > 2f)
            {
                WalkToPlayer();

            }
        }

        if (_state.Equals(ZombieState.Attack))
        {
            if (Vector3.Distance(transform.position, _player.transform.position) > 2f)
            {
                WalkToPlayer();
            
            }
        }
    }

    private void HandleWalkUpdate()
    {
        // _agent.destination = new Vector3(_targetPosition.x, 0f, _targetPosition.z);

        if (Vector3.Distance(transform.position, _targetPosition) < 1.8f)
        {
            if (_target == assignedWindow.gameObject)
            {
                _target = null;
                StartCoroutine(ClimbWindow());
            }
            else
            {
                GoToIdle();
            }
        }
    }

    private void HandleIdleUpdate()
    {
        var colliders = Physics.OverlapSphere(transform.position, 1.8f);

        foreach (var collider in colliders)
        {
            // if player is around attack the player
            if (collider.name.Contains("Camera"))
            {
                GoToAttack();
            }
        }
    }
}
