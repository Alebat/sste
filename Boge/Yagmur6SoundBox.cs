using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;

namespace WS_STE
{
    class Yagmur6SoundBox : ISoundsPlaylist
    {
        Random _rnd = new Random();
        SoundPlayer _next;
        int _nextInd;
        List<string> _files = new List<string>();
        bool _loaded = false;

        public void AddDirectory(string dir, int rndFit = -1)
        {
            if (Directory.Exists(dir))
            {
                List<string> l = new List<string>(Directory.EnumerateFiles(dir));
                int x = rndFit / l.Count + 1;
                List<string> f = new List<string>(x * l.Count);
                if (l.Count > 0)
                    if (rndFit >= 0)
                    {
                        for (int i = 0; i < x; i++)
                            f.AddRange(new List<string>(l));
                        f.Shuffle();
                        x = l.Count - (rndFit % l.Count);
                        f.RemoveRange(f.Count - x, x);
                    }
                    else
                        f.AddRange(l);
                _files.AddRange(f);
                _nextInd = _files.Count;
            }
            else
                throw new DirectoryNotFoundException(dir + " (expanded " + Path.GetFullPath(dir) + ") seems not to exist.");
        }

        public bool HasNext()
        {
            return Count > 0;
        }

        public void LoadNext()
        {
            if (!_loaded)
                if (_nextInd > 0)
                {
                    _next = new SoundPlayer(_files[--_nextInd]);
                    _next.LoadAsync();
                    _loaded = true;
                }
        }

        public void PlayNext()
        {
            LoadNext();
            _next.Play();
            _loaded = false;
            LoadNext();
        }

        public int Count
        {
            get { return _nextInd; }
        }

        public string LastLoaded
        {
            get { return _loaded ? _next.SoundLocation : null; }
        }

        public void Shuffle()
        {
            _nextInd = _files.Count;
            _files.Shuffle((a,b) => a.CompareTo(b));
        }
    }
}
