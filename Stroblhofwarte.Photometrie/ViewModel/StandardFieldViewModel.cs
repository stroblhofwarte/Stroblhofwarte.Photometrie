using Stroblhofwarte.AperturePhotometry;
using Stroblhofwarte.AperturePhotometry.StandardFields;
using Stroblhofwarte.FITS.DataObjects;
using Stroblhofwarte.Image;
using Stroblhofwarte.Photometrie.Dialogs;
using Stroblhofwarte.VSPAPI.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stroblhofwarte.Photometrie.ViewModel
{
    public class StandardFieldViewModel : DockWindowViewModel
    {
        #region Properties

        private ObservableCollection<Fields> _fields;
        public ObservableCollection<Fields> Fields
        {
            get
            {
                return _fields;
            }
        }

        private ObservableCollection<StandardStar> _stars;

        public ObservableCollection<StandardStar> Stars
        {
            get
            {
                return _stars;
            }
        }

        private StandardStar _selectedStar;

        public StandardStar SelectedStar
        {
            get
            {
                return _selectedStar;
            }
            set
            {
                _selectedStar = value;
                OnPropertyChanged("SelectedStar");
            }
        }
        
        #endregion
        #region Ctor

        public StandardFieldViewModel()
        {
            _fields = new ObservableCollection<Fields>() {  { new Fields() { Name = "No standard field", Id = 0 } },
                                                            { new Fields() { Name = "M67", Id=67 } } };
            _stars = new ObservableCollection<StandardStar>();
            ItemChange = _fields[0];
            
            StroblhofwarteImage.Instance.NewCursorClickPosition += Instance_NewCursorClickPosition;
            StroblhofwarteImage.Instance.NewImageLoaded += Instance_NewImageLoaded;
        }

        private void Instance_NewImageLoaded(object? sender, EventArgs e)
        {
            OnPropertyChanged("SelectStarfieldEnabled");
        }

        private void Instance_NewCursorClickPosition(object? sender, EventArgs e)
        {
            double x = StroblhofwarteImage.Instance.CursorClickPosition.X;
            double y = StroblhofwarteImage.Instance.CursorClickPosition.Y;
            Coordinates coor = StroblhofwarteImage.Instance.WCS.GetCoordinates(x, y);
            int idx = 0;
            double minDistance = double.MaxValue;
            int r = 0;
            foreach(StandardStar s in Stars)
            {
                double distance = ((s.X - x) * (s.X -x)) + ((s.Y - y) * (s.Y - y));
                if(minDistance >= distance)
                {
                    minDistance = distance;
                    idx = r;
                }
                r++;
            }
            SelectedStar = Stars[idx];
        }

        private Fields _selectedItem;
        public Fields ItemChange
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                if (_selectedItem.Id == 0)
                {
                    _stars.Clear();
                    StroblhofwarteImage.Instance.ClearAnnotation();
                    MagMeasurement.Instance.CalibrationMode = false;
                    OnPropertyChanged("ItemChange");
                    OnPropertyChanged("Stars");
                }
                if (_selectedItem.Id == 67)
                {
                    _stars.Clear();
                    MagMeasurement.Instance.CalibrationMode = true;
                    Stroblhofwarte.AperturePhotometry.StandardFields.M67 thisField = new Stroblhofwarte.AperturePhotometry.StandardFields.M67();
                    foreach(StandardStar s in thisField.Stars)
                    {
                        Coordinates c = new Coordinates(s.Ra, s.DEC, Epoch.J2000, Coordinates.RAType.Degrees);
                        StroblhofwarteImage.Instance.AddAnnotation(s.Id, c);
                        System.Windows.Point p = StroblhofwarteImage.Instance.WCS.GetCoordinates(c);
                        s.X = p.X;
                        s.Y = p.Y;
                        _stars.Add(s);
                    }
                    OnPropertyChanged("ItemChange");
                    OnPropertyChanged("Stars");
                }
            }
        }

        public bool SelectStarfieldEnabled
        {
            get
            {
                if (StroblhofwarteImage.Instance.WCS == null) return false;
                return true;
            }
        }

        #endregion
    }

    public class Fields
    {
        public string Name { get; set; }
        public int Id { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
