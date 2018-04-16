using UnityEngine;

public class ParticleRenderQueue : MonoBehaviour
{
    public int renderQueue;
    public bool update;
    // Use this for initialization
    void Start()
    {
        Renderer[] renders = GetComponentsInChildren<Renderer>();
        foreach (Renderer render in renders)
        {
            render.material.renderQueue = renderQueue;
        }
    }

    public void Update()
    {
        if (update)
        {
            if (Time.frameCount % 6 == 0)
            {
                Start();
            } 
        }
    }
}
