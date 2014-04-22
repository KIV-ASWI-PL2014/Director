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
using Director.Forms.Export;
using Director.Forms.Inputs;

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
        private ServerPage ServerBox;

        /// <summary>
        /// Scenario page.
        /// </summary>
        private ScenarioPage ScenarioBox;

        /// <summary>
        /// Request page.
        /// </summary>
        private RequestPage RequestBox;

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
        /// Open scenario.
        /// </summary>
        private MenuItem OpenScenarioMenu;

        /// <summary>
        /// Export scenario menu.
        /// </summary>
        private MenuItem SaveScenarioMenu;

        /// <summary>
        /// Scenario clipboard.
        /// </summary>
        private Scenario ScenarioClipboard { get; set; }

        /// <summary>
        /// Request clipboard.
        /// </summary>
        private Request RequestClipboard { get; set; }

        /// <summary>
        /// Copy scenario menu item.
        /// </summary>
        private MenuItem CopyScenario { get; set; }

        /// <summary>
        /// Paste scenario menu item.
        /// </summary>
        private MenuItem PasteScenario { get; set; }

        /// <summary>
        /// Copy request menu item.
        /// </summary>
        private MenuItem CopyRequest { get; set; }

        /// <summary>
        /// Paste request menu item.
        /// </summary>
        private MenuItem PasteRequest { get; set; }


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
            // Prepare boxes
            ServerBox = new ServerPage(this);
            ScenarioBox = new ScenarioPage(this);
            RequestBox = new RequestPage(this);

            /// Title and Initial size
            Title = Director.Properties.Resources.MainWindowTitle;

            // Set default size
            Width = 750;
			Height = 630;

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

            // TODO: Remove in release!
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
                Director.Properties.Resources.TreeStoreInformation,
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
                    ServerBox.ActualPosition = CurrentServer.SelectedRow;
                    ServerBox.SetServer((Server)s);
                }
                else if (s is Scenario)
                {
                    SetContentBoxControl(ScenarioBox);
                    ScenarioBox.ActualPosition = CurrentServer.SelectedRow;
                    ScenarioBox.SetScenario((Scenario)s);
                }
                else if (s is Request)
                {
                    SetContentBoxControl(RequestBox);
                    RequestBox.ActualPosition = CurrentServer.SelectedRow;
                    RequestBox.SetRequest((Request)s);
                }
                else if (s is Homepage)
                {
                    SetContentBoxControl(DirectorHomepage);
                }
            }
        }


        /// <summary>
        /// Update tree store text item.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="text"></param>
        public void UpdateTreeStoreText(TreePosition position, String text)
        {
            ServerStore.GetNavigatorAt(position).SetValue(ColumnName, text);
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
            MenuItem server = new MenuItem(Director.Properties.Resources.MenuServer);

            // Prepare submenu
            server.SubMenu = _createServerMenu();

            // Add server menu to items
            menu.Items.Add(server);

            // Set as main menu
            MainMenu = menu;

            // Create server menu
            ServerMenu = new Menu();

            // Add scenario item
            MenuItem MenuAddScenario = new MenuItem(Director.Properties.Resources.MenuAddScenario)
            {
                Image = Image.FromResource(DirectorImages.ADD_ICON)
            };
            ServerMenu.Items.Add(MenuAddScenario);
            MenuAddScenario.Clicked += AddScenario;

            // Menu paste scenario
            PasteScenario = new MenuItem(Director.Properties.Resources.MenuPasteScenario)
            {
                Image = Image.FromResource(DirectorImages.PASTE_ICON),
                Sensitive = false
            };
            ServerMenu.Items.Add(PasteScenario);

            // Separator
            ServerMenu.Items.Add(new SeparatorMenuItem());

            // Edit default headers
            MenuItem MenuEditDefaultHeaders = new MenuItem(Director.Properties.Resources.MenuEditDefaultHeaders)
            {
                Image = Image.FromResource(DirectorImages.DEFAULT_HEADERS_ICON)
            };
            MenuEditDefaultHeaders.Clicked += MenuEditDefaultHeaders_Clicked;
            ServerMenu.Items.Add(MenuEditDefaultHeaders);

            // Create Scenario menu
            ScenarioMenu = new Menu();

            // Add request menu
            MenuItem MenuAddRequest = new MenuItem(Director.Properties.Resources.ContextMenuAddRequest)
            {
                Image = Image.FromResource(DirectorImages.ADD_ICON)
            };
            MenuAddRequest.Clicked += AddRequest;
            ScenarioMenu.Items.Add(MenuAddRequest);

            // Paste request
            PasteRequest = new MenuItem(Director.Properties.Resources.MenuPasteRequest)
            {
                Image = Image.FromResource(DirectorImages.PASTE_ICON),
                Sensitive = false
            };
            ScenarioMenu.Items.Add(PasteRequest);

            // Copy scenario
            CopyScenario = new MenuItem(Director.Properties.Resources.MenuCopyScenario)
            {
                Image = Image.FromResource(DirectorImages.COPY_ICON)
            };
            CopyScenario.Clicked += CopyScenario_Clicked;
            ScenarioMenu.Items.Add(CopyScenario);
            // Separator
            ScenarioMenu.Items.Add(new SeparatorMenuItem());

            // Delete scenario menu
            MenuItem MenuRemoveScenario = new MenuItem(Director.Properties.Resources.ContextMenuRemoveScenario)
            {
                Image = Image.FromResource(DirectorImages.CROSS_ICON)
            };
            ScenarioMenu.Items.Add(MenuRemoveScenario);
            MenuRemoveScenario.Clicked += RemoveScenario;

            // Request menu
            RequestMenu = new Menu();
            MenuItem MenuEditRequest = new MenuItem(Director.Properties.Resources.MenuEditRequest)
            {
                Image = Image.FromResource(DirectorImages.EDIT_ICON)
            };
            MenuEditRequest.Clicked += MenuEditRequest_Clicked;
            RequestMenu.Items.Add(MenuEditRequest);

            CopyRequest = new MenuItem(Director.Properties.Resources.MenuCopyRequest)
            {
                Image = Image.FromResource(DirectorImages.COPY_ICON)
            };
            CopyRequest.Clicked += CopyRequest_Clicked;
            RequestMenu.Items.Add(CopyRequest);

            // Separator
            RequestMenu.Items.Add(new SeparatorMenuItem());

            // Remove
            MenuItem MenuRemoveRequest = new MenuItem(Director.Properties.Resources.ContextMenuRemoveRequest)
            {
                Image = Image.FromResource(DirectorImages.CROSS_ICON)
            };
            RequestMenu.Items.Add(MenuRemoveRequest);
        }

        /// <summary>
        /// Edit default server headers!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MenuEditDefaultHeaders_Clicked(object sender, EventArgs e)
        {
            ServerHeaders ServerHeadersWindow = new ServerHeaders(this, UServer);
            ServerHeadersWindow.Show();
            Opacity = 0.1;
            ServerHeadersWindow.Closed += delegate
            {
                Opacity = 1;
            };
        }

        /// <summary>
        /// Copy request click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CopyRequest_Clicked(object sender, EventArgs e)
        {
            TreePosition tmp = CurrentServer.SelectedRow;
            if (tmp == null)
                return;
            var r = ServerStore.GetNavigatorAt(tmp).GetValue(ColumnType);
            if (r is Request)
            {
                if (RequestClipboard == null)
                {
                    PasteRequest.Clicked += PasteRequest_Clicked;
                }
                RequestClipboard = (Request)r;
                PasteRequest.Sensitive = true;
            }
        }

        /// <summary>
        /// Paste request into scenario.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PasteRequest_Clicked(object sender, EventArgs e)
        {
            TreePosition tmp = CurrentServer.SelectedRow;
            if (tmp == null || RequestClipboard == null)
                return;
            var s = ServerStore.GetNavigatorAt(tmp).GetValue(ColumnType);
            if (s is Scenario)
            {
                // Copy
                Request NewRequest = RequestClipboard.Clone();

                // Modify name
                NewRequest.Name += "#copy";

                // Add to scenario
                ((Scenario)s).Requests.Add(NewRequest);

                // Refresh tree view
                CreateTreeItem(tmp, NewRequest.Name, RequestImage, NewRequest);
            }
        }

        /// <summary>
        /// Paste scenario on server position.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PasteScenario_Clicked(object sender, EventArgs e)
        {
            TreePosition tmp = CurrentServer.SelectedRow;
            if (tmp == null || ScenarioClipboard == null)
                return;
            var s = ServerStore.GetNavigatorAt(tmp).GetValue(ColumnType);
            if (s is Server)
            {
                // Copy
                Scenario NewScenario = ScenarioClipboard.Clone();

                // Modify name
                NewScenario.Name += "#copy";

                // Add to server
                UServer.Scenarios.Add(NewScenario);

                // Refresh tree view
                tmp = CreateTreeItem(tmp, NewScenario.Name, ScenarioImage, NewScenario);

                foreach (Request req in NewScenario.Requests)
                    CreateTreeItem(tmp, req.Name, RequestImage, req);
            }
        }

        /// <summary>
        /// Copy scenario into clipboard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CopyScenario_Clicked(object sender, EventArgs e)
        {
            TreePosition tmp = CurrentServer.SelectedRow;
            if (tmp == null)
                return;
            var s = ServerStore.GetNavigatorAt(tmp).GetValue(ColumnType);
            if (s is Scenario)
            {
                if (ScenarioClipboard == null)
                    PasteScenario.Clicked += PasteScenario_Clicked;

                ScenarioClipboard = (Scenario)s;
                PasteScenario.Sensitive = true;
            }
        }

        /// <summary>
        /// Edit request data.
        /// </summary>
        void MenuEditRequest_Clicked(object sender, EventArgs e)
        {
            // Get actual Request
            TreePosition tmp = CurrentServer.SelectedRow;

            if (tmp == null)
                return;

            var s = ServerStore.GetNavigatorAt(tmp).GetValue(ColumnType);

            if (s is Request)
            {
                // Create edit window
                EditWindow editWindow = new EditWindow(this, (Request) s);
                
                // Disable main window
                Opacity = 0.5;

                // Dispozed window opacity to 1
                editWindow.Closed += delegate
                {
                    Opacity = 1;
                    UpdateControlView(null, null);
                };

                // Show
                editWindow.Show();
            }
        }

        /// <summary>
        /// Create server menu.
        /// </summary>
        private Menu _createServerMenu()
        {
            Menu ServerMenu = new Menu();

            // Sub menu server
            NewServer = new MenuItem(Director.Properties.Resources.MenuNewServer)
            {
                Image = Image.FromResource(DirectorImages.NEW_SERVER_ICON)
            };
            NewServer.Clicked += CreateNewServer;
            ServerMenu.Items.Add(NewServer);

            // Scenario open
            OpenScenarioMenu = new MenuItem(Director.Properties.Resources.MenuOpenScenario)
            {
                Image = Image.FromResource(DirectorImages.OPEN_SCENARIO_ICON)
            };
            OpenScenarioMenu.Clicked += OpenNewScenario;
            ServerMenu.Items.Add(OpenScenarioMenu);

            // Export scenario
            SaveScenarioMenu = new MenuItem(Director.Properties.Resources.MenuSaveScenario)
            {
                Image = Image.FromResource(DirectorImages.SAVE_SCENARIO_ICON)
            };
            SaveScenarioMenu.Clicked += SaveScenario;
            ServerMenu.Items.Add(SaveScenarioMenu);

            // Separator before exit
            ServerMenu.Items.Add(new SeparatorMenuItem());

            // Sub menu exit
            MenuItem _exit = new MenuItem(Director.Properties.Resources.MenuExitProgram)
            {
                Image = Image.FromResource(DirectorImages.EXIT_ICON)
            };
            _exit.Clicked += delegate
            {
                Application.Exit();
            };
            ServerMenu.Items.Add(_exit);

            return ServerMenu;
        }

        /// <summary>
        /// Open new scenario.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenNewScenario(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog(Director.Properties.Resources.DialogOpenScenario);
            dlg.Multiselect = false;
            dlg.Filters.Add(new FileDialogFilter("Director files", "*.adfe"));
            if (dlg.Run())
                MessageDialog.ShowMessage("Files: ", string.Join("\n", dlg.FileNames));
        }

        /// <summary>
        /// Export scenario.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveScenario(object sender, EventArgs e)
        {
            ExportWindow ew = new ExportWindow(UServer);
            ew.Show();
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
                bool res = MessageDialog.Confirm(Director.Properties.Resources.MessageBoxRemoveScenario, Command.Ok);
                if (res)
                {
                    // If scenario is in clipboard remove too!
                    if (ScenarioClipboard == (Scenario)s)
                    {
                        ScenarioClipboard = null;
                        PasteScenario.Clicked -= PasteScenario_Clicked;
                        PasteScenario.Sensitive = false;
                    }

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
                UpdateControlView(sender, e);
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
            UServer = new Server(Director.Properties.Resources.NewServerString, Director.Properties.Resources.NewServerURL);

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
