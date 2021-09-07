using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotMultipleTile : MonoBehaviour
{
    public BotController botController;

    public Dictionary<Vector3Int, Vector3Int> tiles = new Dictionary<Vector3Int, Vector3Int>();

    public Vector3Int mainTile;

    private Vector3Int oldPos;

    private string oldDirection;

    public void Start()
    {
        for(int x = 1; x <= botController.size; x++)
        {
            for (int y = 1; y <= botController.size; y++)
            {
                tiles.Add(new Vector3Int(x, y, 0), new Vector3Int(botController.currentTileIndex.x + (x-1), botController.currentTileIndex.y + (y - 1),0));
            }
        }

        oldPos = botController.currentTileIndex;

        CalculateNewTiles("S","S", botController.currentTileIndex);
        //Walk(botController.currentTileIndex + Vector3Int.up);
    }

    public bool CheckMultipleTile(Vector3Int pos)
    {
        return tiles.ContainsValue(pos);
    }

    public bool Walk(Vector3Int newPos)
    {
        if (newPos != oldPos)
        {
            var direction = Manager.Instance.gameManager.GetDirection(oldPos, newPos);
            string redirectDirection = "";
            if (direction == "N" || direction == "NW")
            {
                redirectDirection = "N";
            }
            if (direction == "E" || direction == "NE")
            {
                redirectDirection = "E";
            }
            if (direction == "W" || direction == "SW")
            {
                redirectDirection = "W";
            }
            if (direction == "S" || direction == "SE")
            {
                redirectDirection = "S";
            }

            if (oldDirection != redirectDirection)
            {
                ChangeDirectionOfTiles(redirectDirection);
            }

            return CalculateNewTiles(direction, redirectDirection, newPos);
        }
        return true;
    }

    public bool HasBotInTile(Vector3Int pos)
    {
        foreach(var aux in tiles)
        {
            if(aux.Value == pos)
            {
                return true;
            }
        }
        return false;
    }

    public bool CalculateNewTiles(string direction, string redirectDirection, Vector3Int newPos)
    {
        Dictionary<Vector3Int, Vector3Int> newTiles = new Dictionary<Vector3Int, Vector3Int>(tiles);
        if (newPos != oldPos)
        {
            foreach (var aux in tiles)
            {
                Vector3Int newTile = aux.Value;
                if(direction == "N")
                {
                    newTile.x += 1;
                    newTile.y += 1;
                }
                if (direction == "S")
                {
                    newTile.x -= 1;
                    newTile.y -= 1;
                }
                if (direction == "E")
                {
                    newTile.x += 1;
                    newTile.y -= 1;
                }
                if (direction == "W")
                {
                    newTile.x -= 1;
                    newTile.y += 1;
                }
                if (direction == "NW")
                {
                    newTile.y += 1;
                }
                if (direction == "NE")
                {
                    newTile.x += 1;
                }
                if (direction == "SW")
                {
                    newTile.x -= 1;
                }
                if (direction == "SE")
                {
                    newTile.y -= 1;
                }

                newTiles[aux.Key] = newTile;
            }
        }

        foreach(var aux in newTiles)
        {
            GameManager gameManager = Manager.Instance.gameManager;
            if (!gameManager.HasAvailableTile(new Vector3Int(aux.Value.x, aux.Value.y, 0)))
            {
                return false;
            }
        }

        tiles = newTiles;
        oldDirection = redirectDirection;
        oldPos = newPos;
        return true;
    }

    public void ChangeDirectionOfTiles(string newDirection)
    {
        int newDir = ConvertDirection(newDirection);
        int oldDir = ConvertDirection(oldDirection);

        if(oldDir<newDir)
        {
            tiles = TurnLeft();
            newDir--;
            ChangeDirectionOfTiles(ConvertDirection(newDir));
        }
        else if(oldDir>newDir)
        {
            tiles = TurnRight();
            newDir++;
            ChangeDirectionOfTiles(ConvertDirection(newDir));
        }
    }

    public int ConvertDirection(string dir)
    {
        if(dir == "N")
            return 0;
        if (dir == "E")
            return 1;
        if (dir == "W")
            return 2;
        if (dir == "S")
            return 3;

        return 0;
    }


    public string ConvertDirection(int dir)
    {
        if (dir == 0)
            return "N";
        if (dir == 1)
            return "E";
        if (dir == 2)
            return "W";
        if (dir == 3)
            return "S";

        return "";
    }
    public Dictionary<Vector3Int, Vector3Int> TurnRight()
    {
        Dictionary<Vector3Int, Vector3Int> newTiles = new Dictionary<Vector3Int, Vector3Int>(tiles);

        for (int x = 1; x < botController.size; x++)
        {
            newTiles[new Vector3Int(x+1, 1, 0)] = tiles[new Vector3Int(x, 1, 0)];
        }
        for (int y = 1; y < botController.size; y++)
        {
            newTiles[new Vector3Int(botController.size, y+1, 0)] = tiles[new Vector3Int(botController.size, y, 0)];
        }
        for (int x = botController.size; x > 1; x--)
        {
            newTiles[new Vector3Int(x - 1, botController.size, 0)] = tiles[new Vector3Int(x, botController.size, 0)];
        }
        for (int y = botController.size; y > 1; y--)
        {
            newTiles[new Vector3Int(1, y - 1, 0)] = tiles[new Vector3Int(1, y, 0)];
        }

        return newTiles;
    }

    public Dictionary<Vector3Int, Vector3Int> TurnLeft()
    {
        Debug.Log("TURN LEFT");
        Dictionary<Vector3Int, Vector3Int> newTiles = tiles;

        for (int x = 1; x < botController.size; x++)
        {
            newTiles[new Vector3Int(x + 1, botController.size, 0)] = tiles[new Vector3Int(x, botController.size,0)];
        }
        for (int y = 1; y < botController.size; y++)
        {
            newTiles[new Vector3Int(1, y + 1, 0)] = tiles[new Vector3Int(1, y, 0)];
        }
        for (int x = botController.size; x > 1; x--)
        {
            newTiles[new Vector3Int(x - 1, 1, 0)] = tiles[new Vector3Int(x, 1, 0)];
        }
        for (int y = botController.size; y > 1; y--)
        {
            newTiles[new Vector3Int(botController.size, y - 1, 0)] = tiles[new Vector3Int(botController.size, y, 0)];
        }

        return newTiles;
    }
}
