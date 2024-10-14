using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Target
{
    public GameObject lineRendererObjectB; 
    public GameObject lineRendererObjectL; 
    public LineRenderer laserLineRenderer;
    public LineRenderer boxLineRenderer;
    public GameObject targetAsteroid;
    public BoxCollider boxCollider;

    public GameObject player;
}

public class LineRendererScript : MonoBehaviour
{
    private HashSet<Target> _targetObjects;
    
    public HashSet<Target> TargetObjects
    {
        get { return _targetObjects; }
    }
    
    Color c1 = new Color(0, 1, 65.0f/255.0f, 0.05f);
    Color c2 = new Color(0, 1, 65.0f/255.0f, 0.7f);

    private int[] _edgeIndices;
    private Vector3[] _boxPoints;

    public Action onChange;
    private void Awake()
    {

        _targetObjects = new HashSet<Target>();
        _edgeIndices = new int[16]
        {
            0, 1, 2, 3, 0, 4, 5, 1, 5, 6, 2, 6, 7, 3, 7, 4
        };
        
        _boxPoints = new Vector3[8]
        {
            new Vector3(-1, -1, -1), // Bottom-back-left 0
            new Vector3(1, -1, -1),  // Bottom-back-right 1
            new Vector3(1, -1, 1),   // Bottom-front-right 2
            new Vector3(-1, -1, 1),  // Bottom-front-left 3
            new Vector3(-1, 1, -1),  // Top-back-left 4
            new Vector3(1, 1, -1),   // Top-back-right 5 
            new Vector3(1, 1, 1),    // Top-front-right 6
            new Vector3(-1, 1, 1)    // Top-front-left 7
        };

    }

    private void Update()
    {
        if(_targetObjects.Count != 0) DrawAll();
    }

    public void addWireCube(GameObject Asteroid, GameObject player)
    {
        Debug.Log("adding wire cube");
        if (Asteroid == null)
        {
            Debug.LogError("Asteroid parameter is null!");
            return;
        }

        Target obj = new Target();
        
        obj.lineRendererObjectB = new GameObject("LineRendererObject_b");
        obj.lineRendererObjectL = new GameObject("LineRendererObject_l");
    
        obj.boxLineRenderer = obj.lineRendererObjectB.AddComponent<LineRenderer>();
        obj.lineRendererObjectB.transform.parent = this.transform;
        obj.laserLineRenderer = obj.lineRendererObjectL.AddComponent<LineRenderer>();
        obj.lineRendererObjectL.transform.parent = this.transform;
        obj.targetAsteroid = Asteroid;
        obj.boxCollider = Asteroid.GetComponent<BoxCollider>();

        obj.player = player;
        

        // Configure boxLineRenderer
        obj.boxLineRenderer.positionCount = 16;
        obj.boxLineRenderer.startWidth = 0.05f;
        obj.boxLineRenderer.endWidth = 0.05f;
        obj.boxLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        obj.boxLineRenderer.SetColors(c2, c2);

        obj.laserLineRenderer.positionCount = 2;
        obj.laserLineRenderer.startWidth = 0.05f;
        obj.laserLineRenderer.endWidth = 0.05f;
        obj.laserLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        obj.laserLineRenderer.SetColors(c2, c1);

        _targetObjects.Add(obj);
        
        onChange?.Invoke();
    }


    public void removeWireCube(GameObject Asteroid)
    {
        foreach (Target t in _targetObjects)
        {
            if (t.targetAsteroid == Asteroid) 
            {
                Destroy(t.lineRendererObjectB);
                Destroy(t.lineRendererObjectL);
                _targetObjects.Remove(t);
                break;
            }
        }
        onChange?.Invoke();
    }


    private void DrawAll()
    {
        foreach (Target obj in _targetObjects)
        {
            if(obj == null) continue;
            Vector3 center = obj.targetAsteroid.transform.position + obj.boxCollider.center;
            Vector3 size = obj.targetAsteroid.GetComponent<MeshRenderer>().bounds.size / 3;

            for (int i = 0; i < _edgeIndices.Length; i++)
            {
                obj.boxLineRenderer.SetPosition(i, center + Vector3.Scale(size, _boxPoints[_edgeIndices[i]]));
            }

            obj.laserLineRenderer.SetPosition(0, center);
            obj.laserLineRenderer.SetPosition(1, new Vector3(obj.player.transform.position.x-1.5f, obj.player.transform.position.y + 0.5f , obj.player.transform.position.z));
        }
    }
}
