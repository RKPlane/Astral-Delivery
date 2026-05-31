using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class DockingSystem : MonoBehaviour
{
    [Header("Deteccion")]
    public float dockingRange = 4f;
    public LayerMask junkLayer;

    [Header("Lanzamiento")]
    public float launchSpeed = 6f;

    [Header("Input")]
    public InputActionAsset inputActions;
    private InputAction dockAction;
    private InputAction releaseAction;
    private InputAction launchAction;

    [Header("Feedback")]
    public TMPro.TextMeshProUGUI promptText;

    private PlayerController player;
    private MassSystem massSystem;

    private List<DockedItem> dockedItems = new List<DockedItem>();
    private JunkObject nearestJunk;

    private struct DockedItem
    {
        public JunkObject junk;
        public FixedJoint joint;
    }

    void Awake()
    {
        player = GetComponent<PlayerController>();
        massSystem = GetComponent<MassSystem>();

        var playerMap = inputActions.FindActionMap("Player");
        dockAction = playerMap.FindAction("Dock");
        releaseAction = playerMap.FindAction("Release");
        launchAction = playerMap.FindAction("Launch");
    }

    void OnEnable()
    {
        dockAction.Enable();
        releaseAction.Enable();
        launchAction.Enable();
    }

    void OnDisable()
    {
        dockAction.Disable();
        releaseAction.Disable();
        launchAction.Disable();
    }

    void Update()
    {
        ScanForNearbyJunk();
        HandleInput();
    }

    void ScanForNearbyJunk()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, dockingRange, junkLayer);
        float best = Mathf.Infinity;
        JunkObject found = null;

        foreach (var hit in hits)
        {
            var j = hit.GetComponent<JunkObject>();
            if (j == null || j.IsDocked || j.IsCollected) continue;
            float d = Vector3.Distance(transform.position, j.transform.position);
            if (d < best) { best = d; found = j; }
        }

        nearestJunk = found;

        if (promptText != null)
            promptText.text = nearestJunk != null
                ? $"[ E ]  ACOPLAR  -  {nearestJunk.JunkData.label}  ({nearestJunk.JunkData.mass} kg)"
                : string.Empty;
    }

    void HandleInput()
    {
        if (dockAction.WasPressedThisFrame()) TryDock();
        if (releaseAction.WasPressedThisFrame()) TryRelease(launch: false);
        if (launchAction.WasPressedThisFrame()) TryRelease(launch: true);
    }

    void TryDock()
    {
        if (nearestJunk == null) return;

        Rigidbody rbPlayer = player.GetComponent<Rigidbody>();
        Rigidbody rbJunk = nearestJunk.GetComponent<Rigidbody>();

        //FIX
        float m1 = rbPlayer.mass;
        float m2 = rbJunk.mass;
        Vector3 vFinal = (m1 * rbPlayer.linearVelocity + m2 * rbJunk.linearVelocity) / (m1 + m2);

        rbPlayer.linearVelocity = vFinal;
        rbJunk.linearVelocity = vFinal;
        rbJunk.angularVelocity = Vector3.zero;

        //FIX
        //rbJunk.constraints = RigidbodyConstraints.FreezeRotation;

        FixedJoint joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = rbJunk;
        joint.breakForce = Mathf.Infinity;
        joint.breakTorque = Mathf.Infinity;

        dockedItems.Add(new DockedItem { junk = nearestJunk, joint = joint });
        nearestJunk.SetDocked(true);
        massSystem.AddMass(nearestJunk.JunkData.mass);

        nearestJunk = null;
        if (promptText != null) promptText.text = string.Empty;
    }

    void TryRelease(bool launch)
    {
        if (dockedItems.Count == 0) return;

        DockedItem item = dockedItems[^1];
        dockedItems.RemoveAt(dockedItems.Count - 1);
        Destroy(item.joint);

        Rigidbody rbJunk = item.junk.GetComponent<Rigidbody>();

        //fix
        rbJunk.constraints = RigidbodyConstraints.None;

        item.junk.SetDocked(false);

        if (launch)
        {
            float actualSpeed = launchSpeed * (player.Mass / (player.Mass + item.junk.JunkData.mass));
            rbJunk.linearVelocity = transform.forward * actualSpeed;
            player.ApplyReactionImpulse(item.junk.JunkData.mass, transform.forward, actualSpeed);
        }
        else
        {
            rbJunk.linearVelocity = player.Velocity;
            //item.junk.SetCollected(true);
        }

        massSystem.RemoveMass(item.junk.JunkData.mass);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, dockingRange);
    }

    public int DockedCount => dockedItems.Count;
    public float TotalDockedMass
    {
        get { float t = 0f; foreach (var d in dockedItems) t += d.junk.JunkData.mass; return t; }
    }

    public void DeliverAll()
    {
        while (dockedItems.Count > 0)
        {
            DockedItem item = dockedItems[^1];
            dockedItems.RemoveAt(dockedItems.Count - 1);
            Destroy(item.joint);

            Rigidbody rbJunk = item.junk.GetComponent<Rigidbody>();
            rbJunk.constraints = RigidbodyConstraints.None;

            item.junk.SetDocked(false);
            item.junk.SetCollected(true);
            massSystem.RemoveMass(item.junk.JunkData.mass);
        }
    } 
}
