using System;
using System.Collections.Generic;
using SplashKitSDK;

namespace Custom_Program
{
    /// <summary>
    /// Contains main game logics including update, input handle and draw
    /// </summary>
    public class Monopoly
    {
        // the money earn after getting through start line
        private const int LapMoney = 300;
        // the color assigned to players
        private readonly Color[] PlayerColor = new Color[4]
        {
            Color.SwinburneRed,
            Color.Blue,
            Color.Yellow,
            Color.Green,
        };
        private int _turn; // indicate the player's turn
        private Bitmap _sideBarImg; // the side bar bitmap that represents a chosen cell
        private MouseInputManager _MIManager; // mouse click manager
        private MessageBox _msgBox; // the main message box appear in the board
        private MessageBox _sideMsgBox; // the side message box appear in the side of the board
        private List<Player> _players = new List<Player>(); // list of players
        private Board _board; // the main game board
        private Dice _dice1; // dice used for game
        private Dice _dice2;
        public Monopoly(string[] players)
        {
            GameDatabase db = GameDatabase.GetDatabase();
            try
            {
                // Initialize fonts
                SplashKit.LoadFont("GameFont", "Bangers.ttf");
                SplashKit.LoadFont("CellFont", "OpenSans.ttf");

                // Initialize turn
                _turn = 0;

                // Initialize side bar image
                _sideBarImg = null;

                // Initialize mouse input manager
                _MIManager = new MouseInputManager();

                // Initialize dices
                Bitmap diceImg = SplashKit.LoadBitmap("DiceImg", "dice.png");
                SplashKit.BitmapSetCellDetails(diceImg, 136, 145, 6, 1, 6);
                AnimationScript diceScript = SplashKit.LoadAnimationScript("DiceScript", "dice.txt");
                _dice1 = new Dice(224, 250, diceImg);
                _dice1.Anim = diceScript.CreateAnimation("DiceRoll");
                _dice2 = new Dice(395, 250, diceImg);
                _dice2.Anim = diceScript.CreateAnimation("DiceRoll");

                // Initialize property images
                Property.RegisterImg(1, "1.png");
                Property.RegisterImg(2, "2.png");
                Property.RegisterImg(3, "3.png");
                Property.RegisterImg(4, "4.png");
                Property.RegisterImg(5, "5.png");
                Property.RegisterImg(6, "6.png");
                Property.RegisterImg(7, "7.png");
                Property.RegisterImg(8, "8.png");

                // Initialize chance and cards
                Chance.AddCard<MoneyCard>();
                Chance.AddCard<StepCard>();

                // Initialize board and cells
                _board = new Board();
                _board.CellFactory.RegisterCell("property", typeof(Property));
                _board.CellFactory.RegisterCell("start", typeof(Start));
                _board.CellFactory.RegisterCell("jail", typeof(Jail));
                _board.CellFactory.RegisterCell("festival", typeof(Festival));
                _board.CellFactory.RegisterCell("tax", typeof(Tax));
                _board.CellFactory.RegisterCell("chance", typeof(Chance));
                _board.CellFactory.RegisterCell("carpark", typeof(CarPark));
                _board.CellFactory.RegisterCell("resort", typeof(Resort));
                _board.CellFactory.RegisterImg("start", "start.png");
                _board.CellFactory.RegisterImg("property", null);
                _board.CellFactory.RegisterImg("jail", "jail.png");
                _board.CellFactory.RegisterImg("festival", "festival.png");
                _board.CellFactory.RegisterImg("tax", "tax.png");
                _board.CellFactory.RegisterImg("chance", "chance.png");
                _board.CellFactory.RegisterImg("carpark", "carpark.png");
                _board.CellFactory.RegisterImg("resort", "resort.png");
                _board.Load("BoardSetup.txt");
                _board.Clicked += (object sender, EventArgs e) => BoardClickHandle();
                _MIManager.Add(_board);

                // Initialize festival symbol
                _board.FesSymbol = SplashKit.LoadBitmap("festival symbol", "festival_symbol.png");

                // Initialize players
                string[] playerNames = players;
                for (int i = 0; i < playerNames.Length; i++)
                {
                    Player p = new Player(0, 0, 10, playerNames[i], PlayerColor[i], 1000, i);
                    p.MoveTo(_board, _board.FindCell<Start>());
                    _players.Add(p);
                }
                _players[_turn].IsInTurn = true;
                _players[_turn].RollStrategy(_dice1, _dice2);
                Player.SubscribeToMIManager(_MIManager);

                // Initialize message box and side message box
                _msgBox = new MessageBox(_board.X, 185, _board.Width, 25);
                _sideMsgBox = new MessageBox(749, 382, 425, 20);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                // Free Database
                db.FreeDB();
            }
        }
        public void Update()
        {
            if (PlayersLeft.Count > 1)
            {
                // When the player ends the turn by clicking something/by default
                if (!_players[_turn].IsInTurn)
                {
                    NewTurn();
                }
                // When dice stops rolling, player walks to the new cell and follow the game rules
                if (_dice1.EndRolling() && _dice2.EndRolling())
                {
                    InTurn();
                }
            }
            else
            {
                _msgBox.Msg = "Congratulations\n" + PlayersLeft[0].Name + " Wins!";
                _dice1.Disabled = true;
                _dice2.Disabled = true;
                Player.DisableAllActions();
            }
        }
        public void Draw()
        {
            SplashKit.DrawLine(Color.Gray, 749, 238, 1174, 238); 
            SplashKit.DrawText("Click a cell to see its description", Color.Gray, "GameFont", 25, 810, 240);
            if (_sideBarImg != null)
                SplashKit.DrawBitmap(_sideBarImg, 749 + (425 - _sideBarImg.Width) / 2, 280);
            // Draw basic objects
            _board.Draw();
            _dice1.Draw();
            _dice2.Draw();
            // Draw the player information
            for (int i = 0; i < _players.Count; i++)
            {
                if (!_players[i].IsBankrupt)
                    _players[i].Draw() ;
                // Draw player's information
                SplashKit.DrawText(_players[i].Description, Color.Black, "GameFont", 25, 760, 25 + i * 50);
                SplashKit.FillCircle(PlayerColor[i], 1100, 35 + i * 50, 10);
            }

            // Draw the game message
            _msgBox.Draw();
            _sideMsgBox.Draw();
        }
        public void HandleInput()
        {
            if (PlayersLeft.Count > 1)
                _MIManager.NotifyObservers();
        }
        // check Game is over
        public List<Player> PlayersLeft
        {
            get
            {
                return _players.FindAll(p => !p.IsBankrupt);
            }
        }
        // deal with actions when interacting with the board
        private void BoardClickHandle()
        {

            Cell c = _board.SelectCell(SplashKit.MousePosition());
            if (c == null)
            {
                _sideMsgBox.Msg = "";
                _sideBarImg = null;
            }
            else
            {
                // the side message box shows the cell description
                _sideMsgBox.Msg = c.Description;
                _sideBarImg = c.Img;
                Player curPlayer = _players[_turn];
                if (curPlayer.IsSelling) // when player is selling some lands
                {
                    BoardClickSell(curPlayer);
                }
                if (curPlayer.IsChoosingFes)
                {
                    BoardClickFestival(curPlayer);
                }
            }
        }
        // selling logic when board is clicked
        private void BoardClickSell(Player player)
        {
            player.UpdateTemporaryLands();
            // update the message box
            string[] msgLines = _msgBox.Msg.Split("\n");
            _msgBox.Msg = _msgBox.Msg.Replace(msgLines[msgLines.Length - 1], "Sell Total: " + player.SellTotal + "$");
        }
        // festival logic when board is clicked
        private void BoardClickFestival(Player player)
        {
            player.UpdateTemporaryLands();

            if (player.TemporaryLands.Count == 1)
            {
                _board.FesCell = player.TemporaryLands[0];
                _msgBox.Msg = _board.FesCell.Name + " is now hosting festival";
                player.EndTurn();
            }
        }
        // deal with the transition between old turn and new turn
        private void NewTurn()
        {
            GameUtilities.ScreenDelay(1000);
            _msgBox.Msg = "";
            if (_players[_turn].SameDice == 0) // if the player didnt roll the same dice
            {
                do
                {
                    _turn = (_turn + 1) % _players.Count;
                } while (_players[_turn].IsBankrupt);
            }
            else
                _msgBox.Msg = _players[_turn].Name + " rolled the same dice\n";
            Player curPlayer = _players[_turn];
            curPlayer.IsInTurn = true;
            _msgBox.Msg += curPlayer.Name + "'s turn";
            if (curPlayer.TurnsInJail > 0)
            {
                // if the player is in jail
                curPlayer.JailStrategy(_dice1, _dice2);
            }
            else
            {
                _dice1.Disabled = false;
                _dice2.Disabled = false;
                curPlayer.RollStrategy(_dice1, _dice2);
            }

        }
        // deal with the actions after rolling ends (in-turn)
        private void InTurn()
        {
            Player curPlayer = _players[_turn];
            Cell tempCell = null;
            // if the dice number is the same, player can keep moving
            if (_dice1.Value == _dice2.Value)
                curPlayer.SameDice++;
            else
                curPlayer.SameDice = 0;
            // if the number of times that dices have the same number is 3, player goes to jail
            if (curPlayer.SameDice == 3)
            {
                _msgBox.Msg = curPlayer.Name + " goes to jail because of 3 same number rolls";
                SplashKit.Delay(1000);
                curPlayer.SameDice = 0;
                curPlayer.TurnsInJail = 3;
                curPlayer.MoveTo(_board, _board.FindCell<Jail>());
                curPlayer.IsInTurn = false;
            }
            else if (curPlayer.TurnsInJail > 0 && curPlayer.SameDice > 0)
            {
                // if the player is in jail but rolls the same dice
                _msgBox.Msg = curPlayer.Name + " has got out of jail!";
                GameUtilities.ScreenDelay(1000);
                curPlayer.TurnsInJail = 0;
                curPlayer.IsInTurn = false;
            }
            else if (curPlayer.TurnsInJail > 0 && curPlayer.SameDice == 0)
            {
                // if the player is in jail but does not roll the same dice
                _msgBox.Msg = curPlayer.Name + " has " + curPlayer.TurnsInJail + " turns to get out of jail!";
                GameUtilities.ScreenDelay(1000);
                curPlayer.TurnsInJail--;
                curPlayer.IsInTurn = false;
            }
            else
            {
                // the movement of the current player
                for (int i = 1; i <= (3); i++)
                {
                    tempCell = _board.FindCell((curPlayer.Position + 1) % _board.CellNumber);
                    curPlayer.MoveTo(_board, tempCell);
                    AddLapMoney(curPlayer);
                    GameUtilities.ScreenDelay(150, curPlayer.Draw);
                }
                // update message box
                _msgBox.Msg = tempCell.OnPlayerEnter(curPlayer);
            }
            // reset the dice
            _dice1.Reset();
            _dice2.Reset();
        }

        // player earns money after a lap (go through start)
        private void AddLapMoney(Player player)
        {
            if (player.Position == 0)
                player.Money += LapMoney;
        }
    }
}