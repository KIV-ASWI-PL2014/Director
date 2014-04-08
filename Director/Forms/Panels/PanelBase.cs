using Director.Forms.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Xwt.Drawing;

namespace Director.Forms.Panels
{
    /// <summary>
    /// Panel base extensions for all information panels.
    /// </summary>
    abstract class PanelBase : VBox
    {
        /// <summary>
        /// Panel info box.
        /// </summary>
        private InfoBox MainInfoBox { get; set; }

        /// <summary>
        /// Main window.
        /// </summary>
        private MainWindow MainWindow { get; set; }


        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="_mainWindow"></param>
        /// <param name="_desc"></param>
        /// <param name="_image"></param>
        public PanelBase(MainWindow _mainWindow, String _desc, String _image)
        {
            MainWindow = _mainWindow;
            MainInfoBox = new InfoBox(_desc, _image);
            PackStart(MainInfoBox);
            MarginLeft = 10;
            _initializeComponents();
        }

        /// <summary>
        /// Abstract method has to be overrided!
        /// </summary>
        public abstract void _initializeComponents();
    }
}
