using System;
using System.Media;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WS_STE
{
    /// <summary>
    /// Gestisce i suoni, la loro esecuzione e l'ordinamento.
    /// </summary>
    class SoundBox
    {
        Random _rnd = new Random();
        SoundPlayer _next;
        List<KeyValuePair<string, List<string>>> _folders;
        string _lastSnd;
        int _cf = 0;
        int _cs = 0;

        public int Count { get; set; }

        /// <summary>
        /// Ctor master
        /// </summary>
        public SoundBox()
        {
            //                  folder -> file
            //                  key    -> Value
            //             List string -> list string
            //                                 string
            //
            //                  string -> list string
            //                                 string
            _folders = new List<KeyValuePair<string, List<string>>>();
        }

        /// <summary>
        /// Aggiunge i suoni da un elenco.
        /// </summary>
        /// <param name="folders">Percorsi da aggiungere.</param>
        public void AddFolder(List<string> folders, string fol = ".")
        {
            // Se le contiene già
            if (_folders.FindAll(a => fol == a.Key).Count == 0)
                _folders.Add(new KeyValuePair<string, List<string>>(fol, folders));
            else
                _folders.Find(a => a.Key == fol).Value.AddRange(folders);
        }

        /// <summary>
        /// Ripristina i suoni
        /// </summary>
        public void Shuffle()
        {
            Comparison<KeyValuePair<string, List<string>>> c;
            // Only the first part equal => the same category
            c = (a, b) => a.Key.Split('>')[0] == b.Key.Split('>')[0] ? 0 : 1;
            _folders.Shuffle(c);
            _cf = 0;
            _cs = 0;
        }

        /// <summary>
        /// Carica un suono e lo toglie dalla lista.
        /// False se i suoni sono finiti nella dir, null se sono finite le dir.
        /// </summary>
        /// <returns></returns>
        public bool? LoadNext()
        {
            if (_cf < _folders.Count) // ci sono cartelle
                if (_cs < _folders[_cf].Value.Count) // ci sono files nella cartella
                {
                    _lastSnd = _folders[_cf].Value[_cs];
                    _next = new SoundPlayer(_lastSnd);
                    _next.LoadAsync();
                    _cs++;
                    return true;
                }
                else
                // finiti i suoni
                {
                    _cf++;
                    _cs = 0;
                    if (_cf < _folders.Count)
                        // finiti solo i suoni
                        return false;
                    else
                        // finite anche le cartelle
                        return null;
                }
            // finite le cartelle
            else
                _cf = 0;
                return null; // finite le cartelle
        }

        /// <summary>
        /// Esegue il prossimo suono.
        /// </summary>
        public void Play()
        {
            _next.Play();
        }

        /// <summary>
        /// Cartelle rimanenti.
        /// </summary>
        public int RemainingDirs
        {
            get
            {
                return _folders.Count - _cf;
            }
        }

        /// <summary>
        /// Suoni rimanenti.
        /// </summary>
        public int RemainingFiles
        {
            get
            {
                if (_cf < _folders.Count)
                    return _folders[_cf].Value.Count;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Ultimo suono caricato.
        /// </summary>
        public string LastLoaded
        {
            get
            {
                return _lastSnd;
            }
        }
    }
}
