using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WS_STE
{

    public class Yagmur6EventsMan : EventsMan
    {
        public class Event
        {
            /// <param name="duration">0 for invisible, negative for wait resume, positive for timeout.</param>
            public Event(string name, int duration)
            {
                Name = name;
                Duration = duration;
            }

            public string Name { get; protected set; }
            public int Duration { get; protected set; }
            public bool Invisible { get { return Duration == 0; } }
        }

        public enum Yagmur6Event
        {
            Message, Fix, Sound, Rest, Break, Rating, End
        }

        public const int PracticeSound = 0;
        public const int FirstBlockSound = 1;
        public const int SecondBlockSound = 2;

        int _first, _second, _practice;

        Dictionary<Yagmur6Event, Event> _loaded;

        public Yagmur6EventsMan(int practiceBlockCycles, int firstBlockCycles, int secondBlockCycles, Dictionary<Yagmur6Event, Event> loaded)
        {
            _first = firstBlockCycles;
            _second = secondBlockCycles;
            _practice = practiceBlockCycles;
            _loaded = loaded;
        }

        protected override void Process()
        {
            Instructions(1);
            Second(true);
            Instructions(2);
            First();
            Break(5);
            Instructions(3);
            Second();
            Break(6);
            End();
        }

        private void Do(Yagmur6Event e, int p = -1)
        {
            Trig(e, p);
            if (_loaded.ContainsKey(e))
                base.Sleep(_loaded[e].Duration);
        }

        private void Instructions(int p)
        {
            Do(Yagmur6Event.Message, p);
        }

        private void Break(int d)
        {
            Do(Yagmur6Event.Break, d);
        }

        private void First()
        {
            for (int i = 0; i < _first; i++)
            {
                Do(Yagmur6Event.Fix, FirstBlockSound);
                Do(Yagmur6Event.Sound, FirstBlockSound);
                Do(Yagmur6Event.Rest);
            }
        }

        private void Second(bool practice = false)
        {
            for (int i = 0; i < _second; i++)
            {
                Do(Yagmur6Event.Fix, practice ? PracticeSound : SecondBlockSound);
                Do(Yagmur6Event.Sound, practice ? PracticeSound : SecondBlockSound);
                Do(Yagmur6Event.Rating);
            }
        }

        private new void End()
        {
            Do(Yagmur6Event.End);
            base.End();
        }
    }
}
