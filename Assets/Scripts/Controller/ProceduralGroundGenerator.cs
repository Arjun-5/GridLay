using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ProceduralGroundGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] GameObject tilePrefab;
    
    [SerializeField] BoxCollider boundsCollider;
    
    [SerializeField] int minRows = 2, minCols = 2;

    [Header("Post-Generation Logic")]
    [SerializeField] GridPostProcessSO[] postGridBehaviors;

    [SerializeField] Transform instantiateTransform;

    private readonly List<GameObject> spawnedTiles = new();

    private void Start()
    {
        ValidateInput();
        
        GenerateTiles();
    }

    private void ValidateInput()
    {
        Assert.IsNotNull(tilePrefab, "The prefab to instantiate is null. Please check the input.");
        
        Assert.IsNotNull(boundsCollider, "The boundsCollider that determines the instantiation area is not assigned. Please check the input.");
        
        Assert.IsFalse(minRows < 2 || minCols < 2, "The value for Min Rows and Min Cols should be a minimum of 2.");
    }

    public void GenerateTiles()
    {
        spawnedTiles.Clear();

        Bounds bounds = boundsCollider.bounds;

        Vector2 areaSize = bounds.size;
        
        Vector2 tileSize = tilePrefab.transform.localScale;

        int maxCols = Mathf.FloorToInt(areaSize.x / tileSize.x);
        
        int maxRows = Mathf.FloorToInt(areaSize.y / tileSize.y);

        int cols = Mathf.Max(minCols, Random.Range(minCols, maxCols + 1));
        
        int rows = Mathf.Max(minRows, Random.Range(minRows, maxRows + 1));

        float usedWidth = cols * tileSize.x;
        
        float usedHeight = rows * tileSize.y;

        Vector2 startPos = new Vector2(
            bounds.min.x + (areaSize.x - usedWidth) / 2f + tileSize.x / 2f,
            bounds.min.y + (areaSize.y - usedHeight) / 2f + tileSize.y / 2f
        );

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Vector2 spawnPos = new Vector2(
                    startPos.x + col * tileSize.x,
                    startPos.y + row * tileSize.y
                );

                GameObject tile = Instantiate(tilePrefab, spawnPos, tilePrefab.transform.rotation, boundsCollider.transform);
                
                spawnedTiles.Add(tile);
            }
        }

        foreach (var behavior in postGridBehaviors)
        {
            behavior.OnGridInstantiationCompleted(spawnedTiles, instantiateTransform);
        }

        Debug.Log($"Generated Grid: {cols} cols × {rows} rows inside bounds.");
    }
}