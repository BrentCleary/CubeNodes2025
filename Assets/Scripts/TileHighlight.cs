using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileHighlight : MonoBehaviour
{
  public  Material normalMaterial;     // Material 0 (blue)
  public  Material highlightMaterial;  // Material 1 (light blue)
  private MeshRenderer meshRenderer;
	private GameObject outline;


	// Start is called before the first frame update
	void Start()
  {
		outline = transform.Find("Outline").gameObject;
		meshRenderer = GetComponent<MeshRenderer>();
    
    // Ensure the default is set
    SetMaterial(normalMaterial);
	}

  private void SetMaterial(Material mat)
  {
    // If your mesh has only 1 submesh, replace element 0
    Material[] materials = meshRenderer.materials;
    materials[0] = mat;
    meshRenderer.materials = materials;
  }

  void OnMouseEnter()
  {
    outline.SetActive(true);
    SetMaterial(highlightMaterial);
	}

  void OnMouseExit()
  {
    outline.SetActive(false);
    SetMaterial(normalMaterial);
	}


}
