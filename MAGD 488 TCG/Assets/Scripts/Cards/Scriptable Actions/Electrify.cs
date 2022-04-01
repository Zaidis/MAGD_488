using UnityEngine;
public class Electrify : ScripableAction
{
    public override void Action(Tile[] hostBoard, Tile[] clientBoard, Vector2Int attacker, Vector2Int attacked, bool isHost) {
        foreach (Tile t in hostBoard)
            Debug.Log(t.token.name);
        foreach (Tile t in clientBoard)
            Debug.Log(t.token.name);
        Debug.Log("Attacker Position: " + attacker.x + ", " + attacker.y + "\nAttacked Position: " + attacked.x + ", " + attacked.y + "\nIs Host: " + (isHost ? "True" : "False"));
    }
}
