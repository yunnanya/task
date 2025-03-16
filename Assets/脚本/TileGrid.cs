using UnityEngine;
using System.Collections.Generic;

namespace PathFinding
{
    public class TileGrid : MonoBehaviour
    {
        public const int TileWeight_Default = 1;
        public const int TileWeight_Expensive = 50;
        public const int TileWeight_Wall = 100000;

        [Header("Grid Settings")]
        public int Rows = 50;
        public int Cols = 50;
        public GameObject TilePrefab;

        [Header("Tile Colors")]
        public Color TileColor_Default = new Color(0.86f, 0.83f, 0.83f);
        public Color TileColor_Expensive = new Color(0.19f, 0.65f, 0.43f);
        public Color TileColor_Wall = Color.black;

        public Tile[] Tiles { get; private set; }

        private void Awake()
        {
            Tiles = new Tile[Rows * Cols];
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    Tile tile = new Tile(this, r, c, TileWeight_Default);
                    tile.InitGameObject(transform, TilePrefab);
                    Tiles[GetTileIndex(r, c)] = tile;
                }
            }

            GenerateWallLine(10, 5, 20, true);
            GenerateWallLine(5, 15, 15, false);
            GenerateWallLine(30, 30, 10, true);

            ResetGrid();
        }

        public void GenerateWallLine(int startRow, int startCol, int length, bool isHorizontal)
        {
            for (int i = 0; i < length; i++)
            {
                int row = isHorizontal ? startRow : startRow + i;
                int col = isHorizontal ? startCol + i : startCol;

                Tile tile = GetTile(row, col);
                if (tile != null)
                {
                    tile.Weight = TileWeight_Wall;
                    tile.SetColor(TileColor_Wall);
                }
            }
        }

        private void ResetGrid()
        {
            foreach (var tile in Tiles)
            {
                tile.Cost = 0;
                tile.PrevTile = null;
                if (tile.Weight == TileWeight_Wall)
                {
                    tile.SetColor(TileColor_Wall);
                }
                else
                {
                    tile.SetColor(TileColor_Default);
                }
            }
        }

        public Tile GetTile(int row, int col)
        {
            return IsInBounds(row, col) ? Tiles[GetTileIndex(row, col)] : null;
        }

        public Tile GetTileFromWorldPos(Vector3 worldPos)
        {
            // 使用Mathf.RoundToInt确保坐标精确转换
            int row = Mathf.RoundToInt(worldPos.z - 0.5f);
            int col = Mathf.RoundToInt(worldPos.x - 0.5f);
            return GetTile(row, col);
        }

        private bool IsInBounds(int row, int col)
        {
            return row >= 0 && row < Rows && col >= 0 && col < Cols;
        }

        private int GetTileIndex(int row, int col)
        {
            return row * Cols + col;
        }

        public IEnumerable<Tile> GetNeighbors(Tile tile)
        {
            Tile east = GetTile(tile.Row, tile.Col + 1);
            if (east != null) yield return east;

            Tile north = GetTile(tile.Row - 1, tile.Col);
            if (north != null) yield return north;

            Tile west = GetTile(tile.Row, tile.Col - 1);
            if (west != null) yield return west;

            Tile south = GetTile(tile.Row + 1, tile.Col);
            if (south != null) yield return south;
        }

    }
}