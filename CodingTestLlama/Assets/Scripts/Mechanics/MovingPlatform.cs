using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using Platformer.Mechanics;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private bool _randomizeStartingPos = true;

    [SerializeField] private Transform _beginTransform = null;
    private Vector3 _beginPos = Vector3.zero;
    [SerializeField] private Transform _endTransform = null;
    private Vector3 _endPos = Vector3.zero;

    [SerializeField] private float _movementSpeed = 3;
    private float _movementDuration = 0;
    
    private float _currentMovementTime = 0;

    private HashSet<Transform> _collidingTransforms = new HashSet<Transform>();


    private void Awake()
    {
        Assert.IsNotNull(_beginTransform);
        Assert.IsNotNull(_endTransform);

        if (_beginTransform)
        { _beginPos = _beginTransform.position; }
        if (_endTransform)
        { _endPos = _endTransform.position; }

        _movementDuration = Vector3.Distance(_beginPos, _endPos) / _movementSpeed;
        if (_randomizeStartingPos)
        { _currentMovementTime += Random.Range(0, _movementDuration); }
    }

    private void LateUpdate()
    {
        _currentMovementTime += Time.deltaTime;
        _collidingTransforms.RemoveWhere(element => element == null);

        Vector3 velocity = transform.position;

        float t = Mathf.Abs(1 + Mathf.Sin(2*Mathf.PI * (_currentMovementTime % _movementDuration) / _movementDuration))/2;
        transform.position = Vector3.Lerp( _beginPos, _endPos, t);

        velocity = (transform.position - velocity) / Time.deltaTime;

        foreach (Transform collidingTransform in _collidingTransforms)
        {
            PlayerController playerController = collidingTransform.GetComponent<PlayerController>();

            if (playerController)
            { playerController.ConstantVelocity = new Vector2(velocity.x, velocity.y*2); }
            playerController.ForceSetIsGrounded(true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_collidingTransforms.Contains(collision.transform))
        {
            PlayerController playerController = collision.transform.GetComponent<PlayerController>();
            if (!playerController)
            { return; }
            _collidingTransforms.Add(collision.transform); 
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (_collidingTransforms.Contains(collision.transform))
        {
            _collidingTransforms.Remove(collision.transform);

            PlayerController playerController = collision.transform.GetComponent<PlayerController>();

            if (playerController)
            { playerController.ConstantVelocity = Vector2.zero; }
        }
    }

    private void OnValidate()
    {
        Awake();
    }
}
