using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScripableAction : ScriptableObject
{
    public abstract void Action(Tile[] hostBoard, Tile[] clientBoard, Vector2Int attacker, Vector2Int attacked, bool isHost);
}
