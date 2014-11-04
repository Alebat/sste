using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cursorr=System.Windows.Forms.Cursor;

namespace WS_STE
{
    public partial class MainForm : Form
    {
        // info const
        const int _expectedTexts = 6;

        // tipi aux
        enum ExperimentState { Setup, Start, PreTrial, Trial, PreTest,PreTestRest, Break, Test, Idle }

        // std
        Random rnd = new Random();
        // inst
        Triggerer _trigTrial;
        Triggerer _trigTest;
        SoundBox _sbTrial;
        SoundBox _sbTest;
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
        Image _fixDot;
        Image _waitDot;
        Image _restImg;
        List<List<Button>> _ratingsButtons;
        // info
        DirectoryInfo _saveIn;
        DirectoryInfo _user;
        // stato
        ExperimentState _state = ExperimentState.Setup;
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
        bool _recordTrial = false;
        int _currentRating = 0;
        int _repeatExperimentTimes = 1;
        int _repeatedExperimentTimes = 0;
        int _ciclo = 0;
        DateTime _lastRatInit = DateTime.Now;

        // ctor
        public MainForm()
        {
            InitializeComponent();
            SuspendLayout();
            InitializingINIErrorSensitivePart();
            // lay
            panelNotice.Dock = DockStyle.Fill;
            panelFix.Dock = DockStyle.Fill;
            panelSound.Dock = DockStyle.Fill;
            panelRating.Dock = DockStyle.Fill;
            // evt link
            Epoc.ConnectedChanged += new EpocConnectionCallback(AEpoc_ConnectedChanged);
            Epoc.QualityDataAvailable += new EpocCallback(AEpoc_QualityDataAvailable);
            Epoc.SensorsDataAvailable += new EpocCallback(AEpoc_SensorsDataAvailable);
            ResumeLayout();
        }

        // Init e INI load
        private void InitializingINIErrorSensitivePart()
        {
            // snds
            LoadSounds("Sounds.Trial", out _sbTrial);
            LoadSounds("Sounds.Test", out _sbTest);
            
            // local
            _messages = new List<string>();
            string file = Program._settings.GetValue("Local", "lang");
            try
            {
                if (file != null)
                {
                    foreach (string item in File.ReadAllText(file).Split('#'))
                        _messages.Add(item);
                    if (_messages.Count < _expectedTexts)
                        CriticalErrorMessage("Lang file error: some messages missing.\nCheck: must be " + _expectedTexts + " messages separated by a '#' in " + file);
                }
                else
                    CriticalErrorMessage("No lang file set.\nSee [Local] 'lang' in " + Program._settings.SourceFile);
            }
            catch (Exception e)
            { CriticalErrorMessage(String.Format("Error loading lang file:\n{1}\nSee [Local] 'lang' in {0}\n{2}", Program._settings.SourceFile, file, e.Message)); }
            
            // trigs
            _repeatExperimentTimes = Program._settings.GetValue("Experiment", "repetitions", 1);
            _trigTrial = new Triggerer(int.MaxValue, int.MaxValue, Program._settings.GetValue("Trial", "doTheTrial") == "true" ? Get("Trial", "cycles", 0) : 0, 0);
            _trigTest = new Triggerer(Get("Experiment", "sessionCycles", int.MaxValue), Get("Experiment", "sessionMinutes", int.MaxValue / 60) * 60, Get("Experiment", "maxSoundsToPlay", int.MaxValue), Get("Experiment", "breakDuration", -1));
            try
            {
                foreach (var el in Program._settings.GetValues("Triggers"))
                    _trigTrial.TriggersAdd(new TriggerInfo(el));
            }
            catch (Exception e)
            { CriticalErrorMessage(String.Format("Error reading triggers info:\n{0}\nSee [Triggers] in {1}", e.Message, Program._settings.SourceFile));}
            _trigTest.TriggersAddRange(_trigTrial.Triggers);
            _trigTrial.Trigger += new TriggerEventHandler(_AtrigTrial_Trigger);
            _trigTest.Trigger += new TriggerEventHandler(_AtrigTest_Trigger);

            // data
            try
            {
                _saveIn = new DirectoryInfo(Program._settings.GetValue("Data", "folder"));
            }
            catch (Exception e)
            { CriticalErrorMessage(String.Format("Error managing the data folder:\n{0}\nSee [Data] 'folder' in {1}", e.Message, Program._settings.SourceFile)); }
            _recordTrial = Program._settings.GetValue("Trial", "record") == "true";

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
                _fixDot = Image.FromFile(Program._settings.GetValue("Img", Program._settings.GetValue("FixBlock", "image", "fixImg")));
                _waitDot = Image.FromFile(Program._settings.GetValue("Img", Program._settings.GetValue("NoticeBlock", "image", "waitImg")));
                _restImg = Image.FromFile(Program._settings.GetValue("Img", Program._settings.GetValue("RestingBlock", "image", "restImg")));
                panelSound.BackgroundImage = Image.FromFile(Program._settings.GetValue("Img", Program._settings.GetValue("SoundsBlock", "image", "sndImg")));

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
        private void LoadSounds(string iniSection, out SoundBox sbl, bool separated = true)
        {
            sbl = new SoundBox(); // WAS shuffling settings from ini and separated as args
            foreach (string categoryDir in Program._settings.GetValues(iniSection))
            {
                try
                {
                    if (Directory.Exists(categoryDir))
                    {
                        List<string> contents = new List<string>(Directory.EnumerateFiles(categoryDir));
                        // WAS separated
                        foreach (string soundsDir in contents)
                        {
                            try
                            {
                                if (Directory.Exists(soundsDir))
                                {
                                    List<string> contents2 = new List<string>(Directory.EnumerateFiles(soundsDir));
                                    //            sounds,    subcatdir name
                                    sbl.AddFolder(contents2, categoryDir+'>'+soundsDir);
                                }
                            }
                            catch (Exception e)
                            {
                                CriticalErrorMessage(String.Format("Error loading sounds directory:\n{0}\n{1}\n\nSee [{3}] '{2}'", categoryDir, e.Message, Program._settings.SourceFile, iniSection));
                            }
                        }
                    }
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
            string dir = String.Format("{0,4}{1}{2}_{3}-{4}", DateTime.Now.Year, DateTime.Now.Month.ToString().PadLeft(2, '0'), DateTime.Now.Day.ToString().PadLeft(2, '0'), textBoxSurname.Text, textBoxName.Text);
            _user = new DirectoryInfo(_saveIn.FullName + "/" + dir);
            while (_user.Exists)
                _user = new DirectoryInfo(_saveIn.FullName + "/" + dir + String.Format("_{0}{1}{2}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));
            _user.Create();
            // tsSessionCreation, name, surname, date_of_birth, gender, trialCycles, place, notes
            char separator = (char)Get("Data","ordCharSeparator", 45);
            File.WriteAllText(_user.FullName + "/" + Program._settings.GetValue("Data", "meta\\f"), String.Format("{0}{8}{1}{8}{2}{8}{3}{8}{4}{8}{5}{8}{6}{8}{7}",
                DateTime.Now.ToUnixTimestamp(),
                textBoxName.Text.Replace("\\", "\\\\").Replace(";", "\\,").Replace("\n", "\\n"),
                textBoxSurname.Text.Replace("\\", "\\\\").Replace(";", "\\,").Replace("\n", "\\n"),
                maskedTextBoxBirthDate.Text,
                radioButtonMale.Checked ? "Male" : radioButtonFemale.Checked ? "Feamle" : "N",
                _trigTrial.CicliTotali,
                textBoxPlace.Text.Replace("\\", "\\\\").Replace(";", "\\,").Replace("\n", "\\n"),
                textBoxNotes.Text.Replace("\\", "\\\\").Replace(";", "\\,").Replace("\n", "\\n"),
                separator));

            // data files
            string head;
            head = Program._settings.GetValue("Data", "copyNames", "false") != "false" ? Program._settings.GetValue("Data", "eeg\\n") + "\n" : "";
            _eegFile = new TimeDataFile(_user.FullName + "\\" + Program._settings.GetValue("Data", "eeg\\f"), typeof(EPOC_Data).GetFields().Length, head.Replace(',',separator), separator);
            head = Program._settings.GetValue("Data", "copyNames", "false") != "false" ? Program._settings.GetValue("Data", "blocks\\n") + "\n" : "";
            _blocksFile = new TimeDataFile(_user.FullName + "\\" + Program._settings.GetValue("Data", "blocks\\f"), new List<Type> { typeof(int), typeof(double), typeof(double), typeof(double), typeof(double) }, head.Replace(',', separator), separator);
            head = Program._settings.GetValue("Data", "copyNames", "false") != "false" ? Program._settings.GetValue("Data", "sounds\\n") + "\n" : "";
            _soundsFile = new TimeDataFile(_user.FullName + "\\" + Program._settings.GetValue("Data", "sounds\\f"), new List<Type> { typeof(int), typeof(string), typeof(string), typeof(double), typeof(double) }, head.Replace(',',separator), separator);
            List<Type> tt = new List<Type> { typeof(int) };
            head = Program._settings.GetValue("Data", "copyNames", "false") != "false" ? Program._settings.GetValue("Data", "ratings\\n") + "\n" : "";
            _ratingCsvTypes = new List<Type> { typeof(double), typeof(double), typeof(char) };
            for (int i = 0; i < _ratings.Count; i++)
                tt.AddRange(_ratingCsvTypes);
            _ratingsFile = new TimeDataFile(_user.FullName + "\\" + Program._settings.GetValue("Data", "ratings\\f"), tt, head.Replace(',', separator), separator);
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
            if (System.Windows.Forms.DialogResult.Yes == MessageBox.Show(error + "\n\nStop the program?", cap, MessageBoxButtons.YesNo, MessageBoxIcon.Error))
                System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        // core
        private void SetNewExperiment()
        {
            _state = ExperimentState.Setup;
            _eegEnabled = false;
            ActivePanel = panelData;
            _repeatedExperimentTimes = 0;
            _ciclo = 0;
        }
        private void NextState(int time = 0)
        {
            if (time > 0 && !timerSpacebar.Enabled)
            {
                timerSpacebar.Interval = time;
                timerSpacebar.Enabled = true;
            }
            else switch (_state)
            {
                case ExperimentState.Start:
                    if (_trigTrial.CicliTotali > 0)
                    {
                        _state = ExperimentState.PreTrial;
                        ShowNotice(_messages[/*PRE-TRIAL*/1]);
                    }
                    else
                    {
                        _state = ExperimentState.PreTest;
                        NextState();
                    }
                    break;
                case ExperimentState.PreTrial:
                    _state = ExperimentState.Trial;
                    _trigTrial.Start();
                    _sbTrial.Shuffle();
                    _ciclo = 0;
                    break;
                case ExperimentState.Trial:
                    if (_trigTrial.Finish)
                    {
                        _state = ExperimentState.PreTest;
                        ShowNotice(_messages[/*PRE-TEST*/3]);
                    }
                    else
                        _trigTrial.Resume();
                    break;
                case ExperimentState.PreTest:
                    _state = ExperimentState.PreTestRest;
                    ShowNotice(_messages[/*BASELINE*/4], Get("RestingBlock", "mainResting", 1000));
                    _sbTest.Shuffle();
                    _ciclo = 0;
                    break;
                case ExperimentState.PreTestRest:
                    _state = ExperimentState.Test;
                    _eegEnabled = true;
                    _trigTest.Start();
                    break;
                case ExperimentState.Break:
                    _state = ExperimentState.Test;
                    _trigTest.Resume();
                    break;
                case ExperimentState.Test:
                    if (_trigTest.Finish)
                    {
                        _repeatedExperimentTimes++;
                        if (_repeatedExperimentTimes < _repeatExperimentTimes)
                        {
                            _trigTest.Reset();
                            _sbTest.Shuffle();
                            _trigTest.Start();
                        }
                        else
                        {
                            _state = ExperimentState.Idle;
                            _eegEnabled = false;
                            ShowNotice(_messages[/*END*/7]);
                        }
                    }
                    else
                        _trigTest.Resume();
                    break;
                case ExperimentState.Idle:
                    SetNewExperiment();
                    break;
                default:
                    break;
            }
        }
        private string Trigger(Triggerer sender, SoundBox snd, TriggerInfo trigger, int iciclo)
        {
            switch (trigger.InternalName)
            {
                case "GeneralBreak":
                    _state = ExperimentState.Break;
                    ShowNotice(_messages[/*BRK*/6], Get("Experiment", "breakDuration", -1));
                    break;
                case "GeneralFinish":
                    NextState();
                    break;
                case "inizio":
                    _ciclo++;
                    if (_state == ExperimentState.Test || _recordTrial)
                    {
                        _blocksFile.AddValue((_state == ExperimentState.Trial ? -1 : 1) * _ciclo);
                        _blocksFile.AddTimestamp();
                    }
                    snd.LoadNext();
                    if (trigger.Duration > 0)
                        ShowNotice(_messages[/*BLK*/2]);
                    break;
                case "recordedFix":
                    if (_state == ExperimentState.Test || _recordTrial)
                        _blocksFile.AddTimestamp();
                    if (trigger.Duration > 0)
                        ShowFix();
                    break;
                case "fix":
                    if (trigger.Duration > 0)
                        ShowFix();
                    break;
                case "sound":
                    ActivePanel = panelSound;
                    if (_state == ExperimentState.Test || _recordTrial)
                    {
                        string[] f = snd.LastLoaded.Split('/', '\\');
                        _soundsFile.AddValue((_state == ExperimentState.Trial ? -1 : 1) * _ciclo);
                        _soundsFile.AddValue(f[f.Length - 2]);
                        _soundsFile.AddValue(f[f.Length - 1]);
                        _soundsFile.AddTimestamp();
                    }
                    snd.Play();
                    break;
                case "fixAndLoopSound":
                    bool? s = snd.LoadNext();
                    if (s == null)
                        sender.AskStop();
                    else
                        if ((bool)s)
                            sender.AskRepeat(2);
                    if (_state == ExperimentState.Test || _recordTrial)
                        _soundsFile.AddTimestamp();
                    if (trigger.Duration > 0)
                        ShowFix();
                    break;
                case "initRatings":
                    if (_state == ExperimentState.Test || _recordTrial)
                    {
                        if (_ratingsFile.NextChannel != 0)
                            _ratingsFile.Endline();
                        _ratingsFile.AddValue((_state == ExperimentState.Trial ? -1 : 1) * _ciclo);
                    }
                    _lastRatInit = DateTime.Now;
                    ActivePanel = panelRating;
                    break;
                case "rating":
                    if (NextRating())
                    {
                        sender.AskRepeat(1);
                        // se non ha risp
                        if (_state == ExperimentState.Test || _recordTrial)
                        {
                            _ratingsFile.TypeCheck = false;
                            while (_ratingsFile.NextChannel < _ratingCsvTypes.Count * _currentRating - 2)
                                _ratingsFile.AddValue(null);
                            _ratingsFile.TypeCheck = true;
                            // show
                            _ratingsFile.AddTimestamp();
                        }
                    }
                    else
                    {
                        NextState();
                    }
                    break;
                case "endRatings":
                    if (!((_state == ExperimentState.Trial && Program._settings.GetValue("RestingBlock", "useInTrial") == "true") || _state == ExperimentState.Test))
                    {
                        sender.AskRepeat(-1);
                        _blocksFile.Endline();
                    }
                    ActivePanel = panelFix;
                    sender.AskWait(_lastRatInit.AddMilliseconds(Get("RatingBlock", "minRatingsDuration", 0)));
                    break;
                case "rest":
                    if (_state == ExperimentState.Test || _recordTrial)
                        _blocksFile.AddTimestamp();
                    if ((_state == ExperimentState.Trial && Program._settings.GetValue("RestingBlock", "useInTrial") == "true") || _state == ExperimentState.Test)
                    {
                        ShowNotice(_messages[/*BLK-REST*/5]);
                    }
                    break;
            }
            return default(string);
        }
        private bool NextRating()
        {
            foreach (var item in _ratingsButtons)
                if (item[0].Enabled)
                    item.ForEach(b => b.Enabled = b.Visible = false);

            // ifx set el next
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
                _currentRating++;
                return true;
            }
            else
            {
                _currentRating = 0;
                return false;
            }
        }
        private void ShowWaiting()
        {
            panelFix.BackgroundImage = _waitDot;
            ActivePanel = panelFix;
        }
        private void ShowResting()
        {
            panelFix.BackgroundImage = _restImg;
            ActivePanel = panelFix;
        }
        private void ShowFix(int time = -1)
        {
            panelFix.BackgroundImage = _fixDot;
            ActivePanel = panelFix;
            if (time >= 0)
                NextState(time);
        }
        private void ShowNotice(string p, int time = -1)
        {
            labelNotice.Text = p;
            ActivePanel = panelNotice;
            if (time >= 0)
                NextState(time);
        }

        // interth evt wrapper
        string _AtrigTrial_Trigger(Triggerer sender, TriggerInfo meta, int ciclo, int sessione) { return (string)Invoke(new TriggerEventHandler(_trigTrial_Trigger), sender, meta, ciclo, sessione); }
        string _AtrigTest_Trigger(Triggerer sender, TriggerInfo meta, int ciclo, int sessione) { return (string)Invoke(new TriggerEventHandler(_trigTest_Trigger), sender, meta, ciclo, sessione); }
        string _trigTrial_Trigger(Triggerer sender, TriggerInfo meta, int ciclo, int sessione) { return Trigger(sender, _sbTrial, meta, ciclo); }
        string _trigTest_Trigger(Triggerer sender, TriggerInfo meta, int ciclo, int sessione) { return Trigger(sender, _sbTest, meta, ciclo); }
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
                if (Program._settings.GetValue("NoticeBlock", "waitKey") != "true")
                    this.OnKeyPress(new KeyPressEventArgs(' '));
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
            SetNewExperiment();
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
                _state = ExperimentState.Start;
                if (Program._settings.GetValue("SignalBlock", "show") == "true")
                    ActivePanel = panelConnect;
                else
                    NextState();
            }
        }
        private void buttonRating_Click(object sender, EventArgs e)
        {
            MainForm_KeyPress(this, new KeyPressEventArgs((char)((sender as Button).Tag)));
        }

        // keys
        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (ActivePanel == panelNotice)
            {
                if (e.KeyChar == ' ' && !timerSpacebar.Enabled)
                    if (_state == ExperimentState.Idle)
                        NextState();
                    else
                    {
                        int a = Get("NoticeBlock", "defaultTime", 1000);
                        NextState(a);
                        if (a > 0)
                            ShowWaiting();
                    }
            }
            else if (ActivePanel == panelRating)
            {
                if (_currentRating > 0 &&_currentRating <= _ratings.Count && e.KeyChar >= _ratings[_currentRating - 1].Min && e.KeyChar <= _ratings[_currentRating - 1].Max)
                {
                    _ratingsFile.AddTimestamp();
                    _ratingsFile.AddValue(e.KeyChar);
                    NextState();
                }
            }
            else if (ActivePanel == panelConnect && e.KeyChar == ' ')
            {
                NextState();
            }
        }

        // timing
        private void timerSpacebar_Tick(object sender, EventArgs e)
        {
            timerSpacebar.Enabled = false;
            if (_state == ExperimentState.PreTest
                || _state == ExperimentState.PreTestRest
                || _state == ExperimentState.PreTrial
                || _state == ExperimentState.Break
                || _state == ExperimentState.Trial
                || _state == ExperimentState.Test)
            {
                NextState();
            }
        }
    }
}
