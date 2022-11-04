using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Collider2D))]
public class WallJumpBehaviour : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Assert.IsNotNull(_playerController);

        _playerController.TouchedWall();
    }
}
