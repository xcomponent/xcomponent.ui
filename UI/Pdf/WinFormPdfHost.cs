﻿using System;
using System.Windows.Forms;

namespace XComponent.Common.UI.Pdf
{
    public partial class WinFormPdfHost : UserControl
    {
        public WinFormPdfHost()
        {
            InitializeComponent();
            if (!DesignMode)
                try
                {
                    axAcroPDF1.setShowToolbar(true);
                }
                catch (Exception e)
                {
                 //   System.Windows.Forms.MessageBox.Show(e.ToString());
                }
        }

        public void LoadFile(string path)
        {
            if (path != null && axAcroPDF1 != null)
            {
                try
                {
                    axAcroPDF1.LoadFile(path);
                    axAcroPDF1.src = path;
                    axAcroPDF1.setViewScroll("FitH", 0);
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show(e.ToString());
                }
            }
        }

        public void SetShowToolBar(bool on)
        {
            axAcroPDF1.setShowToolbar(on);
        }
    }
}
