using UnityEngine;

public class TileMap : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;
    public int tileSize = 16;

    public Transform tilesHolder;

    public string SortingLayer = "bg";
    public int SortingOrderLayer = 0;

    public float tileSizeFloat { get { return tileSize / 100.0f; } }

    [HideInInspector]
    public Vector3 mousePos;


    public Texture2D sprites;

    public int gridSize;


    void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;

        Vector3 oriPos = transform.position;

        float lineHeight = mapHeight * tileSizeFloat;
        for (int x = 0; x <= mapWidth; x++)
        {
            float posX = x * tileSizeFloat;
            Gizmos.DrawLine(oriPos + Vector3.right * posX, oriPos + new Vector3(posX, lineHeight));
        }

        float lineWidth = mapWidth * tileSizeFloat;
        for (int y = 0; y <= mapHeight; y++)
        {
            float posY = y * tileSizeFloat;
            Gizmos.DrawLine(oriPos + Vector3.up * posY, oriPos + new Vector3(lineWidth, posY));
        }
    }
}

