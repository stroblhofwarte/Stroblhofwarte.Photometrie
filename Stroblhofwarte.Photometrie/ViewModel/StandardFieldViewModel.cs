#region "copyright"

/*
    Copyright © 2023 Othmar Ehrhardt, https://astro.stroblhof-oberrohrbach.de

    This file is part of the Stroblhof.Photometrie application

    This Source Code Form is subject to the terms of the GNU General Public License 3.
    If a copy of the GPL v3.0 was not distributed with this
    file, You can obtain one at https://www.gnu.org/licenses/gpl-3.0.de.html.
*/

#endregion "copyright"

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
            _fields = new ObservableCollection<Fields>() {  { new Fields() { Name = "No standard field", Id = "0" } },
                                                            { new Fields() { Name = "M67", Id="67" } } };

            StandardFieldFiles fileBasesFields = new StandardFieldFiles();
            List<string> files = fileBasesFields.Fields();
            foreach(string file in files)
            {
                StandardFieldHeader h = fileBasesFields.HeaderFrom(file);
                _fields.Add(new ViewModel.Fields() { Name = h.Name, Id = h.Id });
            }

            _stars = new ObservableCollection<StandardStar>();
            ItemChange = _fields[0];

            MagMeasurement.Instance.NewMeasurement += Instance_NewMag;
            StroblhofwarteImage.Instance.NewImageLoaded += Instance_NewImageLoaded;
        }

        private void Instance_NewImageLoaded(object? sender, EventArgs e)
        {
            OnPropertyChanged("SelectStarfieldEnabled");
        }

        private void Instance_NewMag(object? sender, EventArgs e)
        {
            MeasurementEventArgs args = e as MeasurementEventArgs;
            if (args == null) return;
            if (!args.IsMachine) return;
            double x = args.X;
            double y = args.Y;
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
            TransformDataSet.Instance.Update(SelectedStar.Id, args.Mag, SelectedStar.V, SelectedStar.BV, SelectedStar.UB, SelectedStar.VR, SelectedStar.RI);
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
                if (_selectedItem.Id == "0")
                {
                    _stars.Clear();
                    StroblhofwarteImage.Instance.ClearAnnotation();
                    MagMeasurement.Instance.CalibrationMode = false;
                    OnPropertyChanged("ItemChange");
                    OnPropertyChanged("Stars");
                }
                else if (_selectedItem.Id == "67")
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
                else
                {
                    _stars.Clear();
                    MagMeasurement.Instance.CalibrationMode = true;
                    StandardFieldFiles fileBasesFields = new StandardFieldFiles();
                    List<StandardStar> stars = fileBasesFields.StandardField(_selectedItem.Id);
                    foreach (StandardStar s in stars)
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
        public string Id { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
