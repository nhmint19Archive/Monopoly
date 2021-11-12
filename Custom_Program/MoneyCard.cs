using System;
using System.Collections.Generic;
using SplashKitSDK;

namespace Custom_Program
{
    /// <summary>
    /// This is the card that has money-related effect
    /// </summary>
    public class MoneyCard : Card
    {
        public override void Activate(Player player, Board board)
        {
            int money = new Random().Next(-200, 201);
            if (money < 0)
                Description = "Pay " + (-money) + "$";
            else
                Description = "Earn " + money + "$";
            player.Money += money;
        }
    }
}
