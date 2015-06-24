using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cursorr=System.Windows.Forms.Cursor;
using System.Threading;

namespace WS_STE
{
    public partial class MainForm : Form
    {
        // info const
        const int _expectedTexts = 6;

        // std
        Random rnd = new Random();
        // inst
        EventsMan _evt;
        ISoundsPlaylist _sbPractice;
        ISoundsPlaylist _sbFirst;
        ISoundsPlaylist _sbSecond;
        TimeDataFile _blocksFile;
        TimeDataFile _soundsFile;
        TimeDataFile _ratingsFile;
        List<Type> _ratingCsvTypes;
        TimeDataFile _eegFile;
        List<string> _messages;
        List<Rating> _ratings;
        Image _epoCon;
        Image _epoDis;
        Image _batFul;
        Image _batEmp;
        Image _batRed;
        Image _fixCross;
        Image _restCross;
        Image _waitImg;
        List<List<Button>> _ratingsButtons;
        Semaphore nextRatingMutex = new Semaphore(1, 1);
        // info
        DirectoryInfo _saveIn;
        DirectoryInfo _user;
        // stato
        Panel _firstPanel;
        Panel _ap;
        Panel ActivePanel
        {
            get { return _ap; }
            set
            {
                if (_ap != value)
                {
                    if (_ap != null)
                        _ap.Enabled = _ap.Visible = false;
                    _ap = value;
                    value.Enabled = value.Visible = true;
                }
            }
        }
        bool _sc = true;
        bool ShowCursor
        {
            get { return _sc; }
            set 
            {
                if (_sc && !value)
                {
                    Cursor.Hide();
                    _sc = value;
                }
                else if (!_sc && value)
                {
                    Cursor.Show();
                    _sc = value;
                }
            }
        }
        bool _eegEnabled = false;
        bool _practiceRecord = false;
        bool _firstRecord = false;
        bool _secondRecord = false;
        int _currentRating = 0;
        DateTime _lastRatInit = DateTime.Now;

        // ctor
        public MainForm()
        {
            InitializeComponent();
            SuspendLayout();
            FormClosed += MainForm_FormClosed;
            InitializingINIErrorSensitivePart();
            // lay
            panelNotice.Dock = DockStyle.Fill;
            panelFix.Dock = DockStyle.Fill;
            panelSound.Dock = DockStyle.Fill;
            panelRating.Dock = DockStyle.Fill;
            ResumeLayout();
        }

        // Init e INI load
        private void InitializingINIErrorSensitivePart()
        {
            // snds
            LoadSounds("Sounds.Practice", out _sbPractice, Get("Experiment", "sounds.practice.forEachDir", -1));
            LoadSounds("Sounds.First", out _sbFirst, Get("Experiment", "sounds.first.forEachDir", -1));
            LoadSounds("Sounds.Second", out _sbSecond, Get("Experiment", "sounds.second.forEachDir", -1));
            
            // local
            _messages = new List<string>();
            string file = Program._settings.GetValue("Local", "lang");
            try
            {
                if (file != null)
                {
                    _messages.AddRange(File.ReadAllText(file).Split('#'));
                    if (_messages.Count < _expectedTexts)
                        CriticalErrorMessage("Lang file error: some messages missing.\nCheck: must be " + _expectedTexts + " messages separated by a '#' in " + file);
                }
                else
                    CriticalErrorMessage("No lang file set.\nSee [Local] 'lang' in " + Program._settings.SourceFile);
            }
            catch (Exception e)
            { CriticalErrorMessage(String.Format("Error loading lang file:\n{1}\nSee [Local] 'lang' in {0}\n{2}", Program._settings.SourceFile, file, e.Message)); }
            
            // trigs
            Dictionary<Yagmur6EventsMan.Yagmur6Event, Yagmur6EventsMan.Event> trs = new Dictionary<Yagmur6EventsMan.Yagmur6Event, Yagmur6EventsMan.Event>();
            try
            {
                foreach (string el in Program._settings.GetValues("Triggers")) {
                    string[] ab = el.Split(',');
                    string name = ab[0].Trim();
                    int dur = Int32.Parse(ab[1].Trim());
                    trs.Add((Yagmur6EventsMan.Yagmur6Event)Enum.Parse(typeof(Yagmur6EventsMan.Yagmur6Event), name), new Yagmur6EventsMan.Event(name, dur));
                }
            }
            catch (Exception e)
            { CriticalErrorMessage(String.Format("Error reading triggers info:\n{0}\nSee [Triggers] in {1}", e.Message, Program._settings.SourceFile));}
            _evt = new Yagmur6EventsMan(
                _sbPractice.Count,
                _sbFirst.Count,
                _sbSecond.Count,
                trs);
            _evt.Trigs += _evt_Trigs;

            // data
            try
            {
                _saveIn = new DirectoryInfo(Program._settings.GetValue("Data", "folder"));
            }
            catch (Exception e)
            { CriticalErrorMessage(String.Format("Error managing the data folder:\n{0}\nSee [Data] 'folder' in {1}", e.Message, Program._settings.SourceFile)); }
            _practiceRecord = Program._settings.GetValue("Data", "save.practice") == "true";
            _firstRecord = Program._settings.GetValue("Data", "save.first") == "true";
            _secondRecord = Program._settings.GetValue("Data", "save.second") == "true";

            // gui
            if (Program._settings.GetValue("GUI", "colors") == "dark")
            {
                BackColor = Color.FromArgb(32, 32, 32);
                ForeColor = Color.FromName("LightGray");
                panelData.BackColor = Color.FromArgb(20, Color.LightGreen);
            }
            if (Program._settings.GetValue("GUI", "fullScreen") == "true")
                fullscreen();

            // ratings
            _ratings = new List<Rating>();
            _ratingsButtons = new List<List<Button>>();
            bool _showNumbers = Program._settings.GetValue("RatingBlock", "showNumbers") == "true";
            bool _showButtons = Program._settings.GetValue("RatingBlock", "showButtons") == "true";
            foreach (var item in Program._settings.GetValues("Ratings"))
                try
                {
                    Rating r = new Rating(item);
                    _ratings.Add(r);
                    List<Button> l = new List<Button>();
                    for (char i = r.Min; i <= r.Max; i++)
			        {
                        Button b = new Button();
                        b.Visible = b.Enabled = false;
                        b.Name = "buttonRating_" + r.Name + "_" + i;
                        if (_showButtons)
                            b.Tag = i;
                        b.FlatStyle = FlatStyle.Flat;
                        b.FlatAppearance.BorderColor = Color.Black;
                        b.FlatAppearance.BorderSize = 2;
                        b.FlatAppearance.CheckedBackColor = panelRating.BackColor;
                        b.FlatAppearance.MouseOverBackColor = Color.Gray;
                        b.FlatAppearance.MouseDownBackColor = Color.LightGray;
                        if (_showNumbers)
                            b.Text = i.ToString();
                        b.Click += new EventHandler(buttonRating_Click);
                        l.Add(b);
                        panelRating.Controls.Add(b);
			        }
                    _ratingsButtons.Add(l);
                }
                catch (Exception e)
                { CriticalErrorMessage(String.Format("Problem with ratings:\n{0}\nSee [Ratings] in {1}.", e.Message, Program._settings.SourceFile)); }

            // images & lay
            _epoCon = Image.FromFile(Program._settings.GetValue("Img", "epoCon"));
            _epoDis = Image.FromFile(Program._settings.GetValue("Img", "epoDis"));
            if (panelBattery.Enabled = (Program._settings.GetValue("SignalBlock", "showBattery") == "true"))
            {
                _batEmp = Image.FromFile(Program._settings.GetValue("Img", "batEmp"));
                _batFul = Image.FromFile(Program._settings.GetValue("Img", "batFul"));
                _batRed = Image.FromFile(Program._settings.GetValue("Img", "batRed"));

                panelBattery.BackgroundImage = _batFul;
                panelBatCharge.BackgroundImage = _batEmp;
                panelBattery.Size = _batEmp.Size;
                panelBattery.Location = new Point(panelConnect.Width - _batFul.Width, panelBattery.Location.Y);
            }

            try
            {
                _fixCross = 
                    Image.FromFile(Program._settings.GetValue("Img", Program._settings.GetValue("FixBlock", "image", "fixImg")));
                _waitImg = 
                    Image.FromFile(Program._settings.GetValue("Img", Program._settings.GetValue("RatingBlock", "image", "waitImg")));
                _restCross = 
                    Image.FromFile(Program._settings.GetValue("Img", Program._settings.GetValue("RestingBlock", "image", "restImg")));
                panelSound.BackgroundImage = 
                    Image.FromFile(Program._settings.GetValue("Img", Program._settings.GetValue("SoundsBlock", "image", "sndImg")));

                string[] col = Program._settings.GetValue("GUI", "imgBg").Split(',');
                panelRating.BackColor =
                    panelFix.BackColor =
                    panelSound.BackColor = Color.FromArgb(int.Parse(col[0]), int.Parse(col[1]), int.Parse(col[2]));
            }
            catch (Exception e)
            {
                CriticalErrorMessage("Background setting problem: " + e.Message + "\nSee [GUI] 'imgBg' (= r,g,b) and [Img] ... in " + Program._settings.SourceFile);
            }
            _firstPanel = (Program._settings.GetValue("TitleBlock", "show") != "true") ? panelData : panelTitle;
#if DEBUG
            TopMost = false;
#endif
        }

        // aux
        private void fullscreen()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.SetDesktopLocation(0, 0);
            this.Size = SystemInformation.PrimaryMonitorSize;
        }
        private static int Get(string s, string k, int b)
        {
            int a;
            if (!int.TryParse(Program._settings.GetValue(s,k), out a))
                a = b;
            return a;
        }
        private void LoadSounds(string iniSection, out ISoundsPlaylist sbl, int rndFit = -1)
        {
            sbl = new Yagmur6SoundBox();
            List<String> l = Program._settings.GetValues(iniSection);
            if (l == null)
                CriticalErrorMessage(iniSection + " not found in the ini file.");
            foreach (string categoryDir in l)
            {
                try
                {
                    ((Yagmur6SoundBox)sbl).AddDirectory(categoryDir, rndFit);
                }
                catch (Exception e)
                {
                    CriticalErrorMessage(String.Format("Error loading sounds directory:\n{0}\n{1}\n\nSee [{3}] '{2}'", categoryDir, e.Message, Program._settings.SourceFile, iniSection));
                }
            }
        }
        private void SaveMetaStartData()
        {
            // meta save
            string dir = String.Format("{0,4}{1}{2}_{5:00}{6:00}{7:00}_{3}_{4}", DateTime.Now.Year, DateTime.Now.Month.ToString().PadLeft(2, '0'), DateTime.Now.Day.ToString().PadLeft(2, '0'), textBoxSurname.Text, textBoxName.Text, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            _user = new DirectoryInfo(_saveIn.FullName + @"\" + dir);
            while (_user.Exists)
                _user = new DirectoryInfo(_saveIn.FullName + @"\" + dir + String.Format("b"));
            _user.Create();
            // tsSessionCreation, name, surname, date_of_birth, gender, trialCycles, place, notes
            char separator = (char)Get("Data","ordCharSeparator", 45);
            File.WriteAllText(_user.FullName + "/" + Program._settings.GetValue("Data", "meta.f"), String.Format("{0}{8}{1}{8}{2}{8}{3}{8}{4}{8}{5}{8}{6}{8}{7}",
                DateTime.UtcNow.ToUnixTimestamp(),
                textBoxName.Text.Replace("\\", "\\\\").Replace(";", "\\,").Replace("\n", "\\n"),
                textBoxSurname.Text.Replace("\\", "\\\\").Replace(";", "\\,").Replace("\n", "\\n"),
                maskedTextBoxBirthDate.Text,
                radioButtonMale.Checked ? "Male" : radioButtonFemale.Checked ? "Feamle" : "N",
                "---",
                textBoxPlace.Text.Replace("\\", "\\\\").Replace(";", "\\,").Replace("\n", "\\n"),
                textBoxNotes.Text.Replace("\\", "\\\\").Replace(";", "\\,").Replace("\n", "\\n"),
                separator));

            // data files
            string head;
            head = Program._settings.GetValue("Data", "copyNames", "false") != "false" ? Program._settings.GetValue("Data", "eeg.n") + "\n" : "";
            _eegFile = new TimeDataFile(_user.FullName + "\\" + Program._settings.GetValue("Data", "eeg.f"), typeof(EPOC_Data).GetFields().Length, head.Replace(',',separator), separator);
            head = Program._settings.GetValue("Data", "copyNames", "false") != "false" ? Program._settings.GetValue("Data", "blocks.n") + "\n" : "";
            _blocksFile = new TimeDataFile(_user.FullName + "\\" + Program._settings.GetValue("Data", "blocks.f"), new List<Type> { typeof(double), typeof(string), typeof(int) }, head.Replace(',', separator), separator);
            head = Program._settings.GetValue("Data", "copyNames", "false") != "false" ? Program._settings.GetValue("Data", "sounds.n") + "\n" : "";
            _soundsFile = new TimeDataFile(_user.FullName + "\\" + Program._settings.GetValue("Data", "sounds.f"), new List<Type> { typeof(double), typeof(string) }, head.Replace(',',separator), separator);
            head = Program._settings.GetValue("Data", "copyNames", "false") != "false" ? Program._settings.GetValue("Data", "ratings.n") + "\n" : "";
            _ratingCsvTypes = new List<Type> { typeof(double), typeof(double), typeof(char) };
            _ratingsFile = new TimeDataFile(_user.FullName + "\\" + Program._settings.GetValue("Data", "ratings.f"), new List<Type> { typeof(double), typeof(char) }, head.Replace(',', separator), separator);
        }
        private bool CheckTextBox(TextBoxBase textBox, Predicate<string> l = null)
        {
            textBox.Text = textBox.Text.Trim();
            l = l ?? (a => a.Length >= 3);
            if (l(textBox.Text))
            {
                textBox.BackColor = SystemColors.Window;
                return true;
            }
            else
            {
                textBox.BackColor = Color.LightSalmon;
                return false;
            }
        }
        public static void CriticalErrorMessage(string error, string cap = "SST " + Program.short_version + " Error")
        {
#if DEBUG
            throw new Exception(error);
#else
            if (System.Windows.Forms.DialogResult.Yes == MessageBox.Show(error + "\n\nStop the program?", cap, MessageBoxButtons.YesNo, MessageBoxIcon.Error))
                System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
        }

        // core
        private void SetupNewExperiment()
        {
            ActivePanel = panelData;
            _sbFirst.Shuffle();
            _sbSecond.Shuffle();
            _sbPractice.Shuffle();
            _sbFirst.LoadNext();
            _sbSecond.LoadNext();
            _sbPractice.LoadNext();
        }
        private string Trigger(EventsMan sender, object trigger, int d1, int d2)
        {
            if (! trigger.GetType().Equals(typeof(Yagmur6EventsMan.Yagmur6Event)))
                throw new NotImplementedException("Method implemented only for " + typeof(Yagmur6EventsMan.Yagmur6Event).FullName);

            _blocksFile.AddTimestamp();
            _blocksFile.AddValue(trigger.ToString());
            _blocksFile.AddValue(d1);

            Console.WriteLine(trigger.ToString() + (d1 >= 0 ? (":" + d1) : ""));

            switch ((Yagmur6EventsMan.Yagmur6Event)trigger)
            {
                case Yagmur6EventsMan.Yagmur6Event.Message:
                    ShowNotice(_messages[d1]);
                    break;
                case Yagmur6EventsMan.Yagmur6Event.Fix:
                    _eegEnabled = 
                        (_practiceRecord && d1 == Yagmur6EventsMan.PracticeSound) || 
                        (_firstRecord && d1 == Yagmur6EventsMan.FirstBlockSound) ||
                        (_secondRecord && d1 == Yagmur6EventsMan.SecondBlockSound);
                    ShowFix();
                    break;
                case Yagmur6EventsMan.Yagmur6Event.Sound:
                    ActivePanel = panelSound;
                    ISoundsPlaylist snd;
                    switch (d1)
                    {
                        case Yagmur6EventsMan.PracticeSound:
                            snd = _sbPractice;
                            break;
                        case Yagmur6EventsMan.FirstBlockSound:
                            snd = _sbFirst;
                            break;
                        case Yagmur6EventsMan.SecondBlockSound:
                            snd = _sbSecond;
                            break;
                        default:
                            CriticalErrorMessage("Internal error: FndME-94rh24rh2\nUsing Practice sounds.");
                            snd = _sbPractice;
                            break;
	                }
                    snd.PlayNext();
                    if ((d1 == Yagmur6EventsMan.FirstBlockSound && _firstRecord) ||
                        (d1 == Yagmur6EventsMan.SecondBlockSound && _secondRecord) ||
                        (d1 == Yagmur6EventsMan.PracticeSound && _practiceRecord))
                    {
                        _soundsFile.AddTimestamp();
                        _soundsFile.AddValue(snd.LastLoaded);
                    }
                    break;
                case Yagmur6EventsMan.Yagmur6Event.Rest:
                    ShowNotice(_messages[/*BLK-REST*/4]);
                    break;
                case Yagmur6EventsMan.Yagmur6Event.Break:
                    ShowNotice(_messages[d1]);
                    break;
                case Yagmur6EventsMan.Yagmur6Event.Rating:
                    ActivePanel = panelRating;
                    NextRating();
                    break;
                case Yagmur6EventsMan.Yagmur6Event.End:
                    SetupNewExperiment();
                    break;
                default:
                    break;
            }
            return default(string);
        }
        private bool NextRating()
        {
            foreach (var item in _ratingsButtons)
                if (item[0].Enabled)
                    item.ForEach(b => b.Enabled = b.Visible = false);

            // if x set else next
            if (_currentRating < _ratings.Count)
            {
                panelRating.BackgroundImage = _ratings[_currentRating].Image;
                for (int b = 0; b < _ratingsButtons[_currentRating].Count; b++)
                {
                    int cw, iw, bn;
                    cw = ClientSize.Width;
                    iw = panelRating.BackgroundImage.Size.Width;
                    bn = _ratingsButtons[_currentRating].Count;
                    Button a = _ratingsButtons[_currentRating][b];
                    a.Location = new Point((cw - iw) / 2 + iw * (b + 1) / (bn + 1) - iw / (bn + 5) / 2, (ClientSize.Height + panelRating.BackgroundImage.Height) / 2 + 10);
                    a.Size = new Size(iw / (bn + 5), iw / (bn + 5));
                    a.Enabled = true;
                    a.Visible = a.Tag != null;
                }
                timerRating.Interval = _ratings[_currentRating].Duration;
                timerRating.Enabled = true;
                _blocksFile.AddTimestamp();
                _blocksFile.AddValue("NextScale");
                _blocksFile.AddValue(_currentRating);
                _currentRating++;
                return true;
            }
            else
            {
                _currentRating = 0;
                if (ActivePanel == panelRating)
                    ShowWait();
                return false;
            }
        }
        private void ShowWait()
        {
            panelFix.BackgroundImage = _waitImg;
            ActivePanel = panelFix;
        }
        private void ShowRest()
        {
            panelFix.BackgroundImage = _restCross;
            ActivePanel = panelFix;
        }
        private void ShowFix()
        {
            panelFix.BackgroundImage = _fixCross;
            ActivePanel = panelFix;
        }
        private void ShowNotice(string p)
        {
            labelNotice.Text = p;
            ActivePanel = panelNotice;
        }

        // interth evt wrapper
        string _evt_Trigs(EventsMan sender, object trigger, int v1 = -1, int v2 = -1) {
            try
            {
                return (string)Invoke(new EventsMan.TriggerEventHandler(_trigTest_Trigger), new object[] {sender, trigger, v1, v2});
            }
            catch (InvalidOperationException)
            {
                return "error";
            }
        }
        string _trigTest_Trigger(EventsMan sender, object trigger, int d1, int d2) { return Trigger(sender, trigger, d1, d2); }
        void AEpoc_SensorsDataAvailable(DateTime ts, EPOC_Data data) { Invoke(new EpocCallback(Epoc_SensorsDataAvailable), ts, data); }
        void AEpoc_QualityDataAvailable(DateTime ts, EPOC_Data data) { Invoke(new EpocCallback(Epoc_QualityDataAvailable), ts, data); }
        void AEpoc_ConnectedChanged(DateTime ts, bool connected) { Invoke(new EpocConnectionCallback(Epoc_ConnectedChanged), ts, connected); }
        
        // evt gen
        private void MainForm_Load(object sender, EventArgs e)
        {
            ActivePanel = _firstPanel;
            panelImgConn.BackgroundImage = _epoDis;
            OnSizeChanged(null);
        }
        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            panelTitle.Location = new Point((Size.Width - panelTitle.Size.Width) / 2, (Size.Height - panelTitle.Size.Height) / 2);
            panelData.Location = new Point((Size.Width - panelData.Size.Width) / 2, (Size.Height - panelData.Size.Height) / 2);
            panelConnect.Location = new Point((Size.Width - panelConnect.Size.Width) / 2, (Size.Height - panelConnect.Size.Height) / 2);
        }
        private void Epoc_SensorsDataAvailable(DateTime ts, EPOC_Data data)
        {
            if (_eegEnabled)
                _eegFile.FullTextRecord(data.GyroX, data.GyroY, data.F3Data, data.FC5Data, data.AF3Data, data.F7Data, data.T7Data, data.P7Data, data.O1Data, data.O2Data, data.P8Data, data.T8Data, data.F8Data, data.AF4Data, data.FC6Data, data.F4Data, data.packetC);
        }
        private void Epoc_QualityDataAvailable(DateTime ts, EPOC_Data data)
        {
            int min = 200, med = 0;
            var t = data.GetType().GetFields();
            foreach (var item in t)
            {
                if (item.Name != "packetC")
                {
                    int a = (int)item.GetValue(data);
                    med += a;
                    if (min > a)
                        min = a;
                }
            }
            med /= t.Length;
            labelQuality.Text = min < 0 ? "Supporto disconnesso" : "Min: " + min + "% " + "Avg: " + min + "%";
        }
        private void Epoc_ConnectedChanged(DateTime ts, bool connected)
        {
            panelImgConn.BackgroundImage = connected ? _epoCon : _epoDis;
            panelBattery.Visible = panelBattery.Enabled && connected;
        }
        private void Application_Idle(object sender, EventArgs e)
        {
            if (ActivePanel == panelConnect)
                Epoc.AskQuality();
        }
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_evt != null)
            {
                _evt.Close();
                _evt = null;
            }
        }

        // layout chg
        private void panelData_EnabledChanged(object sender, EventArgs e)
        {
            if (((Control)sender).Enabled)
            {
                textBoxPlace.Text = Program._settings.GetValue("MetaBlock", "defaultPlace");
                textBoxNotes.Text = Program._settings.GetValue("MetaBlock", "defaultNotes");
                textBoxName.Clear();
                textBoxSurname.Clear();
                this.AcceptButton = buttonNext;
                maskedTextBoxBirthDate.Clear();
                radioButtonFemale.Checked = radioButtonMale.Checked = false;
                textBoxName.Focus();
                ShowCursor = true;
            }
            else
            {
                this.AcceptButton = null;
            }
        }
        private void panelStart_EnabledChanged(object sender, EventArgs e)
        {
            if (((Control)sender).Enabled)
            {
                this.AcceptButton = buttonNew;
                ShowCursor = true;
            }
            else
            {
                this.AcceptButton = null;
            }
        }
        private void panelNotice_EnabledChanged(object sender, EventArgs e)
        {
            if (((Control)sender).Enabled)
            {
                this.Focus();
                ShowCursor = false;
            }
        }
        private void panelRating_EnabledChanged(object sender, EventArgs e)
        {
            if (((Control)sender).Enabled)
            {
                this.Focus();
                ShowCursor = true;
            }
            else
            {
                ((Control)sender).BackgroundImage = null;
            }
        }
        private void panelConnect_EnabledChanged(object sender, EventArgs e)
        {
            if (((Control)sender).Enabled)
            {
                this.Focus();
                ShowCursor = true;
                if (panelBattery.Enabled)
                    panelBatCharge.Height = _batFul.Height * 100 / (Epoc.BatteryCharge + 1);
                if (!Epoc.Created)
                    Epoc.Create();

                // evt link
                Epoc.ConnectedChanged += new EpocConnectionCallback(AEpoc_ConnectedChanged);
                Epoc.SensorsDataAvailable += new EpocCallback(AEpoc_SensorsDataAvailable);
                Epoc.QualityDataAvailable += new EpocCallback(AEpoc_QualityDataAvailable);

                Application.Idle += new EventHandler(Application_Idle);
            }
            else
            {
                Application.Idle -= new EventHandler(Application_Idle);
            }
        }

        // btns
        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void buttonNew_Click(object sender, EventArgs e)
        {
            SetupNewExperiment();
        }
        private void buttonNext_Click(object sender, EventArgs e)
        {
#if !DEBUG
            DateTime res;
            if (CheckTextBox(textBoxName) &&
                CheckTextBox(textBoxSurname) &&
                CheckTextBox(maskedTextBoxBirthDate, a => DateTime.TryParse(a, out res)) &&
                (radioButtonMale.Checked || radioButtonFemale.Checked) &&
                CheckTextBox(textBoxPlace))
#endif
            {
                SaveMetaStartData();
                if (Program._settings.GetValue("SignalBlock", "show") == "true")
                    ActivePanel = panelConnect;
                else
                    _evt.Start();
            }
        }
        private void buttonRating_Click(object sender, EventArgs e)
        {
            int cr = _currentRating;
            nextRatingMutex.WaitOne();
            if (cr == _currentRating)
                MainForm_KeyPress(this, new KeyPressEventArgs((char)((sender as Button).Tag)));
            nextRatingMutex.Release();
        }

        // keys
        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (ActivePanel == panelNotice)
            {
                if (e.KeyChar == ' ' && !timerRating.Enabled)
                    if (_evt.CurrentEvent.Equals(Yagmur6EventsMan.Yagmur6Event.Break))
                        _evt.Resume(Yagmur6EventsMan.Yagmur6Event.Break);
                    else if (_evt.CurrentEvent.Equals(Yagmur6EventsMan.Yagmur6Event.End))
                        _evt.Resume(Yagmur6EventsMan.Yagmur6Event.End);
                    else
                    {
                        // WAS Wanna show wait? try waiting here
                        // ShowWaiting();
                        _evt.Resume(Yagmur6EventsMan.Yagmur6Event.Message);
                    }
            }
            else if (ActivePanel == panelRating)
            {
                if (_currentRating > 0 &&_currentRating <= _ratings.Count && e.KeyChar >= _ratings[_currentRating - 1].Min && e.KeyChar <= _ratings[_currentRating - 1].Max)
                {
                    timerRating.Enabled = false;
                    _ratingsFile.AddTimestamp();
                    _ratingsFile.AddValue(e.KeyChar);
                    NextRating();
                }
            }
            else if (ActivePanel == panelConnect && e.KeyChar == ' ')
            {
                _evt.Start();
            }
        }

        // timing
        private void timerSpacebar_Tick(object sender, EventArgs e)
        {
            timerRating.Enabled = false;
            int cr = _currentRating;
            nextRatingMutex.WaitOne();
            if (cr == _currentRating)
                NextRating();
            nextRatingMutex.Release();
        }
    }
}
