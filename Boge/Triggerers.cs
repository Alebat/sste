using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace WS_STE
{
    public delegate string TriggerEventHandler(Triggerer sender, TriggerInfo meta, int ciclo, int sessione);

    /// <summary>
    /// Informazioni sul trigger.
    /// </summary>
    public struct TriggerInfo
    {
        public string InternalName;
        /// <summary>
        /// Nome trigger
        /// </summary>
        public string Name;
        /// <summary>
        /// Durata minima trigger o durata effettiva se in output.
        /// </summary>
        public int Duration;
        /// <summary>
        /// Durata random additiva o 0 se in output.
        /// </summary>
        public int RandomDuration;

        public TriggerInfo(string text)
        {
            string[] s = text.Split(',');
            try
            {
                InternalName = s[0];
                Name = InternalName;
                Duration = int.Parse(s[1]);
                if (s.Length > 2)
                    RandomDuration = int.Parse(s[2]);
                else
                    RandomDuration = 0;
            }
            catch
            {
                throw new FormatException("Il formato testuale in input del trigger è errato, controllare o ripristinare a default.\nTextual input trigger format fault, check or reset it.");
            }
        }

        public TriggerInfo(string inn, string n, int d, int rd)
        {
            InternalName = inn;
            Name = n;
            Duration = d;
            RandomDuration = rd;
        }

        public override string ToString()
        {
            return String.Format("{0},{1},{2}", Name, Duration, RandomDuration);
        }
    }

    /// <summary>
    /// Gestisce gli eventi per il triggering.
    /// </summary>
    public class Triggerer : IDisposable
    {
        volatile bool _working;
        Semaphore _pausa = new Semaphore(0, 1);
        volatile bool _inPausa = false;
        volatile bool _ultimoCiclo = false;
        volatile int _repeatLast = 0;
        volatile List<TriggerInfo> _triggers = new List<TriggerInfo>();
        volatile object _triggersLock = new object();
        volatile int _doPauseTill;
        volatile int _doPauseTillB;
        Thread _worker;
        Random _rnd = new Random();
        TriggerInfo _tpausa = new TriggerInfo("GeneralBreak", "-1", -2, 0);
        TriggerInfo _tfine = new TriggerInfo("GeneralFinish", "-2", 0, 0);

        private void Work()
        {
            Finish = false;
            int t = -2, c = -2, s = -2;
            try
            {
                DateTime inizio = DateTime.Now;
                // for: t, cyclic: c, execs: s
                for (t = 1, c = 1, s = 1; t <= CicliTotali && !_ultimoCiclo; t++, c++)
                {
                    if ((c > CicliMaxPerPausa || (DateTime.Now - inizio).TotalSeconds >= TempoMassimoPerPausa))
                    {
                        ExecTrigger(t, s++, _tpausa);
                        c = 1;
                        inizio = DateTime.Now;
                    }
                    lock (_triggersLock)
                    {
                        for (int i = 0; i < _triggers.Count; i++)
                        {
                            ExecTrigger(t, s, _triggers[i]);
                            if (_repeatLast != 0)
                            {
                                i -= _repeatLast;
                                _repeatLast = 0;
                            }
                            if ((_doPauseTillB * 10000000L + _doPauseTill) > DateTime.Now.ToUnixTimestamp() * 1000)
                            {
                                _inPausa = true;
                                _pausa.WaitOne((int)((_doPauseTillB * 10000000L + _doPauseTill) - DateTime.Now.ToUnixTimestamp() * 1000));
                                _inPausa = false;
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException) { }
            Finish = true;
            ExecTrigger(t, s, _tfine);
        }

        /// <summary>
        /// Executes the trigger, calculates the duration and waits.
        /// Negative duration to pause the thread.
        /// </summary>
        private string ExecTrigger(int rep, int ses, TriggerInfo el)
        {
            string res = OnTrigger(el, rep, ses);
            int tot = el.Duration + _rnd.Next(el.RandomDuration);
            if (el.Duration < 0)
            {
                _inPausa = true;
                _pausa.WaitOne();
            }
            else
                _pausa.WaitOne(tot);
            return res;
        }

        public bool Finish { get; private set; }

        public Triggerer()
        {
            
        }

        /// <summary>
        /// Imposta il triggerer con triggers, ripetizioni e sessioni di ripetizioni.
        /// </summary>
        public Triggerer(int cicliMaxperPausa, int tempoMaxRipetizioni, int cicliMaxTot, int durataPausa)
            : this(null, cicliMaxperPausa, tempoMaxRipetizioni, cicliMaxTot, durataPausa)
        {

        }

        /// <summary>
        /// Imposta il triggerer con triggers, ripetizioni e sessioni di ripetizioni.
        /// </summary>
        public Triggerer(IEnumerable<TriggerInfo> triggers, int cicliMaxperPausa, int tempoMaxRipetizioni, int cicliMaxTot, int durataPausa)
        {
            if (triggers != null)
                _triggers = new List<TriggerInfo>(triggers);
            CicliMaxPerPausa = cicliMaxperPausa;
            CicliTotali = cicliMaxTot;
            TempoMassimoPerPausa = tempoMaxRipetizioni;
            _tpausa.Duration = durataPausa;
            Finish = false;
        }

        /// <summary>
        /// Numero delle ripetizioni
        /// </summary>
        public int CicliMaxPerPausa { get; private set; }

        /// <summary>
        /// Tempo massimo sessione in secondi
        /// </summary>
        public int TempoMassimoPerPausa { get; private set; }

        /// <summary>
        /// Numero delle sessioni
        /// </summary>
        public int CicliTotali { get; private set; }

        /// <summary>
        /// Indica se è attiva una pausa
        /// </summary>
        public bool InBreak
        {
            get { return _inPausa; }
        }

        /// <summary>
        /// Riprende il ciclo se è in pausa.
        /// </summary>
        public void Resume()
        {
            _inPausa = false;
            _pausa.Release();
        }

        public void TriggersAdd(TriggerInfo el)
        {
            lock (_triggersLock)
                _triggers.Add(el);
        }

        public void TriggersAddRange(IEnumerable<TriggerInfo> els)
        {
            lock (_triggersLock)
                _triggers.AddRange(els);
        }

        /// <summary>
        /// Copia in sola lettura dei triggers
        /// </summary>
        public List<TriggerInfo> Triggers
        {
            get
            {
                lock (_triggers)
                {
                    return new List<TriggerInfo>(_triggers);
                }
            }
        }

        /// <summary>
        /// Avvia il ciclo in modo asincrono (o sincrono).
        /// </summary>
        /// <param name="blocking"></param>
        public void Start(bool blocking = false)
        {
            if (blocking)
                Work();
            else
            {
                _worker = new Thread(new ThreadStart(Work));
                _worker.IsBackground = true;
                _worker.Start();
            }
        }
        
        /// <summary>
        /// Arresta definitivamente il ciclo di triggering attendendo la fine del ciclo.
        /// </summary>
        public void Reset()
        {
            _ultimoCiclo = false;
            if (_worker != null)
            {
                if (_worker.ThreadState.HasFlag(ThreadState.Suspended))
                    _worker.Resume();
                _worker.Abort();
            }
        }

        /// <summary>
        /// Fa in modo che quello in esecuzione sia l'ultimo ciclo di triggers.
        /// </summary>
        public void AskStop()
        {
            if (_worker != null && _worker.ThreadState != ThreadState.Stopped)
                _ultimoCiclo = true;
        }

        /// <summary>
        /// Fa in modo che il trigger in esecuzione venga ripetuto (>0) o saltato (<0).
        /// </summary>
        public void AskRepeat(int triggers)
        {
            if (_worker != null && _worker.ThreadState != ThreadState.Stopped)
                _repeatLast = triggers;
        }

        /// <summary>
        /// Fa in modo che prima del prossimo trigger venga inserita una pausa.
        /// </summary>
        public void AskWait(DateTime later)
        {
            if (_worker != null && _worker.ThreadState != ThreadState.Stopped)
            {
                long t = (long)(later.ToUnixTimestamp() * 1000);
                _doPauseTill = (int)(t % 10000000);
                _doPauseTillB = (int)(t / 10000000);
            }
        }

        /// <summary>
        /// Scatena un evento trigger.
        /// </summary>
        public string OnTrigger(TriggerInfo meta, int rep, int ses)
        {
            if (Trigger != null)
                return Trigger.Invoke(this, meta, rep, ses);
            else
                return null;
        }

        /// <summary>
        /// Evento trigger.
        /// </summary>
        public event TriggerEventHandler Trigger;

        /// <summary>
        /// Ferma il thread sottostante senza dire baf.
        /// </summary>
        ~Triggerer()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            Reset();
        }
    }
}
