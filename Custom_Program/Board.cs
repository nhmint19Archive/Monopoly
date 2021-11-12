using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using SplashKitSDK;

namespace Custom_Program
{
    /// <summary>
    /// The board is the core of the game, where players interact with cells
    /// </summary>
    public class Board : DrawableObject, IHaveMouseAction
    {
        // Board controls cells using a list
        private readonly List<Cell> _cells; // a list of all cells
        // Cell Factory helps create cells
        private CellFactory _cellFactory;
        // Draw Options for cells
        private int _width, _height;
        private BuyableCell _fesCell; // the cell that is holding festival
        private Bitmap _fesSymbol; // the symbol of festival cell
        public Board() : base(0, 0)
        {
            _cells = new List<Cell>();
            _cellFactory = new CellFactory();
            _fesCell = null;
            _fesSymbol = null;
        }
        // The number of cells that the board manages
        public int CellNumber
        {
            get { return _cells.Count; }
        }
        // The width and height pf the board
        public int Width
        {
            get { return _width; }
        }
        public int Height 
        {
            get { return _height; } 
        }
        // The Cell Factory of the board
        public CellFactory CellFactory
        {
            get { return _cellFactory; }
        }
        public BuyableCell FesCell
        {
            get { return _fesCell; }
            set { _fesCell = value; }
        }
        public Bitmap FesSymbol
        {
            get { return _fesSymbol; }
            set { _fesSymbol = value; }
        }
        // Addding cells to the board
        private void AddCell(Cell c)
        {
            _cells.Add(c);
        }
        // Find a cell based on its position in the board
        public Cell FindCell(int position)
        {
            if (position >= 0 && position < _cells.Count)
                return _cells[position];
            return null;
        }
        // Find the first cell based on its type
        public Cell FindCell<T>() where T: Cell=> _cells.Find(c => c is T);
        // Select a cell 
        public Cell SelectCell(Point2D pt)
        {
            Cell res = null;
            foreach (Cell c in _cells)
            {
                c.IsSelected = c.IsAt(pt);
                if (c.IsSelected)
                    res = c;
            }
            return res;
        }

        // Load the board setup file
        public void Load(string filename)
        {
            /* 
             * Load Board Setup from a file 
             * Board Setup in the file has information in this order:
             * - Board x, y coordinate; 
             * - Width and Height of the board in the window
             * - Number of cells on the width and on the height
             * - For each cell:
             * + type of the cell
             * + name of the cell
             * + x, y of the cell
             * - Other information for special cell:
             * + property: info about type of the property
             */
            StreamReader reader = new StreamReader(filename);
            GameDatabase db = GameDatabase.GetDatabase();
            try
            {
                X = reader.ReadFloat();
                Y = reader.ReadFloat();
                _width = reader.ReadInteger();
                _height = reader.ReadInteger();
                int wCells = reader.ReadInteger();
                int hCells = reader.ReadInteger();
                int cellID;
                string name;
                float x, y;
                Cell c;
                for (int i = 0; i < 2 * (wCells + hCells) - 4; i++)
                {
                    cellID = reader.ReadInteger();
                    x = reader.ReadFloat();
                    y = reader.ReadFloat();
                    name = reader.ReadLine();
                    QueryResult qr = db.Query("SELECT * FROM cells WHERE cellID = " + cellID + ";");
                    c = _cellFactory.CreateCell(qr.QueryColumnForString(1), X + x, Y + y, name, this);
                    c.Position = i;
                    c.Load(qr);
                    // cells on the left of the board
                    if (i >= wCells && i <= wCells + hCells - 3)
                        c.Angle = 90;
                    // cells in the right of the board
                    else if (i >= 2 * wCells + hCells - 2 && i <= 2 * wCells + 2 * hCells - 5)
                        c.Angle = -90;
                    // the default angle is 0
                    AddCell(c);
                }
            }
            finally
            {
                reader.Close();
            }
        }
        public override void Draw()
        {
            foreach (Cell c in _cells)
                c.Draw();
            // Draw the festival symbol on selected sell
            if (_fesSymbol != null && _fesCell != null)
                SplashKit.DrawBitmapOnWindow(SplashKit.CurrentWindow(), _fesSymbol, _fesCell.X, _fesCell.Y, SplashKit.OptionRotateBmp(_fesCell.Angle));
        }
        public bool IsAt(Point2D point)
        {
            return SplashKit.PointInRectangle(
                point, new Rectangle() { X = X, Y = Y, Width = _width, Height = _height }
            );
        }
        public event EventHandler Clicked;
        public void OnClick(EventArgs e)
        {
            Clicked?.Invoke(this, e);
        }
    }












}
