using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;

namespace WS_STE
{
    class Rating
    {
        public Image Image { get; private set; }
        public string Name { get; private set; }
        public char Min { get; private set; }
        public char Max { get; private set; }

        public Rating(string text)
        {
            string[] t = text.Split(',');
            if (t.Length != 4)
                throw new ArgumentException("Numero di token errato per il tipo rating (nome,img,min,max)");
            else
            {
                try
                {
                    Name = t[0];
                    Image = Image.FromFile(t[1]);
                    Min = t[2][0];
                    Max = t[3][0];
                }
                catch (FileNotFoundException e)
                {
                    throw new FileNotFoundException("File di immagine non trovato.", e);
                }
                catch (Exception e)
                {
                    throw new ArgumentException("Errore nei dati in input per il rating.", e);
                }
            }
        }

        public Rating(string name, string image, char min, char max)
        {
            Image = Image.FromFile(image);
            Name = name;
            Min = min;
            Max = max;
        }
    }
}
