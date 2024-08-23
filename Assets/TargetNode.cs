using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetNode : MonoBehaviour
{
    private GameObject parentNode;
    public Material selectionMaterial;
    private List<Renderer> tileRendererList;
    public List<Material> tileMaterialList;

    // Start is called before the first frame update
    void Start()
    {
        parentNode = gameObject.transform.parent.gameObject;

        tileRendererList = parentNode.GetComponentsInChildren<Renderer>().ToList();
        
        foreach(Renderer renderer in tileRendererList)
        {
            tileMaterialList.Add(renderer.material);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        foreach(Renderer renderer in tileRendererList)
        {
            renderer.material = selectionMaterial;
        }
        Debug.Log("OnMouseEnter Activated on " + gameObject.name);
        
        // Material Debuggeer for checking material display - Disabled 08222024.2059 
        // foreach(Material material in tileMaterialList)
        // {
        //     Debug.Log($"{material}");
        // }
    }

    private void OnMouseExit()
    {
        int colorCounter = 0;
        foreach(Renderer renderer in tileRendererList)
        {
            renderer.material = tileMaterialList[colorCounter];
            colorCounter++;
        }
    }
}
