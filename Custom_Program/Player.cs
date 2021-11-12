using System;
using System.Collections.Generic;
using SplashKitSDK;

namespace Custom_Program
{
    /// <summary>
    /// Represents the player on the board
    /// </summary>
    public class Player : DrawableObject
    {
        private const int JailFee = 200; // Fee to escape jail
        private readonly int[,] PlayerPosOnCell = new int[4, 2]
        {
            {15, 15},
            {15, -15},
            {-15, 15},
            {-15, -15}
        };
        // contains the in-game actions of the player, each corresponds a button
        private readonly static Dictionary<string, TemporaryButton> _actions = new Dictionary<string, TemporaryButton>()
        {
            { "Buy", new TemporaryButton(204, 350, 100, 50, "Buy", 25, Color.SkyBlue, Color.SteelBlue, Color.SlateGray) },
            { "Build", new TemporaryButton(204, 350, 100, 50, "Build", 25, Color.SkyBlue, Color.SteelBlue, Color.SlateGray) },
            { "Skip", new TemporaryButton(442, 350, 100, 50, "Skip", 25, Color.SkyBlue, Color.SteelBlue, Color.SlateGray) },
            { "Sell", new TemporaryButton(323, 350, 100, 50, "Sell", 25, Color.SkyBlue, Color.SteelBlue, Color.SlateGray) },
            { "Pay", new TemporaryButton(305, 370, 140, 50, "Pay", 50, Color.SkyBlue, Color.SteelBlue, Color.SlateGray) },
            { "Roll", new TemporaryButton(305, 485, 140, 50, "Roll", 50, Color.SkyBlue, Color.SteelBlue, Color.SlateGray) }
        };
        // get the action
        public static TemporaryButton GetAction(string name)
        {
            if (_actions.ContainsKey(name))
                return _actions[name];
            return null;
        }
        // disable all action buttons
        public static void DisableAllActions()
        {
            foreach (TemporaryButton btn in _actions.Values)
            {
                btn.Disabled = true;
            }
        }
        // subscribe action buttons to mouse input manager
        public static void SubscribeToMIManager(MouseInputManager m)
        {
            foreach (TemporaryButton btn in _actions.Values)
                m.Add(btn);
        }
        private int _money; // the current money
        private int _position; // the player's position on the board
        private int _playerNo; // the player's number 
        private string _name; // the player's name
        private float _radius; // radius for player drawing
        private Color _color; // color that represents the player
        private bool _isInTurn; // check if the player is in turn 
        private bool _isSelling; // check if the player is selling some lands
        private bool _isBankrupt; // check if the player has been bankrupt
        private bool _isChoosingFes; // check if the player is choosing festival
        private int _turnsInJail; // check if the player is in jail
        private int _sameDice; // the number of times that two dices have the same number
        // the lands that belong to the player
        private List<BuyableCell> _lands = new List<BuyableCell>();
        // the lands that are for later use
        private List<BuyableCell> _temporaryLands = new List<BuyableCell>();
        public Player(float x, float y, float radius, string name, Color color, int money, int playerNo) : base(x, y) {
            _money = money;
            _position = 0;
            _radius = radius;
            _name = name;
            _color = color;
            _isInTurn = false;
            _isSelling = false;
            _isBankrupt = false;
            _turnsInJail = 0;
            _sameDice = 0;
            _playerNo = playerNo;
        }
        // add land to _lands
        public void AddLand(BuyableCell land)
        {
            _lands.Add(land);
            land.OwnedBy = this;
        }
        // get number of lands owned
        public int LandCount
        {
            get { return _lands.Count; }
        }
        // get land by type 
        public List<T> GetLands<T>() where T : BuyableCell
        {
            List<T> res = new List<T>();
            foreach (var land in _lands)
            {
                if (land is T)
                    res.Add((T)land);
            }
            return res;
        }
        // update the _temporaryLands
        public void UpdateTemporaryLands()
        {
            foreach (BuyableCell land in _lands)
            {
                if (land.IsSelected)
                {
                    if (_temporaryLands.Contains(land))
                        _temporaryLands.Remove(land);
                    else
                        _temporaryLands.Add(land);
                }
            }
        }
        // show the temporary lands
        public List<BuyableCell> TemporaryLands
        {
            get { return _temporaryLands; }
        }
        public int Position
        {
            get { return _position; }
            set { _position = value; }
        }
        public int Money
        {
            get { return _money; }
            set { _money = value; }
        }
        public string Name
        {
            get { return _name; }
        }
        public Color Color
        {
            get { return _color; }
        }
        public string Description
        {
            get 
            {
                if (_isBankrupt)
                    return _name + ": Bankrupt"; 
                return _name + ": " + _money + "$"; 
            }
        }
        // the total value of _temporaryLands
        public int SellTotal
        {
            get
            {
                int total = _money;
                foreach (BuyableCell land in _temporaryLands)
                {
                    total += land.Value;
                }
                return total;
            }
        }
        // The total value of current money and all lands
        public int TotalMoney
        {
            get
            {
                int total = _money;
                foreach (BuyableCell land in _lands)
                    total += land.Value;
                return total;
            }
        }
        public bool IsInTurn
        {
            get { return _isInTurn; }
            set { _isInTurn = value; }
        }
        public bool IsSelling
        {
            get { return _isSelling; }
            set { _isSelling = value; }
        }
        public bool IsChoosingFes
        {
            get { return _isChoosingFes; }
            set { _isChoosingFes = value; }
        }
        public bool IsBankrupt
        {
            get { return _isBankrupt; }
            set { _isBankrupt = value; }
        }
        public int TurnsInJail
        {
            get { return _turnsInJail; }
            set { _turnsInJail = value; }
        }
        public int SameDice
        {
            get { return _sameDice; }
            set { _sameDice = value; }
        }
        // the player moves to a cell on the board
        public void MoveTo(Board board, Cell c)
        {
            _position = c.Position;
            float playerX = (c.X + c.Img.Width / 2) + PlayerPosOnCell[_playerNo, 0];
            float playerY = (c.Y + c.Img.Height / 2) + PlayerPosOnCell[_playerNo, 1];
            X = playerX;
            Y = playerY;
        }
        // player buys a lands
        public void Buy(BuyableCell c)
        {
            _money -= c.Price;
            AddLand(c);
        }
        // strategy of buying lands
        public void BuyStrategy(BuyableCell c)
        {
            var buyBtn = _actions["Buy"];
            var skipBtn = _actions["Skip"];
            buyBtn.Disabled = false;
            skipBtn.Disabled = false;
            EventHandler onBuy = (object sender, EventArgs e) =>
            {
                Buy(c);
                EndTurn();
            };
            EventHandler onSkip = (object sender, EventArgs e) => EndTurn();
            EventHandler removeEvent = (object sender, EventArgs e) =>
            {
                buyBtn.Clicked -= onBuy;
                skipBtn.Clicked -= onSkip;
            };
            buyBtn.Clicked += onBuy;
            skipBtn.Clicked += onSkip;
            buyBtn.Clicked += removeEvent;
            skipBtn.Clicked += removeEvent;
        }
        // player build a house on a property
        public void Build(Property c, int price)
        {
            _money -= price;
            c.Houses += 1;
        }
        // strategy of building a house
        public void BuildStrategy(Property c, int price)
        {
            var buildBtn = _actions["Build"];
            var skipBtn = _actions["Skip"];
            buildBtn.Disabled = false;
            skipBtn.Disabled = false;
            EventHandler onBuild = (object sender, EventArgs e) =>
            {
                Build(c, price);
                EndTurn();
            };
            EventHandler onSkip = (object sender, EventArgs e) => EndTurn();
            EventHandler removeEvent = (object sender, EventArgs e) =>
            {
                buildBtn.Clicked -= onBuild;
                skipBtn.Clicked -= onSkip;
            };
            buildBtn.Clicked += onBuild;
            skipBtn.Clicked += onSkip;
            buildBtn.Clicked += removeEvent;
            skipBtn.Clicked += removeEvent;
        }
        // player sells multiple lands
        public void Sell()
        {
            foreach (BuyableCell land in _temporaryLands)
            {
                _money += land.Value;
                land.Reset();
                _lands.Remove(land);
            }
        }
        // player pays money to other player
        public void Pay(Player receiver, int debt)
        {
            _money -= debt;
            if (receiver != null) // receiver can be null e.g when pay tax
                receiver.Money += debt;
        }
        // strategy of selling multiple lands
        public void SellStrategy(Player receiver, int debt)
        {
            var sellBtn = _actions["Sell"];
            sellBtn.Disabled = false;
            EventHandler onSell = (object sender, EventArgs e) =>
            {
                if (SellTotal >= debt)
                {
                    Sell();
                    Pay(receiver, debt);
                    EndTurn();
                }
                else
                {
                    SplashKit.DisplayDialog("", "Not have enough money to pay", SplashKit.FontNamed("CellFont"), 15);
                    SellStrategy(receiver, debt);
                }
            };
            sellBtn.Clicked += onSell;
            sellBtn.Clicked += (object sender, EventArgs e) => sellBtn.Clicked -= onSell;
        }
        // strategy of escaping jail
        public void JailStrategy(Dice dice1, Dice dice2)
        {
            if (_money < JailFee)
            {
                dice1.Disabled = false;
                dice2.Disabled = false;
                RollStrategy(dice1, dice2);
            }
            else
            {
                var payBtn = _actions["Pay"];
                var rollBtn = _actions["Roll"];
                payBtn.Disabled = false;
                rollBtn.Disabled = false;
                EventHandler onPay = (object sender, EventArgs e) =>
                {
                    _money -= JailFee;
                    _turnsInJail = 0;
                    payBtn.Disabled = true;
                    dice1.Disabled = false;
                    dice2.Disabled = false;
                };
                EventHandler onRoll = (object sender, EventArgs e) =>
                {
                    payBtn.Disabled = true;
                    payBtn.Clicked -= onPay;
                    dice1.Disabled = false;
                    dice2.Disabled = false;
                    dice1.Roll();
                    dice2.Roll();
                    rollBtn.Disabled = true;
                };
                payBtn.Clicked += onPay;
                rollBtn.Clicked += onRoll;
                payBtn.Clicked += (object sender, EventArgs e) => payBtn.Clicked -= onPay;
                rollBtn.Clicked += (object sender, EventArgs e) => rollBtn.Clicked -= onRoll;
            }
        }
        // strategy of roll the dice
        public void RollStrategy(Dice dice1, Dice dice2)
        {
            var btn = _actions["Roll"];
            btn.Disabled = false;
            EventHandler onBtnClick = (object sender, EventArgs e) =>
            {
                dice1.Roll();
                dice2.Roll();
                btn.Disabled = true;
            };
            btn.Clicked += onBtnClick;
            btn.Clicked += (object sender, EventArgs e) => btn.Clicked -= onBtnClick;
        }
        // player ends the turn
        public void EndTurn()
        {
            DisableAllActions();
            _isInTurn = false;
            _temporaryLands.Clear();
            _isChoosingFes = false;
            _isSelling = false;
        }
        // player sells all lands
        public void SellAll()
        {
            foreach (BuyableCell land in _lands)
            {
                _money += land.Value;
                land.Reset();
            }
            _lands.Clear();
        }
        public override void Draw()
        {
            SplashKit.FillCircle(_color, X, Y, _radius);
            // Draw Action Buttons
            foreach (Button btn in _actions.Values)
                btn.Draw();
            // Draw Outline for selected lands
            foreach (BuyableCell c in _temporaryLands)
                c.DrawOutline(Color.HotPink);
        }
    }
}
