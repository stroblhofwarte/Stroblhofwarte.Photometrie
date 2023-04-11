﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stroblhofwarte.Photometrie.ViewModel
{
  public class MainViewModel
  {
    public DockManagerViewModel DockManagerViewModel { get; private set; }
    public MenuViewModel MenuViewModel { get; private set; }

    public MainViewModel()
    {
      var documents = new List<DockWindowViewModel>();

      for (int i = 0; i < 2; i++)
        documents.Add(new SampleDockWindowViewModel() { Title = "Sample " + i.ToString() });

      documents.Add(new FileViewModel() { Title = "Files" });
      documents.Add(new ImageViewModel() { Title = "Image" });

      this.DockManagerViewModel = new DockManagerViewModel(documents);
      this.MenuViewModel = new MenuViewModel(documents);
    }
  }
}
