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
        private ServerPage ServerBox = new ServerPage();

        /// <summary>
        /// Scenario page.
        /// </summary>
        private ScenarioPage ScenarioBox = new ScenarioPage();

        /// <summary>
        /// Request page.
        /// </summary>
        private RequestPage RequestBox = new RequestPage();

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
        private void _initializeComponents()
        {
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
            CurrentServer.SelectionChanged += UpdateControlView;

            // Create tree view!
            ServerStore = new TreeStore(ColumnName, ColumnImage, ColumnType);
            CurrentServer.Columns.Add("Server", ColumnImage, ColumnName); // Dummy column, not sure why?
            CurrentServer.HeadersVisible = false;

            // Clear tree view!
            ClearCurrentServerTree();

            // Net item parsing
            CurrentServer.ButtonPressed += HandleMouseOnTreeView;

            // Set data source
            CurrentServer.DataSource = ServerStore;

            // Add server to main box
            MainBox.PackStart(CurrentServer);

            // Prepare content box
            ContentBox = new VBox();
            ContentBox.MarginLeft = 10;
            SetContentBoxControl(DirectorHomepage);

            // Add to panel
            MainBox.PackStart(ContentBox, true, true);

            // Set content to main box
            Content = MainBox;

            CreateNewServer(null, null);
        }

        /// <summary>
        /// Clear current server tree - add help box item!
        /// </summary>
        private void ClearCurrentServerTree()
        {
            // Server store
            ServerStore.Clear();

            // Create server store box!
            CreateTreeItem(
                null,
                Director.Locales.Language.TreeStoreInformation,
                Image.FromResource(DirectorImages.HELP_ICON),
                DirectorHomepage
            );
        }

        /// <summary>
        /// Set main control box.
        /// </summary>
        /// <param name="box">Box.</param>
        private void SetContentBoxControl(VBox box)
        {
            if (CurrentControl != null)
                ContentBox.Remove(CurrentControl);

            ContentBox.PackStart(box, expand: true, fill: true);
            CurrentControl = box;
        }

        /// <summary>
        /// Handler react on tree view selection changes.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void UpdateControlView(object sender, EventArgs e)
        {
            if (CurrentServer.SelectedRow != null)
            {
                var s = ServerStore.GetNavigatorAt(CurrentServer.SelectedRow).GetValue(ColumnType);
                if (s is Server)
                {
                    SetContentBoxControl(ServerBox);
                    ServerBox.SetServer((Server)s);
                }
                else if (s is Scenario)
                {
                    SetContentBoxControl(ScenarioBox);
                }
                else if (s is Request)
                {
                    SetContentBoxControl(RequestBox);
                }
                else if (s is Homepage)
                {
                    SetContentBoxControl(DirectorHomepage);
                }
            }
        }

        /// <summary>
        /// Right click mouse handler on tree view!
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void HandleMouseOnTreeView(object sender, ButtonEventArgs e)
        {
            if (e.Button == PointerButton.Right)
            {
                TreePosition tmp;
                RowDropPosition tmp2;

                if (CurrentServer.GetDropTargetRow(e.X, e.Y, out tmp2, out tmp))
                {
                    // Set row
                    CurrentServer.SelectRow(tmp);

                    // Get row data - for proper ContextMenu
                    var data = ServerStore.GetNavigatorAt(tmp).GetValue(ColumnType);

                    if (data is Server)
                    {
                        ServerMenu.Popup(CurrentServer, e.X, e.Y);
                    }
                    else if (data is Scenario)
                    {
                        ScenarioMenu.Popup(CurrentServer, e.X, e.Y);
                    }
                    else if (data is Request)
                    {
                        RequestMenu.Popup(CurrentServer, e.X, e.Y);
                    }
                }
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

            // Create Scenario menu
            ScenarioMenu = new Menu();

            // Add request menu
            MenuItem MenuAddRequest = new MenuItem(Director.Locales.Language.ContextMenuAddRequest)
            {
                Image = Image.FromResource(DirectorImages.ADD_ICON)
            };
            MenuAddRequest.Clicked += AddRequest;
            ScenarioMenu.Items.Add(MenuAddRequest);

            // Separator
            ScenarioMenu.Items.Add(new SeparatorMenuItem());

            // Delete scenario menu
            MenuItem MenuRemoveScenario = new MenuItem(Director.Locales.Language.ContextMenuRemoveScenario)
            {
                Image = Image.FromResource(DirectorImages.CROSS_ICON)
            };
            ScenarioMenu.Items.Add(MenuRemoveScenario);
            MenuRemoveScenario.Clicked += RemoveScenario;

            // Request menu
            RequestMenu = new Menu();
            MenuItem MenuRemoveRequest = new MenuItem(Director.Locales.Language.ContextMenuRemoveRequest)
            {
                Image = Image.FromResource(DirectorImages.CROSS_ICON)
            };
            RequestMenu.Items.Add(MenuRemoveRequest);
        }

        /// <summary>
        /// Remove scenario context menu item clicked!
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void RemoveScenario(object sender, EventArgs e)
        {
            TreePosition tmp = CurrentServer.SelectedRow;

            if (tmp == null)
                return;

            var s = ServerStore.GetNavigatorAt(tmp).GetValue(ColumnType);

            if (s is Scenario)
            {
                Scenario sc = (Scenario)s;
                bool res = MessageDialog.Confirm(Director.Locales.Language.MessageBoxRemoveScenario, Command.Ok);
                if (res)
                {
                    // Remove scenario
                    UServer.RemoveScenario(sc);

                    // Remove from tree view
                    ServerStore.GetNavigatorAt(tmp).Remove();
                }
            }
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
        /// Create new request belongs to selected scenario.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void AddRequest(object sender, EventArgs e)
        {
            TreePosition tmp = CurrentServer.SelectedRow;

            if (tmp == null)
                return;

            var data = ServerStore.GetNavigatorAt(tmp).GetValue(ColumnType);

            if (data is Scenario)
            {
                Scenario s = (Scenario)data;
                Request NewRequest = s.CreateNewRequest();
                CreateTreeItem(tmp, NewRequest.Name, RequestImage, NewRequest);
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
            ClearCurrentServerTree();

            // Add node
            CreateTreeItem(null, UServer.Name, ServerImage, UServer);

            // Disable server menu
            NewServer.Sensitive = false;

            // Disable server menu mac!
            NewServer.Clicked -= CreateNewServer;
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
