using System;
using System.Media;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WS_STE
{
    public interface ISoundsPlaylist
    {
        /// <summary>
        /// Numero di suoni (?rimanenti).
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Ultimo suono caricato.
        /// </summary>
        string LastLoaded { get; }

        /// <summary>
        /// Dispone casualmente i suoni.
        /// </summary>
        void Shuffle();

        /// <summary>
        /// Carica un suono e lo toglie dalla lista.
        /// </summary>
        bool HasNext();

        /// <summary>
        /// Esegue il prossimo suono.
        /// </summary>
        void PlayNext();

        /// <summary>
        /// Carica il prossimo suono.
        /// </summary>
        void LoadNext();
    }
}
