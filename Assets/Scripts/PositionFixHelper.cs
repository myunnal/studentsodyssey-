using UnityEngine;
using UnityEngine.SceneManagement;

public class PositionFixHelper : MonoBehaviour
{
    [Header("Ground Check")]
    public float groundCheckDistance = 1.0f;
    public LayerMask groundLayer;
    
    [Header("Position Fix Settings")]
    public bool fixPositionAfterSceneLoad = true;
    public float groundFixOffset = 0.1f; // Small offset above ground
    
    private Rigidbody2D rb;
    private Collider2D col;
    private PlayerController playerController;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        playerController = GetComponent<PlayerController>();
        
        // Register for scene loaded events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LevelPicker" || scene.name == "Meniu" || scene.name == "CollectablesMenu")
        {
            return; // Don't fix position in menu scenes
        }
        
        // Delay the fix to ensure all scene objects are loaded
        Invoke("FixPlayerPositionAfterSceneLoad", 0.1f);
    }
    
    void FixPlayerPositionAfterSceneLoad()
    {
        if (!fixPositionAfterSceneLoad) return;
        
        if (rb != null)
        {
            // Reset vertical velocity
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
        
        // Check if player is in the air
        if (!IsGrounded())
        {
            SnapToGround();
        }
    }
    
    bool IsGrounded()
    {
        if (col == null) return false;
        
        // Get the bounds of the collider
        Bounds bounds = col.bounds;
        
        // Calculate the start position for the raycast (center bottom of collider)
        Vector2 center = new Vector2(bounds.center.x, bounds.min.y);
        
        // Cast a ray downward
        RaycastHit2D hit = Physics2D.Raycast(center, Vector2.down, groundCheckDistance, groundLayer);
        
        // Debug ray to see where we're checking
        Debug.DrawRay(center, Vector2.down * groundCheckDistance, hit ? Color.green : Color.red, 1.0f);
        
        return hit.collider != null;
    }
    
    void SnapToGround()
    {
        if (col == null) return;
        
        // Get the bounds of the collider
        Bounds bounds = col.bounds;
        
        // Calculate the start position for the raycast (center bottom of collider)
        Vector2 center = new Vector2(bounds.center.x, bounds.min.y);
        
        // Cast a ray downward with extended distance to find ground
        RaycastHit2D hit = Physics2D.Raycast(center, Vector2.down, 10f, groundLayer);
        
        if (hit.collider != null)
        {
            // Calculate new position
            float newY = hit.point.y + (bounds.size.y / 2) + groundFixOffset;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            
            Debug.Log("Snapped player to ground at " + transform.position);
            
            // Make sure the player is considered grounded
            if (playerController != null)
            {
                // Force grounded state (if your PlayerController has this)
                // You might need to adapt this part to match your PlayerController's implementation
            }
            
            // Ensure velocity is zeroed out
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
            }
        }
        else
        {
            Debug.LogWarning("Could not find ground below player!");
        }
    }
    
    // Call this method from other scripts if needed
    public void ForceGroundSnap()
    {
        SnapToGround();
    }
}