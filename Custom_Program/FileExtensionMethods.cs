﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Custom_Program
{
    /// <summary>
    /// Extension for StreamReader methods
    /// </summary>
    public static class FileExtensionMethods
    {
        public static int ReadInteger(this StreamReader reader)
        {
            return Convert.ToInt32(reader.ReadLine());
        }
        public static float ReadFloat(this StreamReader reader)
        {
            return Convert.ToSingle(reader.ReadLine());
        }
    }
}
