using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreatureToken : Token
{
    public int currentAttack;
    public Creature creature;
    public TextMeshPro Attack;
    public override void ApplyCard() {
        currentAttack = creature.defaultPowerAmount;
        currentHealth = creature.defaultHealthAmount;
        Name.text = creature.name;
        Description.text = creature.description;
        Mana.text = creature.manaCost.ToString();
        Art = creature.cardArt;
    }
    public void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Vector2Int attacker, Vector2Int attacked, bool isHost) {
        creature.OnAttack.Action(hostBoard, clientBoard, attacker, attacked, isHost);
    }
}
