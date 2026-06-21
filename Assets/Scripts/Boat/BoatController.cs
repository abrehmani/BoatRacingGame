using UnityEngine; 

public class BoatController : MonoBehaviour 
{ 
    [Header("Boat Movement Settings")] 
    public float moveSpeed = 10f; 
    public float turnSpeed = 50f; 
    public float drag = 0.5f; 
    public float angularDrag = 0.5f; 

    private Rigidbody rb; 
    private float currentMoveInput; 
    private float currentTurnInput; 

    void Awake() 
    { 
        rb = GetComponent<Rigidbody>(); 
        if (rb == null) 
        { 
            Debug.LogError("Rigidbody not found on BoatController. Please add a Rigidbody component."); 
            enabled = false; // Disable script if no Rigidbody 
        } 

        rb.drag = drag; 
        rb.angularDrag = angularDrag; 
    } 

    void Update() 
    { 
        // Input handling 
        currentMoveInput = Input.GetAxis("Vertical"); // W/S or Up/Down arrows 
        currentTurnInput = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows 
    } 

    void FixedUpdate() 
    { 
        ApplyMovement(); 
        ApplyTurning(); 
    } 

    void ApplyMovement() 
    { 
        Vector3 force = transform.forward * currentMoveInput * moveSpeed; 
        rb.AddForce(force, ForceMode.Acceleration); 
    } 

    void ApplyTurning() 
    { 
        float turn = currentTurnInput * turnSpeed * Time.fixedDeltaTime; 
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f); 
        rb.MoveRotation(rb.rotation * turnRotation); 
    } 

    // Optional: Add methods for power-ups, damage, etc. later 
}
