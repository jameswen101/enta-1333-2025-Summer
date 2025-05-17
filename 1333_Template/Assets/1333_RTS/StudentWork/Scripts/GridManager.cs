using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GridSettings gridSettings;
    public GridSettings GridSettings => gridSettings;

    private GridNode[,] gridNodes;

    public bool IsInitialized { get; private set; } = false;
    public void InitializeGrid()
    {
        gridNodes = new GridNode[gridSettings.GridSizeX, gridSettings.GridSizeY];
        for (int x = 0; x < gridSettings.GridSizeX; x++)
        {
            for (int y = 0; y < gridSettings.GridSizeY; y++)
            {
                Vector3 worldPos = gridSettings.UseXZPlane
                    ? new Vector3(x, 0, y) * gridSettings.NodeSize
                    : new Vector3(x, y, 0) * gridSettings.NodeSize;

                GridNode node = new GridNode
                {
                    name = $"Cell_{(x + gridSettings.GridSizeX) * x + y}",
                    WorldPosition = worldPos,
                    Walkable = true,
                    Weight = 1
                };
                gridNodes[x, y] = node;
            }
        }
        IsInitialized = true;

    }

    public GridNode GetNode(int x, int y);
    public void SetWalkable(int x, int y, bool walkable);
    private void OnDrawGizmos()
    {
        if (!showGrid || gridNodes == null || gridSettings == null) return;
        for (int x = 0; x < gridSettings.GridSizeX; x++)
        {
            for (int y = 0; y < gridSettings.GridSizeY; y++)
            {
                GridNode node = gridNodes[x,y];
                Gizmos.color = node.GizmoColor;
                Gizmos.DrawWireCube(node.WorldPosition, Vector3.one * gridSettings.NodeSize * 0.9f);
            }
        }
    }

    [CustomEditorForRenderPipeline(typeof(GridManager))]
    public class GridManagerEditor: Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();  
        }
    }


    private void DoSomethingOnEachNode(System.Action thingToDo)
    {
        for (int x = 0; x < gridSettings.GridSizeX; x++)
        {
            for(int y = 0;y < gridSettings.GridSizeY;y++)
            {
                thingToDo?.Invoke();
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
