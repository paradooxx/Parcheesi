using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
   public int nodeIndex;
   public bool isStarNode;
   public bool isFinishNode;
   public List<Pawn> pawnsOnNode = new List<Pawn>();

   public const int MAX_PLAYERS_PER_NODE = 2;

   public delegate void OnPawnLands(Pawn pawn);
   public event OnPawnLands PawnLandsEvent;

    public bool CanPawnLand(Pawn pawn)
    {
        if(pawnsOnNode.Contains(pawn)) return true;
        if(isStarNode) return true;
        
        int maxPawnsAllowed = isFinishNode ? 4 : MAX_PLAYERS_PER_NODE;

        int playerPieceCount = 0;
        foreach (Pawn p in pawnsOnNode)
        {
            if(p.mainPlayer == pawn.mainPlayer)
            {
                playerPieceCount++;
            }
        }

        return playerPieceCount < maxPawnsAllowed;

        // if(playerPieceCount < MAX_PLAYERS_PER_NODE)
        // {
        //     return true;
        // }
        // else
        // {
        //     return false;
        // }
    }

    public bool AddPawn(Pawn pawn)
    {
        if(pawnsOnNode.Contains(pawn))
        {
            return true;
        }

        int maxPawnsAllowed = isFinishNode ? 4 : MAX_PLAYERS_PER_NODE;
        
        int playerPieceCount = 0;
        foreach (Pawn p in pawnsOnNode)
        {
            if(p.mainPlayer == pawn.mainPlayer)
            {
                playerPieceCount++;
            }
        }
        //checking if the node is full or not
        if(playerPieceCount < maxPawnsAllowed)
        {
            pawnsOnNode.Add(pawn);
            PawnLandsEvent?.Invoke(pawn);
            return true;
        }
        else
        {
            Debug.Log("Node Full");
            EnablePawnInteraction(pawnsOnNode);
            return false;
        }
    }

    private void EnablePawnInteraction(List<Pawn> pawnsOnNode)
    {
        foreach(Pawn p in pawnsOnNode)
        {
            p.EnableMovementInteraction();
        }
    }

    public void RemovePawn(Pawn pawn)
    {
        if(pawnsOnNode.Contains(pawn))
        {
            pawnsOnNode.Remove(pawn);
        }
    }

    public void TryEliminatePawn(Pawn pawn)
    {
        if(isStarNode) return;

        foreach(Pawn p in new List<Pawn>(pawnsOnNode))
        {
            if(p.mainPlayer != pawn.mainPlayer)
            {
                p.ResetToHomePosition();
                Debug.Log($"Pawn {p.name} eliminated by {pawn.name}.");
            }
        }
    }

    public bool IsBlockedForOtherPlayer(Pawn pawn)
    {
        int maxPawnsAllowed = isFinishNode ? 4 : MAX_PLAYERS_PER_NODE;
        int playerPieceCount = 0;

        foreach (Pawn p in pawnsOnNode)
        {
            playerPieceCount ++;
        }
        return playerPieceCount >= maxPawnsAllowed && pawnsOnNode[0].mainPlayer != pawn.mainPlayer;
    }
}
