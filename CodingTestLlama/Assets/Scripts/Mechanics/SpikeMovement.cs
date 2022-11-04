using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class SpikeMovement : MonoBehaviour
{
    [SerializeField] private Transform _beginTransform = null;
    private Vector3 _beginPos = Vector3.zero;
    [SerializeField] private Transform _endTransform = null;
    private Vector3 _endPos = Vector3.zero;

    [SerializeField] private float _speedToEndPos = 10;
    [SerializeField] private float _speedToBeginPos = 1;
    [SerializeField] private float _pauseDuration = 1;
    private float _distanceToMove = 0;
    private float _durationToEndPos = 0;
    private float _totalDuration = 0;
    private float _currentMovementTime = 0;

    private void Awake()
    {
        if (_beginTransform)
        {
            _beginPos = _beginTransform.position;
            Destroy(_beginTransform.gameObject);
        }
        if (_endTransform)
        {
            _endPos = _endTransform.position;
            Destroy(_endTransform.gameObject);
        }

        {
            _distanceToMove = (_beginPos - _endPos).magnitude;
            _durationToEndPos =
                ((_distanceToMove / 2) / _speedToEndPos);
            _totalDuration = 
                _durationToEndPos + ((_distanceToMove / 2) / _speedToBeginPos)
                + 2 * _pauseDuration;
        }
    }

    private void Update()
    {
        _currentMovementTime += Time.deltaTime;
        if (_currentMovementTime < _durationToEndPos)
        {       // move to end pos
            transform.position = Vector3.Lerp(_beginPos, _endPos, _currentMovementTime / _durationToEndPos);
        }
        else if (_currentMovementTime < _durationToEndPos + _pauseDuration)
        {       // pause
            transform.position = _endPos;
        }
        else if (_currentMovementTime < _totalDuration - _pauseDuration) 
        {       // move to begin pos
            float moveToBeginDuration = (_totalDuration - _durationToEndPos - (2*_pauseDuration));
            float currentMoveToBeginDuration = (_currentMovementTime - _durationToEndPos - _pauseDuration);

            transform.position = Vector3.Lerp(_endPos, _beginPos, currentMoveToBeginDuration / moveToBeginDuration);
        }
        else if (_currentMovementTime < _totalDuration)
        {       // pause
            transform.position = _beginPos;
        }
        else
        {       // reset movement
            _currentMovementTime = 0;
        }
    }
}
