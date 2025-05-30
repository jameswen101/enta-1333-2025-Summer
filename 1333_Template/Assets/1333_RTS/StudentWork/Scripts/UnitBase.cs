using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    [SerializeField] protected UnitType unitType;
    public int Width => unitType != null ? unitType.width : 1;
    public int Height => unitType != null ? unitType.height : 1;
    public abstract void MoveTo(GridNode targetNode);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
