using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public EMovementType movementType;
    [SerializeField] private Rigidbody _rb;
    private CharacterMover _mover;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _mover = new CharacterMover(this, movementType, _rb);
    }

    private void FixedUpdate()
    {
        _mover.MoveAndRotate();
    }

    void Update()
    {
        _mover.Update();
    }
}
