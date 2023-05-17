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

        #region Properties

        private Dictionary<int, List<string>> _pad;
        private int _currentPage = 0;
        private int _lineCnt = 0;

        #endregion

        #region Ctor

        public ScratchPad()
        {
            _pad = new Dictionary<int, List<string>>();
            _pad.Add(_currentPage, new List<string>());
        }

        #endregion

        public void Add(string formula)
        {
            _lineCnt++;
            if (_lineCnt > 100)
            {
                CreateAndAdd(formula);
                eventPadChanged?.Invoke(this, null);
                return;
            }
            _pad[_currentPage].Add(formula);
            eventPadChanged?.Invoke(this, null);
        }

        public void CreateAndAdd(string formula)
        {
            _currentPage++;
            _lineCnt = 0;
            _pad.Add(_currentPage, new List<string>());
            _pad[_currentPage].Add(formula);
            eventPadChanged?.Invoke(this, null);
        }

        public int Pages() { return _currentPage; }
        public string GetCurrentPage()
        {
            string page = string.Empty;
            foreach (string s in _pad[_currentPage])
            {
                page += s + "\\\\";
            }
            return page;
        }

        public string GetPage(int p)
        {
            string page = string.Empty;
            if (p >= _currentPage) return page;
            if (p < 0) return page;
            foreach (string s in _pad[p])
            {
                page += s + "\\";
            }
            return page;
        }

        public void Clear()
        {
            _currentPage = 0;
            _pad.Clear();
            _pad.Add(_currentPage, new List<string>());
        }
    }
}