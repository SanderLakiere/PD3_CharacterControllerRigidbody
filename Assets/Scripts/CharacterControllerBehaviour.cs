using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class CharacterControllerBehaviour : MonoBehaviour {
    private CapsuleCollider _capsuleCollider;
    private Rigidbody _rb;
    private bool _jump;
    private Vector3 _movement;

    private Vector3 _velocity;

    [SerializeField]
    private Transform _absoluteForward;

    [SerializeField]
    private float _acceleration = 3f;


    void Start() {
        SetupReferences();
    }

    void Update() {
        HandleJumpInput();
        HandleMovementInput();
    }

    void FixedUpdate() {
        _isGroundedNeedsUpdate = true;
        HandleMovement();
        _rb.MovePosition(_rb.position + _velocity * Time.fixedDeltaTime);    
    }

    private void HandleJumpInput() {
        if (Input.GetButtonDown("Jump")) {
            _jump = true;
        }
    }

    private void HandleMovement() {
        Vector3 xzAbsoluteForward = Vector3.Scale(_absoluteForward.forward, new Vector3(1f, 0f, 1f));
        Quaternion forwardRotation = Quaternion.LookRotation(xzAbsoluteForward);
        Vector3 relativeMovement = forwardRotation * _movement;
        _velocity += relativeMovement * _acceleration * Time.fixedDeltaTime;
    }

    private void HandleMovementInput() {
        _movement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
    }

    private void SetupReferences() {
        _capsuleCollider = this.GetComponent<CapsuleCollider>();
        _rb = this.GetComponent<Rigidbody>();

        #if DEBUG
                Assert.IsNotNull(_rb, "No rigidbody found");
                Assert.IsNotNull(_capsuleCollider, "No capsule collider found");
        #endif
    }

    bool _isGrounded = false;
    bool _isGroundedNeedsUpdate = true;

    private bool IsGrounded() {

        if (_isGroundedNeedsUpdate) {
            Vector3 rayCenter = _capsuleCollider.center;
            float rayLength = _capsuleCollider.bounds.extents.y * 0.1f;
            float sphereRadius = _capsuleCollider.radius * 0.9f;
            RaycastHit hitInfo;

            // bool isGrounded = Physics.Raycast(rayCenter, Vector3.down, out hitInfo, rayLength);

            //SphereCast way
            bool isGrounded = Physics.SphereCast(rayCenter, sphereRadius, Vector3.down, out hitInfo, rayLength);

            _isGrounded = isGrounded && (Vector3.Dot(hitInfo.normal, Vector3.up) > Mathf.Cos(Mathf.PI / 3f));
            _isGroundedNeedsUpdate = false;
        }

        return _isGrounded;
    }
}
