using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Director.Forms.Panels;
using Xwt.Drawing;

namespace Director.Forms
{
    class MainWindow : Window
    {
        /// <summary>
        /// Default box
        /// </summary>
        private HPaned MainBox { get; set; }

        /// <summary>
        /// Content box for widgets!
        /// </summary>
        private VBox ContentBox { get; set; }

        /// <summary>
        /// Current control.
        /// </summary>
        private VBox CurrentControl { get; set; }

        /// <summary>
        /// New instance of homepage.
        /// </summary>
        private VBox DirectorHomepage = new Homepage();

        /// <summary>
        /// Current server sumary.
        /// </summary>
        private TreeView CurrentServer { get; set; }

        public MainWindow()
        {
            _initializeComponents();
        }

        private void _initializeComponents() {
            /// Title and Initial size
            Title = "API Director";
            Width = 650;
            Height = 500;

            // Center screen
            InitialLocation = WindowLocation.CenterScreen;

            // Initialize menu
            _initializeMenu();

            // Initialize content
            _intializeBoxes();
        }

        private void _intializeBoxes()
        {
            // Create HPaned main box
            MainBox = new HPaned();

            // Create tree view
            CurrentServer = new TreeView();
            CurrentServer.WidthRequest = 100;

            // Add server to main box
            MainBox.Panel1.Content = CurrentServer;
            MainBox.Panel1.Resize = false;
            MainBox.Panel1.Shrink = false;

            // Prepare content box
            ContentBox = new VBox();
            ContentBox.MarginLeft = 10;
            ContentBox.PackStart(DirectorHomepage, expand: true, fill: true);

            // Add to panel
            MainBox.Panel2.Content = ContentBox;
            MainBox.Panel2.Resize = true;
            MainBox.Panel2.Shrink = true;

            // Set content to main box
            Content = MainBox;
        }

        private void _initializeMenu() 
        {
            Menu menu = new Menu();

            MenuItem server = new MenuItem("Server");
            server.SubMenu = new Menu();

            // Sub menu server
            MenuItem _new = new MenuItem("New server");
            _new.Image = Image.FromResource("Director.Resources.new_server.png");
            server.SubMenu.Items.Add(_new);

            // Separator before exit
            server.SubMenu.Items.Add(new SeparatorMenuItem());

            // Sub menu exit
            MenuItem _exit = new MenuItem("Exit");
            _exit.Image = Image.FromResource("Director.Resources.exit.png");
            _exit.Clicked += delegate
            {
                Application.Exit();
            };
            server.SubMenu.Items.Add(_exit);
            menu.Items.Add(server);

            // Set menu
            MainMenu = menu;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
