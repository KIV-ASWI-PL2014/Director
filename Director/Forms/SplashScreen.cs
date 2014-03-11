using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Director.Forms.Interfaces;
using Director.Forms.Modules;

namespace Director
{
    namespace Forms
    {
        public partial class SplashScreen : Form, GUIDataReceiver
        {
            Form _parent;

            public SplashScreen(Form _parent)
            {
                InitializeComponent();
                this._parent = _parent;
                GUIAbstractWorker module = new GUIAbstractWorker(this);
                module.RunWorker();
            }

            public void ProcessBackgroundData(GUIAbstractWorker receiver)
            {
                // This is the place for loading stuffs!
                Thread.Sleep(2000);
                receiver.ReportProgress(0, "Data");
            }

            public void EndEvent()
            {
                this.Hide();
                _parent.Show();
            }

            public void SetData(String data)
            { 
                // Nothing YET
            }
        }
    }
}
