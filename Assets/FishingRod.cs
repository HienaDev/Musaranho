using UnityEngine;

public class FishingRod : MonoBehaviour
{
    public enum ForwardAxis { X, Y, Z, NegativeX, NegativeY, NegativeZ }

    [Header("Rod Setup")]
    public Transform tip;
    public Transform lure;

    [Header("Joint Settings")]
    public float spring = 30f;
    public float damper = 5f;
    public float maxDistance = 0.1f;

    [Header("Lure Rotation")]
    public ForwardAxis lureForwardAxis = ForwardAxis.Z;

    private SpringJoint joint;

    void Start()
    {
        if (tip == null || lure == null)
        {
            Debug.LogError("Tip or Lure not assigned!");
            return;
        }

        Rigidbody lureRb = lure.GetComponent<Rigidbody>();
        if (lureRb == null)
            lureRb = lure.gameObject.AddComponent<Rigidbody>();

        lureRb.useGravity = true;
        lureRb.interpolation = RigidbodyInterpolation.Interpolate;
        lureRb.linearDamping = 2f;
        lureRb.angularDamping = 4f;

        joint = lure.gameObject.AddComponent<SpringJoint>();
        joint.connectedBody = null;
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = Vector3.zero;
        joint.connectedAnchor = tip.position;

        joint.spring = spring;
        joint.damper = damper;
        joint.maxDistance = maxDistance;
    }

    void FixedUpdate()
    {
        if (joint != null && tip != null)
        {
            joint.connectedAnchor = tip.position;
        }

        // Make the lure face the rod tip
        Vector3 direction = tip.position - lure.position;
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            lure.rotation = lookRotation * GetAxisRotationOffset(lureForwardAxis);
        }
    }

    Quaternion GetAxisRotationOffset(ForwardAxis axis)
    {
        switch (axis)
        {
            case ForwardAxis.X: return Quaternion.Euler(0, -90, 0);
            case ForwardAxis.Y: return Quaternion.Euler(90, 0, 0);
            case ForwardAxis.NegativeX: return Quaternion.Euler(0, 90, 0);
            case ForwardAxis.NegativeY: return Quaternion.Euler(-90, 0, 0);
            case ForwardAxis.NegativeZ: return Quaternion.Euler(0, 180, 0);
            case ForwardAxis.Z:
            default: return Quaternion.identity;
        }
    }
}
