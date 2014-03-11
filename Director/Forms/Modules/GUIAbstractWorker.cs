using Director.Forms.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Director.Forms.Modules
{
    public class GUIAbstractWorker
    {
        private GUIDataReceiver _receiver;
        private BackgroundWorker _worker = new BackgroundWorker();

        public GUIAbstractWorker(GUIDataReceiver receiver)
        {
            _worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            _worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            _worker.WorkerReportsProgress = true;
            _receiver = receiver;
        }

        public void RunWorker()
        {
            _worker.RunWorkerAsync();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _receiver.EndEvent();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _receiver.SetData(e.UserState.ToString());
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            _receiver.ProcessBackgroundData(this);
        }

        public void ReportProgress(int sender, String data) 
        {
            _worker.ReportProgress(sender, data);
        }
    }
}
