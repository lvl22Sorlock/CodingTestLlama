using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class HeadCollisionController : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController = null;
    [SerializeField] private string _levelTagName = "Level";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Assert.IsNotNull(_playerController);
        if (collision.tag != _levelTagName)
        { return; }

        _playerController.StopPositiveYMovement();
    }
}
