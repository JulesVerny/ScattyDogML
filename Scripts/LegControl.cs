using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegControl : MonoBehaviour
{
    // ==================================================================================
    private Rigidbody LegRigidbody;
    float RequiredRotation = 90.0f;
    // ==================================================================================
    public void InitialiseLeg()
    {
        LegRigidbody = this.GetComponent<Rigidbody>();
        if (LegRigidbody == null) Debug.Log("[ERROR]:  Leg Unable to get its RigidBody Joint");

        ResetLegMotion();
    }  // InitialiseLeg
    // ==================================================================================
    
    public void RotateClockwise()
    {
        float CurrentLegRotation = this.transform.localRotation.eulerAngles.z;

        // Right Leg around 90
        if (CurrentLegRotation < 150.0f)
        {
            RequiredRotation = CurrentLegRotation + 10.0f;
        }

    } // RotateClockwise
    // ==============================================
    public void RotateAntiClockwise()
    {
        float CurrentLegRotation = this.transform.localRotation.eulerAngles.z;

        // Right Leg around 90, 
        if (CurrentLegRotation > 30.0f)
        {
            RequiredRotation = CurrentLegRotation - 10.0f;
        }

    } // RotateAntiClockwise
    // ==============================================
    public void AssertLegRotation()
    {
        // Assert to maintain the Required Rotation
        this.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, RequiredRotation);

        LegRigidbody.angularVelocity = Vector3.zero;
    } // AssertLegRotation

    // ==================================================================================
    public void ResetLegMotion()
    {
        RequiredRotation = 90.0f;
        LegRigidbody.velocity = Vector3.zero;

    } // ResetLegMotion
    // =====================================================================================
}
