using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionManager : MonoBehaviour
{
    public GridManager gridManager;
    /*
    private void TrySelectUnit()
    {
        Ray ray = Camera.ScreenPointToRay(Mouse.current.position.ReadValue());

    }

    private void TryCommandMove()
    {
        if (selectedUnit == null) return;
        Ray ray = Camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane groundPlane = new Plane (Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            GridNode targetNode = GridManager.
        }
    }
    */
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
