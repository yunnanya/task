using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PathFinding
{
    public class TileGrid : MonoBehaviour
    {
        private const int TileWeight_Default = 1;
        private const int TileWeight_Expensive = 50;
        private const int TileWeight_Infinity = int.MaxValue;

        public int Rows;
        public int Cols;
        public GameObject TilePrefab;
        public GameObject character;

        public Color TileColor_Default = new Color(0.86f, 0.83f, 0.83f);
        public Color TileColor_Expensive = new Color(0.19f, 0.65f, 0.43f);
        public Color TileColor_Infinity = new Color(0.37f, 0.37f, 0.37f);
        public Color TileColor_Start = Color.green;
        public Color TileColor_End = Color.red;
        public Color TileColor_Path = new Color(0.73f, 0.0f, 1.0f);
        public Color TileColor_Visited = new Color(0.75f, 0.55f, 0.38f);
        public Color TileColor_Frontier = new Color(0.4f, 0.53f, 0.8f);

        public Tile[] Tiles { get; private set; }

        private IEnumerator _pathRoutine;

        private void Awake()
        {
            Tiles = new Tile[Rows * Cols];
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    Tile tile = new Tile(this, r, c, TileWeight_Default);
                    tile.InitGameObject(transform, TilePrefab);

                    int index = GetTileIndex(r, c);
                    Tiles[index] = tile;
                }
            }


            CreateExpensiveArea(3, 3, 9, 1, TileWeight_Expensive);
            CreateExpensiveArea(3, 11, 1, 9, TileWeight_Expensive);
            ResetGrid();
        }

        private void Update()
        {
            Tile start = GetTile(9, 2);
            Tile end = GetTile(7, 14);

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                MoveCharacterToStart(start);
                StopPathCoroutine();
                _pathRoutine = FindPath(start, end, PathFinder.FindPath_BFS);
                StartCoroutine(_pathRoutine);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                MoveCharacterToStart(start);
                StopPathCoroutine();
                _pathRoutine = FindPath(start, end, PathFinder.FindPath_Dijkstra);
                StartCoroutine(_pathRoutine);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                MoveCharacterToStart(start);
                StopPathCoroutine();
                _pathRoutine = FindPath(start, end, PathFinder.FindPath_AStar);
                StartCoroutine(_pathRoutine);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                MoveCharacterToStart(start);
                StopPathCoroutine();
                _pathRoutine = FindPath(start, end, PathFinder.FindPath_GreedyBestFirstSearch);
                StartCoroutine(_pathRoutine);
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopPathCoroutine();
                ResetGrid();
                start.SetColor(TileColor_Start);
                end.SetColor(TileColor_End);
            }
        }

        private void MoveCharacterToStart(Tile start)
        {
            if (character != null && start != null)
            {
                Vector3 startPosition = start.ToWorldPosition();
                character.transform.position = startPosition; // 将3D模型移动到起点
            }
        }

        private void StopPathCoroutine()
        {
            if (_pathRoutine != null)
            {
                StopCoroutine(_pathRoutine);
                _pathRoutine = null;
            }
        }

        private void CreateExpensiveArea(int row, int col, int width, int height, int weight)
        {
            for (int r = row; r < row + height; r++)
            {
                for (int c = col; c < col + width; c++)
                {
                    Tile tile = GetTile(r, c);
                    if (tile != null)
                    {
                        tile.Weight = weight;
                    }
                }
            }
        }

        private void ResetGrid()
        {
            foreach (var tile in Tiles)
            {
                tile.Cost = 0;
                tile.PrevTile = null;
                tile.SetText("");

                switch (tile.Weight)
                {
                    case TileWeight_Default:
                        tile.SetColor(TileColor_Default);
                        break;
                    case TileWeight_Expensive:
                        tile.SetColor(TileColor_Expensive);
                        break;
                    case TileWeight_Infinity:
                        tile.SetColor(TileColor_Infinity);
                        break;
                }
            }
        }

        private IEnumerator FindPath(Tile start, Tile end, Func<TileGrid, Tile, Tile, List<IVisualStep>, List<Tile>> pathFindingFunc)
        {
            ResetGrid();

            List<IVisualStep> steps = new List<IVisualStep>();
            List<Tile> path = pathFindingFunc(this, start, end, steps);
            

            // 新增：将路径传递给行人模型
            CharacterMovement character = FindObjectOfType<CharacterMovement>();
            if (character != null)
            {
                character.SetPath(path);
            }

            foreach (var step in steps)
            {
                step.Execute();
                yield return new WaitForFixedUpdate();
            }
        }

        public Tile GetTile(int row, int col)
        {
            if (!IsInBounds(row, col))
            {
                return null;
            }

            return Tiles[GetTileIndex(row, col)];
        }

        public IEnumerable<Tile> GetNeighbors(Tile tile)
        {
            // 东（右）
            Tile east = GetTile(tile.Row, tile.Col + 1);
            if (east != null) yield return east;

            // 北（上）
            Tile north = GetTile(tile.Row - 1, tile.Col);
            if (north != null) yield return north;

            // 西（左）
            Tile west = GetTile(tile.Row, tile.Col - 1);
            if (west != null) yield return west;

            // 南（下）
            Tile south = GetTile(tile.Row + 1, tile.Col);
            if (south != null) yield return south;
        }

        private bool IsInBounds(int row, int col)
        {
            bool rowInRange = row >= 0 && row < Rows;
            bool colInRange = col >= 0 && col < Cols;
            return rowInRange && colInRange;
        }

        private int GetTileIndex(int row, int col)
        {
            return row * Cols + col;
        }
    }
}
