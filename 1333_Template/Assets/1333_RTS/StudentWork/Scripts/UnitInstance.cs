using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UnitInstance : UnitBase
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Prefab Stuff")]
    [SerializeField] private Animator charAnimator;
    [SerializeField] private SkinnedMeshRenderer unitSkin;
    [SerializeField] private GameObject headLocator;
    [SerializeField] private ParticleSystem hurtParticles;
    [SerializeField] private Transform animationParent;

    private GameObject animatedUnit;
    [SerializeField] private PathFinder pathFinder;
    [SerializeField] private AStarPathfinder aStarPathfinder;
    [SerializeField] private GridManager gridManager;
    private Animator characterAnimator;
    private List<GridNode> currentPath = new List<GridNode>();
    private int pathIndex = 0;
    private Vector3? targetWorldPosition = null;
    private bool isMoving = false;

    public bool IsMoving => isMoving;

    public List<GridNode> CurrentPath => currentPath;

    public void Initialize(PathFinder _pathfinder, UnitType _unitType, GridManager _gridManager, AStarPathfinder _aStarPathfinder)
    {
        pathFinder = _pathfinder;
        gridManager = _gridManager;
        aStarPathfinder = _aStarPathfinder;
        unitType = _unitType;

        animatedUnit = Instantiate(unitType.unitPrefab, animationParent);
        charAnimator = animationParent.GetComponent<Animator>();
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
        transform.position = Vector3.MoveTowards(transform.position, nextWaypoint, step);

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
        Debug.Log($"[SetTarget] Generated path with {currentPath?.Count ?? 0} nodes");
        targetWorldPosition = worldPosition;
        currentPath = aStarPathfinder.FindPath(gridManager, transform.position, worldPosition, Width, Height);

        Debug.Log($"Path generated with {currentPath?.Count ?? 0} nodes");

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

    private void OnDrawGizmos()
    {
        if (currentPath != null)
            Debug.Log($"Path node count: {currentPath.Count}");

        Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(transform.position, transform.position + Vector3.forward * 2);

        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Gizmos.DrawLine(currentPath[i].WorldPosition, currentPath[i + 1].WorldPosition);
        }

        //        if (!Application.isPlaying || currentPath == null || currentPath.Count < 2)
        //            return;

        //        Gizmos.color = Color.cyan;

        //        for (int i = 0; i < currentPath.Count - 1; i++)
        //        {
        //            Gizmos.DrawLine(currentPath[i].WorldPosition, currentPath[i + 1].WorldPosition);
        //#if UNITY_EDITOR
        //            Debug.Log($"Node {i}: {currentPath[i].WorldPosition} | gCost: {currentPath[i].gCost}, hCost: {currentPath[i].hCost}, fCost: {currentPath[i].fCost}");
        //#endif
        //        }
    }


}
