using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.AI.Navigation;
public class NavMeshBaker : MonoBehaviour
{
    public NavMeshSurface surface;

    // Start is called before the first frame update

    private void OnEnable()
    {
        ActionController.OnUpdateNavmesh+=ReBakeNavmesh;
    }
    private void OnDisable()
    {
        ActionController.OnUpdateNavmesh-=ReBakeNavmesh;
    }
    void Start()
    {
        StartCoroutine(BuildPTMNavMesh());
    }
    private void Update() {
        
    }

    void ReBakeNavmesh()
    {
        
            BakeImmediately();
        
    }
    IEnumerator BuildPTMNavMesh()
    {
        yield return new WaitForSeconds(0.3f);
        surface.BuildNavMesh();
 
    }
    void BakeImmediately()
    {
        surface.BuildNavMesh();
    }

}
