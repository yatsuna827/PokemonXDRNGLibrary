using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGCRNGLibrary
{
    public static class Logger
    {
        static StringBuilder logger = new StringBuilder();
        static public void Clear()
        {
            logger.Clear();
        }
        static public void WriteLog(string log)
        {
            logger.AppendLine(log);
        }
        static public string GetLog()
        {
            return logger.ToString();
        }
    }
}
