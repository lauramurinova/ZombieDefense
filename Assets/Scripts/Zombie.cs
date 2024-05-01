using System.Collections;
using BlackWhale.DestructibleMeshSystem.Demo;
using UnityEngine;
using UnityEngine.AI;

public enum ZombieState
{
    Idle = 0,
    Walk  = 1,
    Attack = 2,
    Die = 3
}

public class Zombie : MonoBehaviour
{
    [HideInInspector] public Transform assignedWindow;
    
    [SerializeField] private ControllerDestroyCells[] _controllerDestroyCells;
    [SerializeField] public Animator _animator;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private ZombieController _zombieController;
    [SerializeField] private GameObject _body;

    public ZombieState _state = ZombieState.Attack;
    private Vector3 _targetPosition;
    private GameObject _player;
    
    void Start()
    {
        _player = GameObject.Find("OVRCameraRig");
        
        GoToWindow();
        foreach (var controller in _controllerDestroyCells)
        {
            controller._destroyed.AddListener(GoToPlayer);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name.ToLower().Contains("bullet"))
        {
            Die();
        }
    }

    private void GoToPlayer()
    {
        foreach (var controller in _controllerDestroyCells)
        {
            controller._destroyed.RemoveAllListeners();
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
        transform.LookAt(_player.transform.position);
        transform.position = assignedWindow.position;
        _agent.isStopped = false;
        _state = ZombieState.Walk;
        _targetPosition = _player.transform.position;
        _animator.SetTrigger("Walk");
    }

    private void GoToWindow()
    {
        transform.LookAt(assignedWindow);
        _agent.isStopped = false;
        _state = ZombieState.Walk;
        _targetPosition = assignedWindow.position;
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
        _agent.isStopped = true;
        transform.LookAt(_player.transform);
        _state = ZombieState.Attack;
        _animator.SetTrigger("Attack");
    }

    public void Die()
    {
        if (_state.Equals(ZombieState.Die)) return;
        
        StopAllCoroutines();
        _agent.isStopped = true;
        _state = ZombieState.Die;
        _animator.SetTrigger("Die");
        GetComponent<CapsuleCollider>().enabled = false;
        _zombieController.SpawnZombie();
        Invoke(nameof(DisableAnimator), 2f);
        enabled = false;
        foreach (var controller in _controllerDestroyCells)
        {
            controller._destroyed.RemoveAllListeners();
        }
    }
    
    private void DisableAnimator()
    {
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
        }

        if (_state.Equals(ZombieState.Attack))
        {
            // called when player is attacking
        }
    }

    private void HandleWalkUpdate()
    {
        _agent.destination = _targetPosition;

        if (Vector3.Distance(transform.position, _targetPosition) < 1.5f)
        {
            GoToIdle();
        }
    }

    private void HandleIdleUpdate()
    {
        var colliders = Physics.OverlapSphere(transform.position, 2f);

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
