using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Artifact/Hellhound Whistle", fileName = "Card")]
public class Hellhound_Whistle : Artifact
{

    //give surrounding melee creatures +2/+0
    public override void OnPlay(Tile[] hostBoard, Tile[] clientBoard, Tile parent) {

        Debug.LogError("Artifact On Play occurs");

        // base.OnPlay();
        int myTileID = parent.GetTileID();
        int leftTile = myTileID - 1;
        int rightTile = myTileID + 1;
        int frontTile = myTileID + 5;
        if (parent.hostTile) {

            if (parent.meleeTile) {
                //check left and right
                if(leftTile >= 5) {
                    if (hostBoard[leftTile].meleeTile) {
                        if (hostBoard[leftTile].token != null) {
                            if (hostBoard[leftTile].token.GetComponent<Token>() is CreatureToken c) {
                                c.currentAttack += 2;
                                c.UpdateStats();
                            }
                        }
                    }
                }
                if(rightTile <= 9) {
                    if (hostBoard[rightTile].meleeTile) {
                        if (hostBoard[rightTile].token != null) {
                            if (hostBoard[rightTile].token.GetComponent<Token>() is CreatureToken c) {
                                c.currentAttack += 2;
                                c.UpdateStats();
                            }
                        }
                    }
                }
            } else {
                //check in front
                if (hostBoard[frontTile].meleeTile) {
                    if (hostBoard[frontTile].token != null) {
                        if (hostBoard[frontTile].token.GetComponent<Token>() is CreatureToken c) {
                            c.currentAttack += 2;
                            c.UpdateStats();
                        }
                    }
                }
            }

            parent.token.GetComponent<Token>().UpdateStats();

        } else {
            if (parent.meleeTile) {
                //check left and right
                if (leftTile >= 5) {
                    if (clientBoard[leftTile].meleeTile) {
                        if (clientBoard[leftTile].token != null) {
                            if (clientBoard[leftTile].token.GetComponent<Token>() is CreatureToken c) {
                                c.currentAttack += 2;
                                c.UpdateStats();
                            }
                        }
                    }
                }
                if (rightTile <= 9) {
                    if (clientBoard[rightTile].meleeTile) {
                        if (clientBoard[rightTile].token != null) {
                            if (clientBoard[rightTile].token.GetComponent<Token>() is CreatureToken c) {
                                c.currentAttack += 2;
                                c.UpdateStats();
                            }
                        }
                    }
                }
            }
            else {
                //check in front
                if (clientBoard[frontTile].meleeTile) {
                    if (clientBoard[frontTile].token != null) {
                        if (clientBoard[frontTile].token.GetComponent<Token>() is CreatureToken c) {
                            c.currentAttack += 2;
                            c.UpdateStats();
                        }
                    }
                }
            }

            parent.token.GetComponent<Token>().UpdateStats();
        }
    }

}
