using AvalonDock.Layout.Serialization;
using AvalonDock;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;

namespace Stroblhofwarte.Photometrie.ViewModel
{
  public class DockManagerViewModel
  {
    private RelayCommand mLoadLayoutCommand = null;
    private RelayCommand mSaveLayoutCommand = null;
    /// <summary>Gets a collection of all visible documents</summary>
    public ObservableCollection<DockWindowViewModel> Documents { get; private set; }

    public ObservableCollection<object> Anchorables { get; private set; }

    /// <summary>
    /// Implement a command to load the layout of an AvalonDock-DockingManager instance.
    /// This layout defines the position and shape of each document and tool window
    /// displayed in the application.
    /// 
    /// Parameter:
    /// The command expects a reference to a <seealso cref="DockingManager"/> instance to
    /// work correctly. Not supplying that reference results in not loading a layout (silent return).
    /// </summary>
    public ICommand LoadLayoutCommand
    {
        get
        {
            if (this.mLoadLayoutCommand == null)
            {
                this.mLoadLayoutCommand = new RelayCommand((p) =>
                {
                    DockingManager docManager = p as DockingManager;

                    if (docManager == null)
                        return;

                    this.LoadDockingManagerLayout(docManager);
                });
            }

            return this.mLoadLayoutCommand;
        }
    }

    /// <summary>
    /// Implements a command to save the layout of an AvalonDock-DockingManager instance.
    /// This layout defines the position and shape of each document and tool window
    /// displayed in the application.
    /// 
    /// Parameter:
    /// The command expects a reference to a <seealso cref="string"/> instance to
    /// work correctly. The string is supposed to contain the XML layout persisted
    /// from the DockingManager instance. Not supplying that reference to the string
    /// results in not saving a layout (silent return).
    /// </summary>
    public ICommand SaveLayoutCommand
    {
        get
        {
            if (this.mSaveLayoutCommand == null)
            {
                this.mSaveLayoutCommand = new RelayCommand((p) =>
                {
                    string xmlLayout = p as string;

                    if (xmlLayout == null)
                        return;

                    this.SaveDockingManagerLayout(xmlLayout);
                });
            }

            return this.mSaveLayoutCommand;
        }
    }

    public DockManagerViewModel(IEnumerable<DockWindowViewModel> dockWindowViewModels)
    {
      this.Documents = new ObservableCollection<DockWindowViewModel>();
      this.Anchorables = new ObservableCollection<object>();

      foreach (var document in dockWindowViewModels)
      {
        document.PropertyChanged += DockWindowViewModel_PropertyChanged;
        if (!document.IsClosed)
          this.Documents.Add(document);
      }
    }

    private void DockWindowViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      DockWindowViewModel document = sender as DockWindowViewModel;

      if (e.PropertyName == nameof(DockWindowViewModel.IsClosed))
      {
        if (!document.IsClosed)
          this.Documents.Add(document);
        else
          this.Documents.Remove(document);
      }
    }

    #region methods
    #region LoadLayout
    /// <summary>
    /// Loads the layout of a particular docking manager instance from persistence
    /// and checks whether a file should really be reloaded (some files may no longer
    /// be available).
    /// </summary>
    /// <param name="docManager"></param>
    private void LoadDockingManagerLayout(DockingManager docManager)
    {
        string layoutFileName = "layout.dat";

        if (System.IO.File.Exists(layoutFileName) == false)
            return;

        var layoutSerializer = new XmlLayoutSerializer(docManager);

        layoutSerializer.LayoutSerializationCallback += (s, args) =>
        {
            // This can happen if the previous session was loading a file
            // but was unable to initialize the view ...
            if(args.Model.ContentId == "AavsoViewModel")
            {
                foreach(DockWindowViewModel c in Documents)
                {
                    if((c as AavsoViewModel) != null)
                    {
                        args.Content = c;
                        break;
                    }
                } 
            }
            else if (args.Model.ContentId == "ApertureViewModel")
            {
                foreach (DockWindowViewModel c in Documents)
                {
                    if ((c as ApertureViewModel) != null)
                    {
                        args.Content = c;
                        break;
                    }
                }
            }
            else if (args.Model.ContentId == "FileViewModel")
            {
                foreach (DockWindowViewModel c in Documents)
                {
                    if ((c as FileViewModel) != null)
                    {
                        args.Content = c;
                        break;
                    }
                }
            }
            else if (args.Model.ContentId == "ImageInfoViewModel")
            {
                foreach (DockWindowViewModel c in Documents)
                {
                    if ((c as ImageInfoViewModel) != null)
                    {
                        args.Content = c;
                        break;
                    }
                }
            }
            else if (args.Model.ContentId == "ImageViewModel")
            {
                foreach (DockWindowViewModel c in Documents)
                {
                    if ((c as ImageViewModel) != null)
                    {
                        args.Content = c;
                        break;
                    }
                }
            }
            else if (args.Model.ContentId == "MagnificationViewModel")
            {
                foreach (DockWindowViewModel c in Documents)
                {
                    if ((c as MagnificationViewModel) != null)
                    {
                        args.Content = c;
                        break;
                    }
                }
            }
            else if (args.Model.ContentId == "ReportViewModel")
            {
                foreach (DockWindowViewModel c in Documents)
                {
                    if ((c as ReportViewModel) != null)
                    {
                        args.Content = c;
                        break;
                    }
                }
            }
            else
            {
                args.Cancel = true;
            }
        };

        layoutSerializer.Deserialize(layoutFileName);
    }

    #endregion LoadLayout

    #region SaveLayout
    private void SaveDockingManagerLayout(string xmlLayout)
    {
        // Create XML Layout file on close application (for re-load on application re-start)
        if (xmlLayout == null)
            return;

        string fileName = "layout.dat";

        File.WriteAllText(fileName, xmlLayout);
    }
    #endregion SaveLayout
    #endregion methods
    }


}
