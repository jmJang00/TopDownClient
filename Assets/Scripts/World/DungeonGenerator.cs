using System;
using UnityEngine;

class DungeonGenerator : MonoBehaviour
{
    public int width;
    public int height;
    public int totalRoom;
    public bool showGizmos = true;

    const int EMPTY = 0;
    const int FLOOR = 1;

    struct Room
    {
        public int x, y, w, h;
        public int CenterX() { return x + w / 2; }
        public int CenterY() { return y + h / 2; }
    }

    public int[,] Map { get { return map; } set { map = value; } }

    int[,] map = null;
    Room[] rooms = new Room[64];
    int roomCount = 0;

    // xorshift
    public uint seed = 123456789;

    uint Rand()
    {
        seed ^= seed << 13;
        seed ^= seed >> 17;
        seed ^= seed << 5;
        return seed;
    }

    int RandRange(int min, int max)
    {
        return (int)(Rand() % (uint)(max - min)) + min;
    }

    bool Intersect(Room a, Room b)
    {
        return !(a.x + a.w < b.x || b.x + b.w < a.x ||
                 a.y + a.h < b.y || b.y + b.h < a.y);
    }

    void AddRoom()
    {
        for (int i = 0; i < totalRoom; i++)
        {
            Room r;
            r.w = RandRange(4, 10);
            r.h = RandRange(4, 8);
            r.x = RandRange(1, width - r.w - 1);
            r.y = RandRange(1, height - r.h - 1);

            bool ok = true;
            for (int j = 0; j < roomCount; j++)
            {
                if (Intersect(r, rooms[j]))
                {
                    ok = false;
                    break;
                }
            }

            if (!ok) continue;

            if (roomCount < totalRoom)
            {
                rooms[roomCount++] = r;
            }
            else
            {
                return;
            }

            // carve
            for (int y = r.y; y < r.y + r.h; y++)
                for (int x = r.x; x < r.x + r.w; x++)
                    map[y, x] = FLOOR;

            return;
        }
    }

    void DigLine(int x1, int y1, int x2, int y2)
    {
        int x = x1;
        while (x != x2)
        {
            map[y1, x] = FLOOR;
            x += (x2 > x1) ? 1 : -1;
        }

        int y = y1;
        while (y != y2)
        {
            map[y, x2] = FLOOR;
            y += (y2 > y1) ? 1 : -1;
        }
    }

    // Prim MST (간단 구현)
    void ConnectRooms()
    {
        bool[] visited = new bool[roomCount];
        visited[0] = true;

        for (int i = 0; i < roomCount - 1; i++)
        {
            int bestA = -1, bestB = -1;
            int bestDist = int.MaxValue;

            for (int a = 0; a < roomCount; a++)
            {
                if (!visited[a]) continue;

                for (int b = 0; b < roomCount; b++)
                {
                    if (visited[b]) continue;

                    int dx = rooms[a].CenterX() - rooms[b].CenterX();
                    int dy = rooms[a].CenterY() - rooms[b].CenterY();
                    int dist = dx * dx + dy * dy;

                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        bestA = a;
                        bestB = b;
                    }
                }
            }

            visited[bestB] = true;

            DigLine(
                rooms[bestA].CenterX(), rooms[bestA].CenterY(),
                rooms[bestB].CenterX(), rooms[bestB].CenterY()
            );
        }
    }

    void AddExtraConnectionsSmart(int extraCount)
    {
        for (int i = 0; i < extraCount; i++)
        {
            int a = RandRange(0, roomCount);

            int best = -1;
            int bestDist = int.MaxValue;

            for (int b = 0; b < roomCount; b++)
            {
                if (a == b) continue;

                int dx = rooms[a].CenterX() - rooms[b].CenterX();
                int dy = rooms[a].CenterY() - rooms[b].CenterY();
                int dist = dx * dx + dy * dy;

                if (dist < bestDist && RandRange(0, 2) == 0) // 랜덤 필터
                {
                    bestDist = dist;
                    best = b;
                }
            }

            if (best != -1)
            {
                DigLine(
                    rooms[a].CenterX(), rooms[a].CenterY(),
                    rooms[best].CenterX(), rooms[best].CenterY()
                );
            }
        }
    }

    void OnDrawGizmos()
    {
        if (!showGizmos || map == null)
            return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Gizmos.color = (map[y, x] == FLOOR) ? Color.white : Color.black;
                Vector3 pos = new Vector3(x - width / 2 + 0.5f, 0.5f, y - height / 2 + 0.5f);
                Gizmos.DrawCube(pos, Vector3.one);
            }
        }
    }

    public int[,] GenerateMap()
    {
        map = new int[height, width];
        roomCount = 0;

        // 맵 초기화
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                map[y, x] = EMPTY;

        // 방 생성
        for (int i = 0; i < totalRoom; i++)
            AddRoom();

        // 연결
        ConnectRooms();

        AddExtraConnectionsSmart(5);

        return map;
    }
}
