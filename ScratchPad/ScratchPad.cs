using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ScratchPad
{
    public class ScratchPad
    {
        #region Singelton

        private static ScratchPad _instance;
        public static ScratchPad Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ScratchPad();
                }
                return _instance;
            }
        }

        #endregion

        public event EventHandler eventPadChanged;
        public event EventHandler eventNewPageCreated;

        #region Properties

        private Dictionary<int, List<string>> _pad;
        private int _currentPage = 0;
        private int _lineCnt = 0;

        private bool _isActive = false;
        private bool _batch = false;

        #endregion

        #region Ctor

        public ScratchPad()
        {
            _pad = new Dictionary<int, List<string>>();
            _pad.Add(_currentPage, new List<string>());
        }

        #endregion

        public void Begin()
        {
            _batch = true;
        }

        public void Commit()
        {
            _batch = false;
            eventPadChanged?.Invoke(this, null);
        }

        public void Add(string formula)
        {
            if (!Stroblhofwarte.Config.GlobalConfig.Instance.ScratchPadActive) return;
            _lineCnt++;
            /*if (_lineCnt > 100)
            {
                CreateAndAdd(formula);
                eventPadChanged?.Invoke(this, null);
                return;
            }*/
            _pad[_currentPage].Add(formula);
            if(!_batch) eventPadChanged?.Invoke(this, null);
        }

        public void CreateAndAdd(string formula)
        {
            if (!Stroblhofwarte.Config.GlobalConfig.Instance.ScratchPadActive) return;
            _currentPage++;
            _lineCnt = 0;
            _pad.Add(_currentPage, new List<string>());
            _pad[_currentPage].Add(formula);
            eventNewPageCreated?.Invoke(this, null);
            eventPadChanged?.Invoke(this, null);
        }

        public int Pages() { return _currentPage; }
        public string GetCurrentPage()
        {
            if (!Stroblhofwarte.Config.GlobalConfig.Instance.ScratchPadActive) return string.Empty;
            string page = string.Empty;
            foreach (string s in _pad[_currentPage])
            {
                page += s + "\\\\";
            }
            return page;
        }

        public string GetPage(int p)
        {
            if (!Stroblhofwarte.Config.GlobalConfig.Instance.ScratchPadActive) return string.Empty;
            string page = string.Empty;
            if (p > _currentPage) return page;
            if (p < 0) return page;
            foreach (string s in _pad[p])
            {
                page += s + @"\\";
            }
            return page;
        }

        public void Clear()
        {
            _currentPage = 0;
            _pad.Clear();
            _pad.Add(_currentPage, new List<string>());
            eventPadChanged?.Invoke(this, null);
        }

        public int CurrentPagePtr()
        {
            return _currentPage;
        }

        public bool Save(string filename)
        {
            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, _pad);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Load(string filename)
        {
            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    _pad = (Dictionary<int, List<string>>)formatter.Deserialize(fs);
                    _currentPage = _pad.Count - 1;
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}