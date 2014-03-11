using Director.Forms.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Director.Forms.Interfaces
{
    public interface GUIDataReceiver
    {
        void SetData(string data);
        void EndEvent();
        void ProcessBackgroundData(GUIAbstractWorker receiver);
    }
}
