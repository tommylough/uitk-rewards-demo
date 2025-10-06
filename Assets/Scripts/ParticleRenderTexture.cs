using UnityEngine;

[System.Serializable]
public class ParticleRenderTexture : MonoBehaviour
{
    [Header("Render Settings")]
    public RenderTexture renderTexture;
    public Camera particleCamera;
    public LayerMask particleLayer = 1 << 8; // Layer 8 for particles
    
    [Header("Auto Setup")]
    public bool autoCreateRenderTexture = true;
    public int textureWidth = 1024;
    public int textureHeight = 1024;
    
    void Awake()
    {
        SetupParticleCamera();
        if (autoCreateRenderTexture)
            CreateRenderTexture();
    }
    
    void SetupParticleCamera()
    {
        // If no camera assigned, create one automatically
        if (particleCamera == null)
        {
            GameObject cameraGO = new GameObject("ParticleCamera");
            cameraGO.transform.SetParent(transform);
            particleCamera = cameraGO.AddComponent<Camera>();
            
            // Auto-setup only if we created the camera
            particleCamera.cullingMask = particleLayer;
            particleCamera.clearFlags = CameraClearFlags.Color;
            particleCamera.backgroundColor = Color.clear;
            particleCamera.orthographic = true;
            particleCamera.orthographicSize = 5;
            particleCamera.depth = -10;
            
            // Move camera off-screen initially
            particleCamera.transform.position = new Vector3(-1000, -1000, 0);
        }
        else
        {
            // Manual camera assigned - just ensure it targets particles
            particleCamera.cullingMask = particleLayer;
        }
    }
    
    void CreateRenderTexture()
    {
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(textureWidth, textureHeight, 24, RenderTextureFormat.DefaultHDR);
            renderTexture.name = "ParticleEffectTexture";
            renderTexture.antiAliasing = 4; // Add anti-aliasing
        }
        
        particleCamera.targetTexture = renderTexture;
    }
    
    public void SetCameraPosition(Vector3 worldPosition)
    {
        particleCamera.transform.position = worldPosition;
    }
    
    public void SetCameraSize(float size)
    {
        particleCamera.orthographicSize = size;
    }
}