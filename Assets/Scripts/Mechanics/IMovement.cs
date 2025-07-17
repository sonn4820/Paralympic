using System.Collections;
using UnityEngine;

public interface IMovementInput
{
    public Vector3 GetMovingDirection();
    public float GetRotateDirection();
}

public enum EMovementType
{
    WHEEL_CHAIR,
    PROSTHETIC_LEG
}

public class WheelchairMovementInput : IMovementInput
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Vector3 GetMovingDirection()
    {
        Vector3 direction = Input.GetAxis("Vertical") * Vector3.forward;
        return direction.normalized;
    }

    public float GetRotateDirection()
    {
        return Input.GetAxis("Horizontal");
    }
}

public class ProstheticLegMovementInput : IMovementInput
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Vector3 GetMovingDirection()
    {
        Vector3 direction = Input.GetAxis("Vertical") * Vector3.forward + Input.GetAxis("Horizontal") * Vector3.right;
        return direction.normalized;
    }

    public float GetRotateDirection()
    {
        return 0;
    }
}

public class CharacterMover
{
    private Rigidbody _rb;
    private EMovementType _movementType;
    private IMovementInput _movementInput;
    private Character _character;

    private float _movementSpeed = 5f;
    private float _rotateSpeed = 60f; //for wheelchair
    private float _rotateInterpolation = 0.1f; //for prosthetic leg

    private float _stamina = 100f;
    private float _staminaRegenPerTick = 10f;

    private float _dashRange = 24f;
    private float _dashCooldown = 1f;
    private bool _canDash = true;
    private bool _isDashing = false;
    private float _dashingTime = 0.2f;
    private float _dashingStaminaCostPerUse = 50f;

    private bool _isSprinting = false;
    private float _sprintingSpeedBonusPercentage = 2.5f;
    private float _sprintingStaminaCostPerTick = 25f;
    private float _sprintingStaminaThresholdValue = 35f;


    public CharacterMover(Character character, EMovementType type, Rigidbody rb)
    {
        _character = character;
        _rb = rb;
        _movementType = type;
        switch (type)
        {
            case EMovementType.WHEEL_CHAIR:
                _movementInput = new WheelchairMovementInput();
                break;
            case EMovementType.PROSTHETIC_LEG:
                _movementInput = new ProstheticLegMovementInput();
                break;
        }
    }

    public void MoveAndRotate()
    {
        if (_isDashing) return;

        switch (_movementType)
        {
            case EMovementType.WHEEL_CHAIR:
                _rb.linearVelocity = _character.transform.forward *
                                     (_movementSpeed * _movementInput.GetMovingDirection().z);

                float deltaYawDegrees = (float)_movementInput.GetRotateDirection() * _rotateSpeed * Time.deltaTime;
                _character.transform.Rotate(0, deltaYawDegrees, 0);
                break;

            case EMovementType.PROSTHETIC_LEG:
                Vector3 direction = _movementInput.GetMovingDirection() * _movementSpeed;
                if (_isSprinting)
                {
                    direction *= _sprintingSpeedBonusPercentage;
                }

                _rb.linearVelocity = new Vector3(direction.x, _rb.linearVelocity.y, direction.z);
                if (direction != Vector3.zero)
                {
                    Quaternion ogRot = _character.transform.rotation;
                    Quaternion dirRot = Quaternion.LookRotation(direction);
                    _character.transform.rotation = Quaternion.Slerp(ogRot, dirRot, _rotateInterpolation);
                }

                break;
        }
    }

    public void UpdateMeter()
    {
        if (_isSprinting)
        {
            _stamina -= _sprintingStaminaCostPerTick * Time.deltaTime;
        }
        else
        {
            _stamina += _staminaRegenPerTick * Time.deltaTime;
        }

        _stamina = Mathf.Clamp(_stamina, 0f, 100f);
        Debug.Log(_stamina);
    }

    public bool CanDash()
    {
        return _movementType == EMovementType.WHEEL_CHAIR
               && _canDash
               && _stamina >= _dashingStaminaCostPerUse
               && Input.GetKeyDown(KeyCode.LeftShift);
    }

    public void SprintAction()
    {
        if (_movementType == EMovementType.PROSTHETIC_LEG 
            && ((_isSprinting && _stamina > 0f) || (!_isSprinting && _stamina > _sprintingStaminaThresholdValue))
           )
        {
            _isSprinting = Input.GetKey(KeyCode.LeftShift);
        }
        else
        {
            _isSprinting = false;
        }
    }


    public IEnumerator DashAction()
    {
        _canDash = false;
        _isDashing = true;
        _rb.useGravity = false;

        Vector3 dashDirection = _character.transform.forward * _dashRange;
        _rb.linearVelocity = new Vector3(dashDirection.x, _rb.linearVelocity.y, dashDirection.z);
        _stamina -= _dashingStaminaCostPerUse;
        yield return new WaitForSeconds(_dashingTime);

        _isDashing = false;
        _rb.useGravity = true;

        yield return new WaitForSeconds(_dashCooldown);
        _canDash = true;
    }
}