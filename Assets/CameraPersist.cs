using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class CinemachineCameraPersist : MonoBehaviour
{
    private static CinemachineCameraPersist instance;
    private CinemachineVirtualCamera vcam;
    private Transform player;
    private Vector3 lastPlayerPosition;

    void Awake()
    {
        // Singleton: only one camera-persist object ever
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Find the VCAM in children
            vcam = GetComponentInChildren<CinemachineVirtualCamera>();
            if (vcam == null)
                Debug.LogError("[CinemachineCameraPersist] No CinemachineVirtualCamera found in children.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void LateUpdate()
    {
        // If we have a valid player, remember its current pos every frame
        if (player != null)
            lastPlayerPosition = player.position;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find the Player in the new scene by tag
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go != null && vcam != null)
        {
            var newPlayer = go.transform;

            // Compute how far the player has “teleported”
            // (lastPlayerPosition was from the old scene,
            // newPlayer.position is the spawn point in this scene)
            Vector3 delta = newPlayer.position - lastPlayerPosition;

            // Assign the camera’s targets
            vcam.Follow = newPlayer;
            vcam.LookAt = newPlayer;  // only if you’re using LookAt

            // Warp so Cinemachine doesn’t blend from world-0,0
            CinemachineCore.Instance.OnTargetObjectWarped(newPlayer, delta);

            // Update our reference
            player = newPlayer;
            // And set lastPlayerPosition so next LateUpdate records correctly
            lastPlayerPosition = newPlayer.position;
        }
        else if (vcam == null)
        {
            Debug.LogWarning("[CinemachineCameraPersist] vcam missing on scene load.");
        }
        else
        {
            Debug.LogWarning("[CinemachineCameraPersist] Player tag not found in scene: " + scene.name);
        }
    }
}
