using System;
using System.Collections.Generic;
using System.Collections;
using SplashKitSDK;

namespace Custom_Program
{
    public class CellFactory
    {
        // A dictionary that holds the types of Cells
        private Dictionary<string, Type> _cellRegistry = new Dictionary<string, Type>();
        // A dictionary that holds the images of Cells that are not Properties
        private Dictionary<string, string> _imgRegistry = new Dictionary<string, string>();
        // Register which cell type that can be created 
        public void RegisterCell(string typeName, Type type)
        {
            _cellRegistry[typeName] = type;
        }
        // Register which bitmap is for which cell type
        public void RegisterImg(string typeName, string filename)
        {
            _imgRegistry[typeName] = filename;
        }
        // Create a new cell from its type name
        public Cell CreateCell(string typeName, float x, float y, string name, Board board)
        {
            Bitmap img = null;
            if (_imgRegistry[typeName] != null)
                img = SplashKit.LoadBitmap(name, _imgRegistry[typeName]);
            return (Cell)Activator.CreateInstance(_cellRegistry[typeName], new Object[] { x, y, name, img, board });
        }
    }
}
