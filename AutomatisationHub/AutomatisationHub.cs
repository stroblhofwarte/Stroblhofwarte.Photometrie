namespace AutomatisationHub
{
    public class AutomatisationHub
    {
        #region Singelton

        private static AutomatisationHub _instance;

        public static AutomatisationHub Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new AutomatisationHub();
                }
                return _instance;
            }
        }

        public bool NextImageAvailabel { get; set; }

        public bool AutomaticImageLoadEnabled { get; set; }
        public bool AutomaticPhotometryEnabled { get; set; }
        #endregion

        #region Events

        // This event is fired when someone should load a new image
        // -> FileViewModel will hook to this and load the next image when fired.
        public event EventHandler NextImageRequest;

        // This event is fired when someone should do photometry on the loaded image
        public event EventHandler PhotometryRequest;

        // This event is fired when photometry was done
        public event EventHandler PhotometryDone;

        #endregion

        #region Properties

        public System.Drawing.Point VarPoint { get; set; }
        public System.Drawing.Point CPoint { get; set; }
        public System.Drawing.Point KPoint { get; set; }


        #endregion

        #region Ctor

        #endregion

        public void NextImage()
        {
            NextImageRequest?.Invoke(this, null);
        }

        public void StartPhotometry() 
        {
            PhotometryRequest?.Invoke(this, null);
        }

        public void PhotometryFinished()
        {
            PhotometryDone?.Invoke(this, null);
        }
    }
}