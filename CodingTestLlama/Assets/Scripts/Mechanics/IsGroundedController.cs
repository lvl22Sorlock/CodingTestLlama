using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Collider2D))]
public class IsGroundedController : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController = null;
    [SerializeField] private string _levelTagName = "Level";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Assert.IsNotNull(_playerController);
        if (collision.tag != _levelTagName)
        { return; }

        _playerController.SetIsGrounded(true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Assert.IsNotNull(_playerController);
        if (collision.tag != _levelTagName)
        { return; }

        _playerController.SetIsGrounded(false);
    }
}
