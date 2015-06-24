using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WS_STE
{
    class TimeDataFile
    {
        StreamWriter _file;
        public string File { get; private set; }
        public int TotalChannels { get; private set; }
        public char Separator { get; private set; }
        public object EmptyObject { get; set; }

        public bool TypeCheck { get; set; }
        public int NextChannel { get; private set; }
        public List<Type> Channels { get; private set; }
        
        public TimeDataFile(string file, int channels, string header, char separator)
        {
            _file = new StreamWriter(File = file);
            TotalChannels = channels;
            Separator = separator;
            TypeCheck = false;
            _file.Write(header);
            EmptyObject = "None";
        }

        public TimeDataFile(string file, List<Type> chans, string header, char separator)
        {
            _file = new StreamWriter(File = file);
            TotalChannels = chans.Count;
            Separator = separator;
            Channels = new List<Type>(chans);
            TypeCheck = true;
            _file.Write(header);
            EmptyObject = "None";
        }

        public void FullTextRecord(params object[] data)
        {
            if (TypeCheck)
                throw new InvalidOperationException("TypeCheck attivo.");
            if (data.Length != TotalChannels)
                throw new ArgumentException(String.Format("Numero errato {0}/{1} di argomenti.", data.Length, TotalChannels));
            else
            {
                _file.Write("{0}", DateTime.UtcNow.ToUnixTimestamp());
                foreach (object el in data)
                    _file.Write("{1}{0}", el, Separator);
                Endline();
            }
        }

        public void AddValue(object val)
        {
            val = val ?? EmptyObject;
            if (TypeCheck)
            {
                Type t = val.GetType();
                if (!t.IsSubclassOf(Channels[NextChannel]) && !Object.ReferenceEquals(t, Channels[NextChannel]))
                    throw new ArgumentException(String.Format("Tipo errato per l'argomento. Richiesto: {0}, fornito {1}.", Channels[NextChannel].FullName, t.FullName));
            }
            _file.Write((NextChannel == 0) ? "{0}" : "{1}{0}", val, Separator);
            NextChannel = (NextChannel + 1) % TotalChannels;
            if (NextChannel == 0)
                Endline();
        }

        public void AddTimestamp()
        {
            AddValue(DateTime.UtcNow.ToUnixTimestamp());
        }

        public void Endline()
        {
            if (NextChannel != 0)
            {
                while (NextChannel != 0)
                {
                    _file.Write((NextChannel == 0) ? "{0}" : "{1}{0}", EmptyObject, Separator);
                    NextChannel = (NextChannel + 1) % TotalChannels;
                }
            }
            _file.WriteLine();
            _file.Flush();
        }

        ~TimeDataFile()
        {
            try
            {
                _file.Close();
            }
            catch
            {
            }
        }
    }
}
