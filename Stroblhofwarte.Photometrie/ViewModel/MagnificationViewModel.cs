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
using Stroblhofwarte.Image;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Stroblhofwarte.Photometrie.ViewModel
{
    public class MagnificationViewModel : DockWindowViewModel
    {
        #region Properties

        public BitmapImage ImageSource
        {
            get
            {
                if (!StroblhofwarteImage.Instance.IsValid) return null;

                using (MemoryStream memory = new MemoryStream())
                {
                    int magN = Properties.Settings.Default.MagnificationN;
                    StroblhofwarteImage.Instance.GetSubimage(StroblhofwarteImage.Instance.CursorPosition, magN, magN).Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;
                    BitmapImage bitmapimage = new BitmapImage();
                    bitmapimage.BeginInit();
                    bitmapimage.StreamSource = memory;
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    //bitmapimage.DecodePixelWidth = _fitsImage.Width();
                    bitmapimage.EndInit();

                    return bitmapimage;
                }
            }
        }

        #endregion

        #region ctor

        public MagnificationViewModel()
        {
            StroblhofwarteImage.Instance.NewCursorPosition += Instance_NewCursorPosition;
            ContentId = "MagnificationViewModel";
        }

        private void Instance_NewCursorPosition(object? sender, EventArgs e)
        {
            OnPropertyChanged("ImageSource");
        }
        #endregion
    }
}
