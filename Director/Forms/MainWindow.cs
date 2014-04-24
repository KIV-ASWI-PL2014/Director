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
using Director.ExportLib;

namespace Director.Forms
{
    internal class MainWindow : Window
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
        private MenuItem OpenServerMenu;

        /// <summary>
        /// Export scenario menu.
        /// </summary>
        private MenuItem SaveServerMenu;

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
        /// Close scenario.
        /// </summary>
        private MenuItem CloseServer { get; set; }

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

            // Set buttons to enabled
            DisableServerButtons();
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

            // Control box with bottom info panel
            VBox MainContent = new VBox();
            MainContent.PackStart(MainBox, expand: true, fill: true);

            // Create Info label with link
            LinkLabel _info = new LinkLabel("© NetBrick, s.r.o.")
            {
                Uri = new Uri("http://director.strnadj.eu/")
            };
            MainContent.PackStart(_info, vpos: WidgetPlacement.End, hpos: WidgetPlacement.End);

            // Clear tree view!
            ClearCurrentServerTree();

            // Set content to main box
            Content = MainContent;
        }

        /// <summary>
        /// Clear current server tree - add help box item!
        /// </summary>
        private void ClearCurrentServerTree()
        {
            // Server store
            ServerStore.Clear();

            // Create server store box!
            var pos = CreateTreeItem(
                null,
                Director.Properties.Resources.TreeStoreInformation,
                Image.FromResource(DirectorImages.HELP_ICON),
                DirectorHomepage
                );

            // Select information
            CurrentServer.SelectRow(pos);
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
            ScenarioMenu.Items.Add(new SeparatorMenuItem());

            // Scenario UP and down
            MenuItem ItemUpMenu = new MenuItem(Director.Properties.Resources.OrderUP)
            {
                Image = Image.FromResource(DirectorImages.UP_ICON)
            };
            ItemUpMenu.Clicked += ItemUpMenu_Clicked;
            ScenarioMenu.Items.Add(ItemUpMenu);
            
            MenuItem ItemDownMenu = new MenuItem(Director.Properties.Resources.OrderDOWN)
            {
                Image = Image.FromResource(DirectorImages.DOWN_ICON)
            };
            ItemDownMenu.Clicked += ItemDownMenu_Clicked;
            ScenarioMenu.Items.Add(ItemDownMenu);

            // Add to scenario and request menu
            ScenarioMenu.Items.Add(new SeparatorMenuItem());

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

            RequestMenu.Items.Add(new SeparatorMenuItem());

            // Scenario UP and down
            MenuItem RequestItemUpMenu = new MenuItem(Director.Properties.Resources.OrderUP)
            {
                Image = Image.FromResource(DirectorImages.UP_ICON)
            };
            RequestItemUpMenu.Clicked += ItemUpMenu_Clicked;
            RequestMenu.Items.Add(RequestItemUpMenu);

            MenuItem RequestItemDownMenu = new MenuItem(Director.Properties.Resources.OrderDOWN)
            {
                Image = Image.FromResource(DirectorImages.DOWN_ICON)
            };
            RequestItemDownMenu.Clicked += ItemDownMenu_Clicked;
            RequestMenu.Items.Add(RequestItemDownMenu);

            // Separator
            RequestMenu.Items.Add(new SeparatorMenuItem());

            // Remove
            MenuItem MenuRemoveRequest = new MenuItem(Director.Properties.Resources.ContextMenuRemoveRequest)
            {
                Image = Image.FromResource(DirectorImages.CROSS_ICON)
            };
            RequestMenu.Items.Add(MenuRemoveRequest);
            MenuRemoveRequest.Clicked += MenuRemoveRequest_Clicked;
        }

        /// <summary>
        /// Remove scenario context menu item clicked!
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void MenuRemoveRequest_Clicked(object sender, EventArgs e)
        {
            TreePosition tmp = CurrentServer.SelectedRow;

            if (tmp == null)
                return;

            var s = ServerStore.GetNavigatorAt(tmp).GetValue(ColumnType);

            if (s is Request)
            {
                Request req = (Request)s;
                bool res = MessageDialog.Confirm(Director.Properties.Resources.MessageBoxRemoveRequest, Command.Ok);
                if (res)
                {
                    // If scenario is in clipboard remove too!
                    if (RequestClipboard == req)
                    {
                        ScenarioClipboard = null;
                        PasteScenario.Clicked -= PasteRequest_Clicked;
                        PasteScenario.Sensitive = false;
                    }

                    // Remove scenario
                    req.ParentScenario.RemoveRequest(req);

                    // Remove from tree view
                    ServerStore.GetNavigatorAt(tmp).Remove();

                    // Set position to parent scenario
                    CurrentServer.SelectRow(req.ParentScenario.TreePosition);
                }
            }
        }

        /// <summary>
        /// Item down - move scenario down.
        /// </summary>
        void ItemDownMenu_Clicked(object sender, EventArgs e)
        {
            TreePosition tmp = CurrentServer.SelectedRow;
            if (tmp == null)
                return;
            var r = ServerStore.GetNavigatorAt(tmp).GetValue(ColumnType);

            if (r is Request)
            {
                // Switch requests down
                Request _request = (Request)r;
                var _newRequest = _request.ParentScenario.MoveRequestDown(_request);

                if (_newRequest != null)
                    SwitchRequestObjects(_request, _newRequest);
            }
            else if (r is Scenario)
            {
                // Switch requests down
                Scenario _scenario = (Scenario)r;
                var _newScenario = _scenario.ParentServer.MoveScenarioDown(_scenario);

                if (_newScenario != null)
                    SwitchScenarioObjects(_scenario, _newScenario);
            }
        }

        /// <summary>
        /// Switch scenario ServerStore objects.
        /// </summary>
        private void SwitchScenarioObjects(Scenario _scenario, Scenario _newScenario)
        {
            ServerStore.GetNavigatorAt(_scenario.TreePosition).SetValue(ColumnName, _newScenario.Name);
            ServerStore.GetNavigatorAt(_newScenario.TreePosition).SetValue(ColumnName, _scenario.Name);

            // Switch objects
            ServerStore.GetNavigatorAt(_scenario.TreePosition).SetValue(ColumnType, _newScenario);
            ServerStore.GetNavigatorAt(_newScenario.TreePosition).SetValue(ColumnType, _scenario);

            // Switch positions
            TreePosition tmpTP = _scenario.TreePosition;
            _scenario.TreePosition = _newScenario.TreePosition;
            _newScenario.TreePosition = tmpTP;

            // Switch childrens
            ServerStore.GetNavigatorAt(_scenario.TreePosition).RemoveChildren();
            ServerStore.GetNavigatorAt(_newScenario.TreePosition).RemoveChildren();

            // Create childrens again
            foreach (Request s in _scenario.Requests.OrderBy(n => n.Position))
            {
                s.TreePosition = CreateTreeItem(_scenario.TreePosition, s.Name, RequestImage, s);
            }

            foreach (Request s in _newScenario.Requests.OrderBy(n => n.Position))
            {
                s.TreePosition = CreateTreeItem(_newScenario.TreePosition, s.Name, RequestImage, s);
            }

            // Refresh
            UpdateControlView(null, null);
        }

        /// <summary>
        /// Switch request serer store objects.
        /// </summary>
        private void SwitchRequestObjects(Request _request, Request _newRequest)
        {
            // Switch objects and names in tree view!
            ServerStore.GetNavigatorAt(_request.TreePosition).SetValue(ColumnName, _newRequest.Name);
            ServerStore.GetNavigatorAt(_newRequest.TreePosition).SetValue(ColumnName, _request.Name);

            // Switch objects
            ServerStore.GetNavigatorAt(_request.TreePosition).SetValue(ColumnType, _newRequest);
            ServerStore.GetNavigatorAt(_newRequest.TreePosition).SetValue(ColumnType, _request);

            // Switch positions
            TreePosition tmpTP = _request.TreePosition;
            _request.TreePosition = _newRequest.TreePosition;
            _newRequest.TreePosition = tmpTP;

            // Refresh box
            UpdateControlView(null, null);
        }

        /// <summary>
        /// Move item up.
        /// </summary>
        void ItemUpMenu_Clicked(object sender, EventArgs e)
        {
            TreePosition tmp = CurrentServer.SelectedRow;
            if (tmp == null)
                return;
            var r = ServerStore.GetNavigatorAt(tmp).GetValue(ColumnType);

            if (r is Request)
            {
                // Switch requests down
                Request _request = (Request)r;
                var _newRequest = _request.ParentScenario.MoveRequestUp(_request);

                if (_newRequest != null)
                    SwitchRequestObjects(_request, _newRequest);
            }
            else if (r is Scenario)
            {
                // Switch requests down
                Scenario _scenario = (Scenario)r;
                var _newScenario = _scenario.ParentServer.MoveScenarioUp(_scenario);

                if (_newScenario != null)
                    SwitchScenarioObjects(_scenario, _newScenario);
            }
        }

        /// <summary>
        /// Edit default server headers!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuEditDefaultHeaders_Clicked(object sender, EventArgs e)
        {
            ServerHeaders ServerHeadersWindow = new ServerHeaders(this, UServer);
            ServerHeadersWindow.Show();
            Opacity = 0.1;
            ServerHeadersWindow.Closed += delegate { Opacity = 1; };
        }

        /// <summary>
        /// Copy request click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyRequest_Clicked(object sender, EventArgs e)
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
        private void PasteRequest_Clicked(object sender, EventArgs e)
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
                var position = CreateTreeItem(tmp, NewRequest.Name, RequestImage, NewRequest);

                // Add position to item
                NewRequest.TreePosition = position;
            }
        }

        /// <summary>
        /// Paste scenario on server position.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasteScenario_Clicked(object sender, EventArgs e)
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
                tmp = NewScenario.TreePosition = CreateTreeItem(tmp, NewScenario.Name, ScenarioImage, NewScenario);

                // Set all scenarios 
                foreach (Request req in NewScenario.Requests)
                {
                    req.TreePosition = CreateTreeItem(tmp, req.Name, RequestImage, req);
                }
            }
        }

        /// <summary>
        /// Copy scenario into clipboard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyScenario_Clicked(object sender, EventArgs e)
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
        private void MenuEditRequest_Clicked(object sender, EventArgs e)
        {
            // Get actual Request
            TreePosition tmp = CurrentServer.SelectedRow;

            if (tmp == null)
                return;

            var s = ServerStore.GetNavigatorAt(tmp).GetValue(ColumnType);

            if (s is Request)
                OpenEditRequest((Request)s);
        }

        /// <summary>
        /// Open edit request window and wait until finish!
        /// </summary>
        /// <param name="request"></param>
        public void OpenEditRequest(Request request)
        {
            // Create edit window
            EditWindow editWindow = new EditWindow(this, request);

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
            ServerMenu.Items.Add(NewServer);

            // Scenario open
            OpenServerMenu = new MenuItem(Director.Properties.Resources.MenuOpenScenario)
            {
                Image = Image.FromResource(DirectorImages.OPEN_SCENARIO_ICON)
            };
            ServerMenu.Items.Add(OpenServerMenu);

            // Export scenario
            SaveServerMenu = new MenuItem(Director.Properties.Resources.MenuSaveScenario)
            {
                Image = Image.FromResource(DirectorImages.SAVE_SCENARIO_ICON)
            };
            ServerMenu.Items.Add(SaveServerMenu);

            // Close scenario
            CloseServer = new MenuItem(Director.Properties.Resources.MenuCloseScenario)
            {
                Image = Image.FromResource(DirectorImages.CROSS_ICON),
                Sensitive = false
            };
            ServerMenu.Items.Add(CloseServer);

            // Separator before exit
            ServerMenu.Items.Add(new SeparatorMenuItem());

            // Sub menu exit
            MenuItem _exit = new MenuItem(Director.Properties.Resources.MenuExitProgram)
            {
                Image = Image.FromResource(DirectorImages.EXIT_ICON)
            };
            _exit.Clicked += delegate { Application.Exit(); };
            ServerMenu.Items.Add(_exit);

            return ServerMenu;
        }

        /// <summary>
        /// Disable server buttons (new, open).
        /// </summary>
        private void DisableServerButtons()
        {
            NewServer.Clicked += CreateNewServer;
            NewServer.Sensitive = true;
            OpenServerMenu.Clicked += OpenNewServer;
            OpenServerMenu.Sensitive = true;

            SaveServerMenu.Sensitive = false;
            SaveServerMenu.Clicked -= SaveServer;
            CloseServer.Sensitive = false;
            CloseServer.Clicked -= CloseServer_Clicked;
        }

        /// <summary>
        /// Enable server buttons.
        /// </summary>
        private void EnableServerButtons()
        {
            NewServer.Clicked -= CreateNewServer;
            NewServer.Sensitive = false;
            OpenServerMenu.Clicked -= OpenNewServer;
            OpenServerMenu.Sensitive = false;

            SaveServerMenu.Sensitive = true;
            SaveServerMenu.Clicked += SaveServer;
            CloseServer.Sensitive = true;
            CloseServer.Clicked += CloseServer_Clicked;
        }

        /// <summary>
        /// Close server menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CloseServer_Clicked(object sender, EventArgs e)
        {
            bool res = MessageDialog.Confirm(Director.Properties.Resources.MessageBoxCloseScenario, Command.Ok);
            if (res)
            {
                UServer = null;
                ClearCurrentServerTree();
                UpdateControlView(null, null);
                DisableServerButtons();
            }
        }

        /// <summary>
        /// Open new scenario.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenNewServer(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog(Director.Properties.Resources.DialogOpenScenario);
            dlg.Multiselect = false;
            dlg.Filters.Add(new FileDialogFilter("Director files", "*.adfe"));
            if (dlg.Run())
            {
                try
                {
                    UServer = Deserialization.DeserializeAll(dlg.FileNames[0]);
                }
                catch
                {
                    MessageDialog.ShowError(Director.Properties.Resources.DialogInvalidFile);
                    return;
                }
                
                // Server tree view reconstruction
                ClearCurrentServerTree();

                // Add node
                var tmp = UServer.TreePosition = CreateTreeItem(null, UServer.Name, ServerImage, UServer);

                // Disable server menu
                NewServer.Sensitive = false;

                // Disable server menu mac!
                NewServer.Clicked -= CreateNewServer;

                // Create all scenarios and requests
                foreach (Scenario s in UServer.Scenarios.OrderBy(n => n.Position))
                {
                    s.TreePosition = CreateTreeItem(tmp, s.Name, ScenarioImage, s);

                    foreach (Request r in s.Requests.OrderBy(n => n.Position))
                    {
                        r.TreePosition = CreateTreeItem(s.TreePosition, r.Name, RequestImage, r);
                    }
                }

                // Select Server and update view
                CurrentServer.SelectRow(tmp);
                UpdateControlView(null, null);

                // Enable close sceanrio
                EnableServerButtons();
            }

        }

        /// <summary>
        /// Export scenario.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveServer(object sender, EventArgs e)
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
                    if (ScenarioClipboard == sc)
                    {
                        ScenarioClipboard = null;
                        PasteScenario.Clicked -= PasteScenario_Clicked;
                        PasteScenario.Sensitive = false;
                    }

                    // Remove scenario
                    UServer.RemoveScenario(sc);

                    // Remove from tree view
                    ServerStore.GetNavigatorAt(tmp).Remove();

                    // Set position to server
                    CurrentServer.SelectRow(UServer.TreePosition);
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
                NewScenario.TreePosition = CreateTreeItem(tmp, NewScenario.Name, ScenarioImage, NewScenario);
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
                NewRequest.TreePosition = CreateTreeItem(tmp, NewRequest.Name, RequestImage, NewRequest);
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
            UServer = new Server(Director.Properties.Resources.NewServerString,
                Director.Properties.Resources.NewServerURL);

            // Add server to tree store
            ClearCurrentServerTree();

            // Add node
            UServer.TreePosition = CreateTreeItem(null, UServer.Name, ServerImage, UServer);

            // Disable server menu
            NewServer.Sensitive = false;

            // Disable server menu mac!
            NewServer.Clicked -= CreateNewServer;

            // Enable close server
            EnableServerButtons();
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