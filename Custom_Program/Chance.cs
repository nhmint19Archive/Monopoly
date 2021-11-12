using System;
using System.Collections.Generic;
using System.Reflection;
using SplashKitSDK;

namespace Custom_Program
{
    /// <summary>
    /// Chance is a where player picks a random card
    /// </summary>
    public class Chance : Cell
    {
        // a dictionary of available cards 
        private static Dictionary<Type, Card> _cards = new Dictionary<Type, Card>();
        private Card _pickCard; // the card that is picked
        // add cards
        public static void AddCard(Card card) => _cards.Add(card.GetType(), card);
        // add cards by type
        public static void AddCard<T>() where T : Card
        {
            T card = Activator.CreateInstance<T>();
            AddCard(card);
        }
        public Chance(float x, float y, string name, Bitmap img, Board board) : base(x, y, name, img, board) 
        {
            _pickCard = null;       
        }
        public override string Description
        {
            get { return "Get a mysterious card"; }
        }
        public override string OnPlayerEnter(Player player)
        {
            string res = player.Name + " picks a card\n";
            if (_cards.Count > 0)
            {
                // get random cards
                int position = player.Position;
                int cardNo = new Random().Next(0, _cards.Count);
                List<Card> cardList = new List<Card>(_cards.Values);
                _pickCard = cardList[cardNo];
                // activate the card's effect
                _pickCard.Activate(player, _board);
                res += _pickCard.Description;
                _pickCard.Description = "";
                _pickCard = null;
                // if the player is not in the original position
                if (player.Position != position)
                    res += "\n\n" + _board.FindCell(player.Position).OnPlayerEnter(player);
                else
                    player.IsInTurn = false;
            }
            return res;
        }
    } 
}
