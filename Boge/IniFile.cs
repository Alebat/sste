using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace WS_STE
{
    class IniFile
    {
        Dictionary<string, Dictionary<string, string>> _main;
        string _saveFile;

        public string SourceFile { get; private set; }

        private void ParseAdd(StreamReader lines)
        {
            try
            {
                // default sec
                string section = "global";
                // imm. add
                _main.Add(section, new Dictionary<string, string>());
                // fe line
                while (!lines.EndOfStream)
                {
                    string line = lines.ReadLine();
                    // trim start
                    line = line.TrimStart(' ', '\t');
                    // comment cleaning
                    if (line.Length > 0)
                    {
                        if (line[0] != '*')
                        {
                            int ioa = line.IndexOfAny(";#".ToCharArray());
                            if (ioa >= 0)
                                line = line.Remove(ioa);
                        }
                        else
                            line = line.Substring(1);
                        // trim end
                        line = line.TrimEnd(' ', '\t');
                    }
                    // valid len > 0
                    if (line.Length > 0)
                        // first ch
                        if (line[0] == '[')
                        {
                            // closing ] else IGNORED
                            if (line.Contains("]"))
                            {
                                section = line.Substring(1).Remove(line.LastIndexOf(']') - 1);
                                if (!_main.ContainsKey(section))
                                    _main.Add(section, new Dictionary<string, string>());
                            }
                        }
                        // data
                        else
                        {
                            // key val
                            if (line.Contains("="))
                            {
                                // concat assignation (a=b=c=d=3)
                                string[] tokens = line.Split('=');
                                string s = tokens[tokens.Length - 1].Trim(' ', '\t');
                                for (int i = 0; i < tokens.Length - 1; i++)
                                {
                                    string p = tokens[i].Trim(' ', '\t');
                                    if (_main[section].ContainsKey(p))
                                        _main[section][p] = s;
                                    else
                                        _main[section].Add(p, s);
                                }
                            }
                            else
                            {
                                string s = line.Trim(' ', '\t');
                                if (_main[section].ContainsKey(s))
                                    _main[section][s] = "";
                                else
                                    _main[section].Add(s, "");
                            }
                        }
                }
            }
            catch (Exception e)
            {
                throw new FormatException(String.Format("Ini parsing {0}:\n{1}", "'file'", e.Message));
            }
        }

        private void SaveAll(StreamWriter file)
        {
            file.WriteLine("# saved: comments deleted");
            // fe section
            foreach (KeyValuePair<string,Dictionary<string,string>> sec in _main)
            {
                // sec name
                file.WriteLine("[{0}]", sec.Key);
                // fe val / assign
                foreach (KeyValuePair<string,string> kv in sec.Value)
                {
                    if (kv.Key.Length > 0)
                        file.Write(kv.Key + "=");
                    file.WriteLine(kv.Value);
                }
            }
        }

        public IniFile()
            : this("main.ini")
        {

        }

        public IniFile(string source)
        {
            _main = new Dictionary<string, Dictionary<string, string>>();
            if (!File.Exists(source))
                throw new FileNotFoundException(source);
            else
                ParseAdd(new StreamReader(source));
            SourceFile = source;
        }

        public Dictionary<string, Dictionary<string, string>> Data
        {
            get { return _main; }
        }

        public string GetValue(string section, string key)
        {
            string a;
            if (_main.ContainsKey(section) && _main[section].ContainsKey(key))
                return a = _main[section][key];
            else
                return null;
        }

        public string GetValue(string section, string key, string def)
        {
            return GetValue(section, key) ?? def;
        }

        public int GetValue(string section, string key, int def)
        {
            int n;
            if (int.TryParse(GetValue(section, key) ?? "nooooo", out n))
                return n;
            else
                return def;
        }

        public List<string> GetValues(string section)
        {
            List<string> v = null;
            if (_main.ContainsKey(section))
                v = new List<string>(_main[section].Values);
            return v;
        }
    }
}
