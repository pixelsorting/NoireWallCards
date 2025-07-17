using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BepInEx;
using UnboundLib;
using UnboundLib.Cards;
using ModdingUtils;

namespace NoireWallCards
{
    public class CardHolder : MonoBehaviour
    {
        public List<GameObject> Cards;
        public List<GameObject> HiddenCards;

        internal void RegisterCards()
        {
            foreach (var Card in Cards)
            {
                CustomCard.RegisterUnityCard(Card, NoireWallCards.modInitials, Card.GetComponent<CardInfo>().cardName, enabled:true, null);
            }
            foreach (var Card in HiddenCards)
            {
                CustomCard.RegisterUnityCard(Card, NoireWallCards.modInitials, Card.GetComponent<CardInfo>().cardName, enabled:false, null);
                ModdingUtils.Utils.Cards.instance.AddHiddenCard(Card.GetComponent<CardInfo>());
            }
        }
    }
}