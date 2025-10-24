using Neocortex;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [Header("Grid Settings")]
    public int rows = 10;
    public int columns = 10;
    public float cellSize = 2f;
    public GameObject cellPrefab;

    private Dictionary<string, Vector3> gridPositions = new Dictionary<string, Vector3>();
    private Dictionary<string, GameObject> gridCells = new Dictionary<string, GameObject>();

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                string gridId = GetGridId(row, col);
                char columnLetter = (char)('A' + col);
                int rowNumber = row;

                Vector3 position = new Vector3(col * cellSize, 0, row * cellSize);
                gridPositions[gridId] = position;

                if (cellPrefab != null)
                {
                    GameObject cell = Instantiate(cellPrefab, position, Quaternion.identity, transform);
                    cell.name = gridId;

                    NeocortexGridObject interactable = cell.GetComponent<NeocortexGridObject>();
                    if (interactable == null)
                    {
                        interactable = cell.AddComponent<NeocortexGridObject>();
                    }

                    interactable.ToInteractable(gridId,columnLetter.ToString(),rowNumber, false);    

                    gridCells[gridId] = cell;

                    Debug.Log($"Created grid cell {gridId} with column={columnLetter}, number={rowNumber}");
                }
            }
        }
    }

    string GetGridId(int row, int col)
    {
        char letter = (char)('A' + col);
        return $"{letter}{row:D2}";
    }

    public bool GetGridPosition(string gridId, out Vector3 position)
    {
        gridId = gridId.ToUpper().Replace(" ", "");
        return gridPositions.TryGetValue(gridId, out position);
    }

   

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 pos = new Vector3(col * cellSize, 0, row * cellSize);
                Gizmos.DrawWireCube(pos, new Vector3(cellSize * 0.9f, 0.1f, cellSize * 0.9f));
            }
        }
    }
}
