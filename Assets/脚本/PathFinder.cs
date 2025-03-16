using System;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    public static class PathFinder
    {
        public static List<Tile> FindPath_AStar(TileGrid grid, Tile start, Tile end)
        {
            if (start == null || end == null || start.Weight >= TileGrid.TileWeight_Wall || end.Weight >= TileGrid.TileWeight_Wall)
            {
                Debug.LogWarning("起点或终点不可达");
                return new List<Tile>();
            }

            foreach (Tile tile in grid.Tiles) tile.Cost = int.MaxValue;
            start.Cost = 0;

            Comparison<Tile> heuristicComparison = (a, b) =>
                (a.Cost + GetHeuristicCost(a, end)).CompareTo(b.Cost + GetHeuristicCost(b, end));

            MinHeap<Tile> frontier = new MinHeap<Tile>(heuristicComparison);
            frontier.Add(start);

            HashSet<Tile> visited = new HashSet<Tile>();
            visited.Add(start);
            start.PrevTile = null;

            while (frontier.Count > 0)
            {
                Tile current = frontier.Remove();
                if (current == end) break;

                foreach (Tile neighbor in GetTraversableNeighbors(grid, current))
                {
                    if (neighbor.Weight >= TileGrid.TileWeight_Wall) continue;

                    int newCost = current.Cost + neighbor.Weight;
                    if (newCost < neighbor.Cost)
                    {
                        neighbor.Cost = newCost;
                        neighbor.PrevTile = current;
                        if (!visited.Contains(neighbor))
                        {
                            frontier.Add(neighbor);
                            visited.Add(neighbor);
                        }
                    }
                }
            }

            List<Tile> path = BacktrackToPath(end);
            if (path.Count == 0) Debug.LogWarning("无法找到路径");
            return path;
        }

        private static IEnumerable<Tile> GetTraversableNeighbors(TileGrid grid, Tile tile)
        {
            foreach (Tile neighbor in grid.GetNeighbors(tile))
            {
                if (neighbor.Weight >= TileGrid.TileWeight_Wall) continue;
                yield return neighbor;
            }
        }

        private static float GetHeuristicCost(Tile current, Tile end)
        {
            Vector3 currentPos = current.ToWorldPosition();
            Vector3 endPos = end.ToWorldPosition();
            return Vector3.Distance(currentPos, endPos);
        }

        private static List<Tile> BacktrackToPath(Tile end)
        {
            List<Tile> path = new List<Tile>();
            Tile current = end;
            while (current != null)
            {
                path.Add(current);
                current = current.PrevTile;
            }
            path.Reverse();
            return path.Count > 1 ? path : new List<Tile>(); // 确保起点不等于终点
        }

        // ======================== 坐标转换接口 ========================
        public static List<Vector3> GetPathPoints(TileGrid grid, Vector3 startPos, Vector3 endPos)
        {
            Tile start = grid.GetTileFromWorldPos(startPos);
            Tile end = grid.GetTileFromWorldPos(endPos);
            
            if (start == null || end == null)
            {
                Debug.LogWarning("起始点或终点无效");
                return new List<Vector3>();
            }

            List<Tile> path = FindPath_AStar(grid, start, end);
            return path.ConvertAll(t => t.ToWorldPosition());
        }
    }
}