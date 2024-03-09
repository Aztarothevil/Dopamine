using Prism.Mvvm;
using System;

namespace Dopamine.Services.Lyrics
{
    public class LyricsLineViewModel : BindableBase
    {
        private TimeSpan time;
        private string text;
        private bool isTimed;
        private bool isHighlighted;
        private int index;
        private double duration;

        public TimeSpan Time
        {
            get { return this.time; }
        }

        public string Text
        {
            get { return this.text; }
        }

        public bool IsTimed
        {
            get { return this.isTimed; }
        }

        public bool IsHighlighted
        {
            get { return this.isTimed & this.isHighlighted; }
            set { SetProperty<bool>(ref this.isHighlighted, value); }
        }

        public int Index
        {
            get { return this.index; }
        }

        public double Duration
        {
            get { return this.duration; }
            set { duration = value; }
        }

        public LyricsLineViewModel(TimeSpan time, string text, int index)
        {
            this.time = time;
            this.text = text;
            this.index = index;
            this.isTimed = true;
        }

        public LyricsLineViewModel(string text)
        {
            this.time = TimeSpan.Zero;
            this.text = text;
            this.index = 0;
            this.isTimed = false;
            this.duration = 0;
        }
    }
}