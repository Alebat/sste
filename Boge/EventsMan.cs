using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace WS_STE
{
    /// <summary>
    /// Gestisce gli eventi per il triggering.
    /// </summary>
    public abstract class EventsMan
    {
        public delegate string TriggerEventHandler(EventsMan sender, object trigger, int v1 = -1, int v2 = -1);

        volatile bool _started = false;
        volatile bool _ended = false;
        Thread _process = null;
        SemaphoreSlim _sem = new SemaphoreSlim(0, 1);

        public event TriggerEventHandler Trigs;

        public object CurrentEvent { get; protected set; }

        ~EventsMan()
        {
            Close();
        }

        protected string Trig(object evt, int t = -1)
        {
            CurrentEvent = evt;
            if (Trigs != null)
                return Trigs.Invoke(this, evt, t, -1);
            return null;
        }

        protected void Sleep(int millis = -1)
        {
            if (millis < 0)
                _sem.Wait();
            else
                _sem.Wait(millis);
        }

        internal void End()
        {
            _ended = true;
        }

        public void Reset()
        {
            Close();
            _process = new Thread(new ThreadStart(Process));
            _started = false;
            _ended = false;
        }

        public void Start()
        {
            if (!_started)
            {
                if (_process == null)
                    Reset();
                _process.Start();
                _started = true;
            }
        }

        public void Close()
        {
            if (_process != null)
            {
                if (_process.IsAlive)
                    _process.Abort();
                _process = null;
            }
        }

        public void Resume(object controlCurrentEvent)
        {
            if (CurrentEvent.Equals(controlCurrentEvent))
                _sem.Release();
        }

        protected abstract void Process();
    }
}
