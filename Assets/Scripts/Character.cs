using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public EMovementType movementType;
    private Rigidbody _rb;
    private CharacterMover _mover;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _mover = new CharacterMover(this, movementType, _rb);
    }

    private void FixedUpdate()
    {
        _mover.MoveAndRotate();
    }

    void Update()
    {
        _mover.UpdateMeter();
        if (_mover.CanDash())
        {
            StartCoroutine(_mover.DashAction());
        }

        _mover.SprintAction();
    }
}
