using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class DataForParticles : MonoBehaviour
{
    public RenderMesh renderMesh;
    public bool ready = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!ready)
        {
            TrailSpawnSystem.renderMesh = renderMesh;
            TrailSpawnSystem.enabled = true;
            ready = true;
        }
    }
}
