using Director.Forms.Controls;
using System;
using Xwt;

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
        protected InfoBox MainInfoBox { get; set; }

        /// <summary>
        /// Main window.
        /// </summary>
        protected MainWindow CurrentMainWindow { get; set; }

        /// <summary>
        /// Set actual position.
        /// </summary>
        public TreePosition ActualPosition { get; set; }


        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="_mainWindow"></param>
        /// <param name="_desc"></param>
        /// <param name="_image"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PanelBase(MainWindow _mainWindow, String _desc, String _image)
        {
            CurrentMainWindow = _mainWindow;
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
