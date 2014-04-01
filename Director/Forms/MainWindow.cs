using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Director.Forms.Panels;
using Xwt.Drawing;
using Director.Forms.Controls;
using Director.DataStructures;

namespace Director.Forms
{
    class MainWindow : Window
    {
        /// <summary>
        /// Default box
        /// </summary>
		private HBox MainBox { get; set; }

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
        /// Server page.
        /// </summary>
        private VBox ServerPage = new ServerPage();

        /// <summary>
        /// Current server sumary.
        /// </summary>
        private TreeView CurrentServer { get; set; }

        /// <summary>
        /// New server store.
        /// </summary>
        private TreeStore ServerStore;

        /// <summary>
        /// New server or server whitch is using.
        /// </summary>
        private Server UServer { get; set; }

        /// <summary>
        /// Data fields describing server - save to TreeStore.
        /// </summary>
        private DataField<Image> ColumnImage = new DataField<Image>();
        private DataField<String> ColumnName = new DataField<string>();
        private DataField<Object> ColumnType = new DataField<object>();


        /// <summary>
        /// Default images for Server tree.
        /// </summary>
        private Image ServerImage = Image.FromResource(DirectorImages.ROOT_ICON);
        private Image ScenarioImage = Image.FromResource(DirectorImages.SCENARIO_ICON);
        private Image RequestImage = Image.FromResource(DirectorImages.REQUEST_ICON);
        private Image ProcessingImage = Image.FromResource(DirectorImages.PROCESSING_ICON);

        /// <summary>
        /// Server context menu.
        /// </summary>
        private Menu ServerMenu;

        /// <summary>
        /// Scenario context menu.
        /// </summary>
        private Menu ScenarioMenu;

        /// <summary>
        /// Request context menu.
        /// </summary>
        private Menu RequestMenu;

        /// <summary>
        /// Default constructor initiate components!
        /// </summary>
        public MainWindow()
        {
            _initializeComponents();
        }

        /// <summary>
        /// Initiate all form components.
        /// </summary>
        private void _initializeComponents() {
            /// Title and Initial size
            Title = Director.Locales.Language.MainWindowTitle;

            // Set default size
            Width = 650;
            Height = 500;

            // Center screen
            InitialLocation = WindowLocation.CenterScreen;

            // Initialize menu
            _initializeMenu();

            // Initialize content
            _intializeBoxes();
        }
        /// <summary>
        /// Initialize whole layout!
        /// </summary>
        private void _intializeBoxes()
        {
            // Create HPaned main box
			MainBox = new HBox();
			MainBox.ExpandVertical = true;
			MainBox.ExpandHorizontal = true;
			MainBox.Spacing = 3;

            // Create tree view
            CurrentServer = new TreeView();
			CurrentServer.WidthRequest = 200;
            CurrentServer.SelectionMode = SelectionMode.Single;

            // Create tree view!
            ServerStore = new TreeStore(ColumnName, ColumnImage, ColumnType);
            CurrentServer.Columns.Add("Server", ColumnImage, ColumnName); // Dummy column, not sure why?
            CurrentServer.HeadersVisible = false;

            // Net item parsing
            CurrentServer.ButtonPressed += HandleMouseOnTreeView;

            // Set data source
            CurrentServer.DataSource = ServerStore;

            // Add server to main box
			MainBox.PackStart (CurrentServer);

            // Prepare content box
            ContentBox = new VBox();
            ContentBox.MarginLeft = 10;
            ContentBox.PackStart(DirectorHomepage, expand: true, fill: true);

            // Add to panel
			MainBox.PackStart (ContentBox, true, true);

            // Set content to main box
			Content = MainBox;
        }

        private void HandleMouseOnTreeView(object sender, ButtonEventArgs e)
        {
            if (e.Button == PointerButton.Right && CurrentServer.SelectedRow != null)
            {
                Point p = new Point(e.X, e.Y);
                TreePosition tmp;
                RowDropPosition tmp2;

                CurrentServer.GetDropTargetRow(e.X, e.Y, out tmp2, out tmp);
                CurrentServer.SelectRow(tmp);
                ServerMenu.Popup(CurrentServer, p.X, p.Y);
            }
        }

        /// <summary>
        /// Menu associers.
        /// </summary>
        private MenuItem NewServer;

        /// <summary>
        /// Prepare menu.
        /// </summary>
        private void _initializeMenu() 
        {
            // Default menu
            Menu menu = new Menu();

            // Create main menu item
            MenuItem server = new MenuItem(Director.Locales.Language.MenuServer);

            // Prepare submenu
            server.SubMenu = new Menu();

            // Sub menu server
            NewServer = new MenuItem(Director.Locales.Language.MenuNewServer)
            { 
                Image = Image.FromResource(DirectorImages.NEW_SERVER_ICON) 
            };
            NewServer.Clicked += CreateNewServer;
            server.SubMenu.Items.Add(NewServer);

            // Separator before exit
            server.SubMenu.Items.Add(new SeparatorMenuItem());

            // Sub menu exit
            MenuItem _exit = new MenuItem(Director.Locales.Language.MenuExitProgram)
            {
                Image = Image.FromResource(DirectorImages.EXIT_ICON)
            };
            _exit.Clicked += delegate
            {
                Application.Exit();
            };
            server.SubMenu.Items.Add(_exit);
            menu.Items.Add(server);

            // Set menu
            MainMenu = menu;

            // Create server menu
            ServerMenu = new Menu();

            // Add scenario item
            MenuItem MenuAddScenario = new MenuItem(Director.Locales.Language.MenuAddScenario)
            {
                Image = Image.FromResource(DirectorImages.ADD_ICON)
            };
            ServerMenu.Items.Add(MenuAddScenario);
            MenuAddScenario.Clicked += AddScenario;
        }

        /// <summary>
        /// Create new scenario which belongs to server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddScenario(object sender, EventArgs e)
        {
            // Get selected row
            TreePosition tmp = CurrentServer.SelectedRow;

            // Return if null
            if (tmp == null) 
                return;

            // Get row
            var data = ServerStore.GetNavigatorAt(tmp).GetValue(ColumnType);

            if (data is Server)
            {
                Server s = (Server)data;

                // Add new scenario
                Scenario NewScenario = s.CreateNewScenario();

                // Add to tree!
                CreateTreeItem(tmp, NewScenario.Name, ScenarioImage, NewScenario);
            }
        }

        /// <summary>
        /// Create tree item on specific position.
        /// </summary>
        /// <param name="pos">Position</param>
        /// <param name="name">Item name</param>
        /// <param name="icon">Icon</param>
        /// <param name="item">Object</param>
        /// <returns>New tree position</returns>
        private TreePosition CreateTreeItem(TreePosition pos, string name, Image icon, Object item)
        {
            return ServerStore.AddNode(pos)
                              .SetValue(ColumnName, name)
                              .SetValue(ColumnImage, icon)
                              .SetValue(ColumnType, item)
                              .CurrentPosition;
        }

        /// <summary>
        /// Create new server and add server to server store!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateNewServer(object sender, EventArgs e)
        {
            // Create server
            UServer = new Server(Director.Locales.Language.NewServerString, Director.Locales.Language.NewServerURL);

            // Add server to tree store
            ServerStore.Clear();

            // Add node
            CreateTreeItem(null, UServer.Name, ServerImage, UServer);

            // Disable server menu
			NewServer.Sensitive = true;
        }


        /// <summary>
        /// Dispose window.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
