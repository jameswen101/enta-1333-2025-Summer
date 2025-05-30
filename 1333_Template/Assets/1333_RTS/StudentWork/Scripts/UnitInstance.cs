using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInstance : UnitBase
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    private PathFinder pathFinder;
    private List<GridNode> currentPath = new List<GridNode>();
    private int pathIndex = 0;
    private Vector3? targetWorldPosition = null;
    private bool isMoving = false;

    public bool IsMoving => isMoving;

    public List<GridNode> CurrentPath => currentPath;

    public void Initialize(PathFinder _pathfinder, UnitType _unitType)
    {
        pathFinder = _pathfinder;
        unitType = _unitType;
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if not moving or no path, do nothing
        if (!isMoving || currentPath == null || currentPath.Count == 0 || pathIndex >= currentPath.Count)
        {
            return;
        }

        Vector3 nextWaypoint = currentPath[pathIndex].WorldPosition;
        Vector3 direction = (nextWaypoint - transform.position).normalized;
        float step = moveSpeed + Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, direction, step);

        if (Vector3.Distance(transform.position, nextWaypoint) < 0.05f)
        {
            pathIndex++;
            if (pathIndex >= currentPath.Count)
            {
                isMoving = false;
            }

        }
    }

    public void SetTarget(Vector3 worldPosition)
    {
        targetWorldPosition = worldPosition;
        //currentPath = pathFinder.FindPath(transform.position, worldPosition, Width, Height); //currently error
        pathIndex = 0;
        isMoving = currentPath != null && currentPath.Count > 1;
    }

    public void SetTarget(GridNode node)
    {
        SetTarget(node.WorldPosition);
    }
    public override void MoveTo(GridNode targetNode)
    {
        SetTarget(targetNode);
    }


}
