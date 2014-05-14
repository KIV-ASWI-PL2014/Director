using System;
using System.Collections.Generic;
using System.Linq;
using Xwt;
using Director.Forms.Panels;
using Xwt.Drawing;
using Director.Forms.Controls;
using Director.DataStructures;
using Director.Forms.Export;
using Director.Forms.Inputs;
using Director.ExportLib;
using RestSharp;
using Director.ParserLib;
using Director.Formatters;
using System.Threading;
using Director.Import.Apiary;
using Director.Import.Postman;

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
        private static Image ServerImage = Image.FromResource(DirectorImages.ROOT_ICON);

        /// <summary>
        /// Scenario image.
        /// </summary>
        private static Image ScenarioImage = Image.FromResource(DirectorImages.SCENARIO_ICON);

        /// <summary>
        /// Request image.
        /// </summary>
        private static Image RequestImage = Image.FromResource(DirectorImages.REQUEST_ICON);

        /// <summary>
        /// OK image.
        /// </summary>
        private static Image OKImage = Image.FromResource(DirectorImages.OK_ICON);

        /// <summary>
        /// Error image.
        /// </summary>
        private static Image ERORImage = Image.FromResource(DirectorImages.CROSS_ICON);

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
        /// Run all menu item.
        /// </summary>
        private MenuItem RunAllMenu { get; set; }

        /// <summary>
        /// Export variables state!
        /// </summary>
        private MenuItem ExportVariablesMenu { get; set; }

        /// <summary>
        /// Stop menu.
        /// </summary>
        private MenuItem StopThreadMenu { get; set; }

        /// <summary>
        /// Caption font!
        /// </summary>
        public static Font CaptionFont { get; set; }

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

            // Icon
            Icon = Image.FromResource(DirectorImages.ROOT_ICON);

            // Set default size
            Width = 770;
            Height = 630;

            // Caption font
            CaptionFont = Font.SystemFont.WithSize(17).WithWeight(FontWeight.Bold);

            // Center screen
            InitialLocation = WindowLocation.CenterScreen;

            // Initialize menu
            _initializeMenu();

            // Initialize content
            _intializeBoxes();

            // Set buttons to enabled
            DisableServerButtons();

            // Runn progressing images
            _runProgressImages();
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
            CurrentServer.WidthRequest = 230;
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
            ContentBox = new VBox()
            {
                MarginLeft = 10
            };
            SetContentBoxControl(DirectorHomepage);

            // Add to panel
            MainBox.PackStart(ContentBox, expand: true, fill: true);

            // Control box with bottom info panel
            VBox MainContent = new VBox();
            MainContent.PackStart(MainBox, expand: true, fill: true);

            // Create Info label with link
			LinkLabel _info = new LinkLabel("Copyright 2014 NetBrick, s.r.o.")
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
                        ServerMenu.Popup();
                    }
                    else if (data is Scenario)
                    {
                        ScenarioMenu.Popup();
                    }
                    else if (data is Request)
                    {
                        RequestMenu.Popup();
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

            // Create running menu
            MenuItem RunMenu = new MenuItem(Director.Properties.Resources.MenuRun);

            // Create submenu
            RunMenu.SubMenu = _createRunSubmenu();

            // Add
            menu.Items.Add(RunMenu);

            // Import menu
            MenuItem ImportMenu = new MenuItem(Director.Properties.Resources.Import);
            menu.Items.Add(ImportMenu);
            ImportMenu.SubMenu = _createImportMenu();

            // Settings
            MenuItem SettingsMenu = new MenuItem(Director.Properties.Resources.SettingsMenu);
            menu.Items.Add(SettingsMenu);
            SettingsMenu.SubMenu = new Menu();

            // Language
            MenuItem Languages = new MenuItem(Director.Properties.Resources.Language)
            {
                Image = Image.FromResource(DirectorImages.LIST_ICON)
            };
            SettingsMenu.SubMenu.Items.Add(Languages);

            // Langauges submenu
            Menu LanguagesSubMenu = new Menu();
            Languages.SubMenu = LanguagesSubMenu;

            // Get locale
            string locale = Director.Properties.Settings.Default.language;

            // English
            MenuItem LocaleEnglish = new MenuItem("English")
            {
                Image = Image.FromResource(DirectorImages.EN_ICON)
            };
            LanguagesSubMenu.Items.Add(LocaleEnglish);
            LocaleEnglish.Clicked += delegate
            {
                Director.Properties.Settings.Default.language = "en";
                Director.Properties.Settings.Default.Save();
                MessageDialog.ShowMessage(Director.Properties.Resources.RestartApp);
            };

            // Czech
            MenuItem LocaleCzech = new MenuItem("Czech")
            {
                Image = Image.FromResource(DirectorImages.CS_ICON)
            };
            LanguagesSubMenu.Items.Add(LocaleCzech);
            LocaleCzech.Clicked += delegate
            {
                Director.Properties.Settings.Default.language = "cs";
                Director.Properties.Settings.Default.Save();
                MessageDialog.ShowMessage(Director.Properties.Resources.RestartApp);
            };

            // Help menu
            MenuItem HelpMenu = new MenuItem(Director.Properties.Resources.HelpMenu);
            menu.Items.Add(HelpMenu);

            // Help menu
            Menu HelpSubMenu = new Menu();
            HelpMenu.SubMenu = HelpSubMenu;

            // About menu
            MenuItem AboutMenu = new MenuItem(Director.Properties.Resources.About)
            {
                Image = Image.FromResource(DirectorImages.HELP_ICON)
            };
            HelpSubMenu.Items.Add(AboutMenu);
            AboutMenu.Clicked += delegate
            {
                About.About AboutWindow = new About.About();
                AboutWindow.Run();
                AboutWindow.Dispose();
            };

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
            

            // Run scenario
            MenuItem MenuRunScenario = new MenuItem(Director.Properties.Resources.RunScenario)
            {
                Image = Image.FromResource(DirectorImages.RUN_ICON)
            };
            ScenarioMenu.Items.Add(MenuRunScenario);
            MenuRunScenario.Clicked += RunScenario;

            MenuItem MenuRunScVariables = new MenuItem(Director.Properties.Resources.SetVariables)
			{
				Image = Image.FromResource(DirectorImages.VARIABLES_ICON)
			};
			ScenarioMenu.Items.Add (MenuRunScVariables);
			MenuRunScVariables.Clicked += RunScenarioWithVariables;

            // Separator
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
        /// Create import menu.
        /// </summary>
        /// <returns></returns>
        private Menu _createImportMenu()
        {
            Menu _imM = new Menu();

            // Apiary
            MenuItem ApiaryImport = new MenuItem(Director.Properties.Resources.ImportApiary)
            {
                Image = Image.FromResource(DirectorImages.APIARY_ICON)
            };
            _imM.Items.Add(ApiaryImport);
            ApiaryImport.Clicked += ApiaryImport_Clicked;

            // Postman
            MenuItem PostmanImport = new MenuItem(Director.Properties.Resources.ImportPostman)
            {
                Image = Image.FromResource(DirectorImages.POSTMAN_ICON)
            };
            _imM.Items.Add(PostmanImport);
            PostmanImport.Clicked += PostmanImport_Clicked;


            // Return
            return _imM;
        }

        /// <summary>
        /// Postman import.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PostmanImport_Clicked(object sender, EventArgs e)
        {

            // Show open file dialog
            OpenFileDialog of = new OpenFileDialog(Director.Properties.Resources.SelectJsonFile);
            of.Multiselect = false;
            of.Filters.Add(new FileDialogFilter("Json files", "*.json"));
            if (of.Run())
            {
                Server tmp = UServer;
                if (tmp == null)
                {
                    tmp = new Server();
                }

                // Process result
                Boolean result = PostMan.ProcessPostmanFile(tmp, of.FileName);

                // Set again
                UServer = tmp;

                if (result)
                {
                    // Refresh tree
                    RefreshServerTreeView();

                    // Enable close sceanrio
                    EnableServerButtons();
                }
            }
        }

        /// <summary>
        /// Start apiary import.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ApiaryImport_Clicked(object sender, EventArgs e)
        {
            // Info
            MessageDialog.ShowMessage(Director.Properties.Resources.InfoApiarySnowcrash);

            // Show open file dialog
            OpenFileDialog of = new OpenFileDialog(Director.Properties.Resources.SelectJsonFile);
            of.Multiselect = false;
            of.Filters.Add(new FileDialogFilter("Json files", "*.json"));
            if (of.Run())
            {
                Server tmp = UServer;
                if (tmp == null)
                {
                    tmp = new Server();
                }

                // Process result
                Boolean result = Apiary.ProcessApiaryFile(tmp, of.FileName);

                // Set again
                UServer = tmp;

                if (result)
                {
                    // Refresh tree
                    RefreshServerTreeView();

                    // Enable close sceanrio
                    EnableServerButtons();
                }
            }
        }

        /// <summary>
        /// Create run submenu.
        /// </summary>
        /// <returns></returns>
        private Menu _createRunSubmenu()
        {
            Menu _runSubmenu = new Menu();

            // Run all
            RunAllMenu = new MenuItem(Director.Properties.Resources.RunAllMenu)
            {
                Image = Image.FromResource(DirectorImages.RUN_ICON),
                Sensitive = false
            };
            _runSubmenu.Items.Add(RunAllMenu);

            // Stop
            StopThreadMenu = new MenuItem(Director.Properties.Resources.Stop)
            {
                Image = Image.FromResource(DirectorImages.CROSS_ICON),
                Sensitive = false
            };
            _runSubmenu.Items.Add(StopThreadMenu);

            // Separator
            _runSubmenu.Items.Add(new SeparatorMenuItem());

            // Export variables
            ExportVariablesMenu = new MenuItem(Director.Properties.Resources.ExportVariablesStates)
            {
                Image = Image.FromResource(DirectorImages.EXPORT_VARIABLES_ICON),
                Sensitive = false
            };
            _runSubmenu.Items.Add(ExportVariablesMenu);

            return _runSubmenu;
        }

		/// <summary>
		/// Run scenario with variables.
		/// </summary>
		private void RunScenarioWithVariables(object sender, EventArgs e)
		{
			TreePosition tmp = CurrentServer.SelectedRow;

			if (tmp == null)
				return;

			var data = ServerStore.GetNavigatorAt(tmp).GetValue(ColumnType);

			if (data is Scenario)
			{
				if (WorkingThread != null)
				{
					MessageDialog.ShowError(Director.Properties.Resources.CanNotStartScenario);
					return;
				}

				Scenario s = (Scenario)data;

				// Show dialog
				var variableDialog = new SelectVariables (s);

                // Status
                var stat = variableDialog.Run();

				// Save and run | Ok - save variables
                if (stat != null && stat != Command.Cancel)
                    s.customVariables = variableDialog.NewVariableList();
            
                // Run
                if (stat == Command.Ok)
                {
                    WorkingThread = new Thread(ThreadWorker);
                    WorkingThread.IsBackground = true;
                    WorkingThread.Start(s);
                }

                // Update view
                UpdateControlView(sender, e);

				variableDialog.Dispose ();
			}
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
            ServerHeadersWindow.Run();
            ServerHeadersWindow.Dispose();
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
            SaveServerMenu = new MenuItem(Director.Properties.Resources.MenuSaveServer)
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

            RunAllMenu.Sensitive = false;
            StopThreadMenu.Sensitive = false;
            ExportVariablesMenu.Sensitive = false;
			try {
				RunAllMenu.Clicked -= RunAllMenu_Clicked;
				SaveServerMenu.Clicked -= SaveServer;
				CloseServer.Clicked -= CloseServer_Clicked;
                StopThreadMenu.Clicked -= StopThreadMenu_Clicked;
                ExportVariablesMenu.Clicked -= ExportVariablesMenu_Clicked;
			} catch {

			}
            SaveServerMenu.Sensitive = false;
            CloseServer.Sensitive = false;
        }

        /// <summary>
        /// Export variables.
        /// </summary>
        void ExportVariablesMenu_Clicked(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog(Director.Properties.Resources.ExportVariablesStates)
            {
                Multiselect = false,
                InitialFileName = "variables.xml"
            };
            dlg.Filters.Add(new FileDialogFilter("XML format document", "*.xml"));
            if (dlg.Run() && dlg.FileNames.Count() == 1)
            {
                ExportVariables.Export(UServer, dlg.FileName);
            }
        }

        /// <summary>
        /// Running objects for timer.
        /// </summary>
        public static List<TreePosition> RunningObjects = new List<TreePosition>();

        /// <summary>
        /// Last index.
        /// </summary>
        public int LastIndex = 0;

        /// <summary>
        /// Invoke time and change images.
        /// </summary>
        private void _runProgressImages()
        {
            Xwt.Application.TimeoutInvoke(TimeSpan.FromMilliseconds(100), () =>
            {
                foreach (var pos in RunningObjects.ToArray())
                    ChangeTreeIcon(pos, DirectorImages.RUN_ICONS[LastIndex]);

                while (ChangeTreeImage.Count != 0)
                {
                    TreeChange it = ChangeTreeImage.Dequeue();
                    if (it.NewImage != null)
                    {
                        ChangeTreeIcon(it.Position, it.NewImage);
                    }
                    if (it.Text != null && it.Text.Length > 0)
                    {
                        UpdateTreeStoreText(it.Position, it.Text);
                    }
                }

                // Increase
                LastIndex = (LastIndex + 1) > 7 ? 0 : LastIndex + 1;

				// Refresh server
				if (RefreshCurrentServer) {
					UpdateControlView(null, null);
					RefreshCurrentServer = false;
				}

                return true;
            });
        }

        /// <summary>
        /// Tree change structure.c
        /// </summary>
        public struct TreeChange {
            public TreePosition Position;
            public Image NewImage;
            public String Text;
        }

        /// <summary>
        /// Change tree images!
        /// </summary>
        public static Queue<TreeChange> ChangeTreeImage = new Queue<TreeChange>();

        /// <summary>
        /// Running request worker.
        /// </summary>
        public static void ThreadWorker(object RunningObject)
        {
            // Clear running objects
            List<Scenario> Scenarios = new List<Scenario>();
            RunningObjects.Clear();

            // Add all scenarios
            if (RunningObject is Server)
            {
                Scenarios = ((Server)RunningObject).Scenarios;
            }
            else
                Scenarios.Add((Scenario)RunningObject);

            // Set all scenarios to before-running time!
            foreach (var s in Scenarios)
            {
                // Change icon
                ChangeIcon(s.TreePosition, ScenarioImage);

				// Create variables if not exists
				if (s.customVariables == null)
                	s.customVariables = new Dictionary<string, string>();

                // Change icons
                foreach (var r in s.Requests)
                    ChangeIcon(r.TreePosition, RequestImage);
            }

            // First
            bool first = true;

            // Run scenarios in specific order
            foreach (var s in Scenarios.OrderBy(n => n.Position))
            {
                // Running success?
                bool success = true;

                // Scenariio is running!
                RunningObjects.Add(s.TreePosition);

                // Waiting time
                if (first == false && s.TimeAfterPrevious > 0)
                {
                    int time = s.TimeAfterPrevious;
                    while (time > 0)
                    {
                        ChangeIcon(s.TreePosition, text: string.Format("{0} ({1} s)", s.Name, time));
                        Thread.Sleep(1000);
                        time--;
                    }
                    ChangeIcon(s.TreePosition, text: s.Name);
                }
                first = false;

                foreach (var r in s.Requests.OrderBy(n => n.Position))
                {
                    // Add to running objects
                    RunningObjects.Add(r.TreePosition);

                    // Wait?
                    if (r.WaitAfterPreviousRequest > 0)
                    {
                        int time = r.WaitAfterPreviousRequest;
                        while (time > 0)
                        {
                            ChangeIcon(r.TreePosition, text: string.Format("{0} ({1} s)", r.Name, time));
                            Thread.Sleep(1000);
                            time--;
                        }
                        ChangeIcon(r.TreePosition, text: r.Name);
                    }

                    // Clear result
                    r.ClearResults();

                    // Parser
                    Parser p = new Parser();

					// Prepare URL
					ParserResult result = Parser.parseHeader(r.Url, r.ParentScenario.customVariables);
					if (result.isSuccess () == false) {
						// Headers
						r.AddResultViewItem(1, Director.Properties.Resources.ErrorGeneratingUrl + ":");

						// Errors
						foreach (var i in result.getErrors())
						{
							r.AddResultViewItem(2, i.getMessage());
						}

						// Change icon
						ChangeIcon(r.TreePosition, ERORImage);
						success = false;
						break;
					}
					// Erro result
					String URL = result.getResult ();

					// Body
					String RequestBody = null;

					// Create RestRequest
					var request = new RestRequest(r.RequestMethod);

                    // Prepare template
                    if (r.RequestTemplate != null && r.RequestTemplate.Length > 0)
                    {
                        result = p.generateRequest(r.RequestTemplate, r.ParentScenario.customVariables);

                        // Cannot generate reqeust!
                        if (result.isSuccess() == false)
                        {
                            // Write errors!
                            r.AddResultViewItem(1, Director.Properties.Resources.ErrorWhenGeneratingTemplate + ":");

                            // Errors
                            foreach (var i in result.getErrors())
                            {
                                r.AddResultViewItem(2, i.getMessage());
                            }

                            // Change icon
                            ChangeIcon(r.TreePosition, ERORImage);
                            success = false;
                            break;
                        }
                            
                        // Parameter
						if (result.getResult() != null && result.getResult().Length > 0)
							request.AddParameter( "application/json", result.getResult(), ParameterType.RequestBody);
                    }
						
					// Prepare headers
					foreach (var h in r.Headers) {
						result = Parser.parseHeader(h.Value, r.ParentScenario.customVariables);
						if (result.isSuccess () == false) {
							// Headers
							r.AddResultViewItem(1, Director.Properties.Resources.ErrorWhenGeneratingHeaders + ":");

							// Errors
							foreach (var i in result.getErrors())
							{
								r.AddResultViewItem(2, i.getMessage());
							}

							// Change icon
							ChangeIcon(r.TreePosition, ERORImage);
							success = false;
							break;
						}
						// Add header
						request.AddHeader (h.Name, result.getResult ());
					}

                    // Prepare error...
                    r.AddResultViewItem(1, Director.Properties.Resources.Error + ":");

                    // Send
					RestResponse response = Director.Remote.Remote.SendRemoteRequest(r, request, URL);

                    // Valid?
                    bool valid = false;

                    // Response
                    p = new Parser();
                    ParserResult res = null;
                        
                    if (r.ResponseTemplate != null && r.ResponseTemplate.Length > 0)
                        res = p.parseResponse(r.ResponseTemplate, response.Content, s.customVariables, true);


                    // Parse response
                    if (response.ResponseStatus != ResponseStatus.Completed)
                    {
                        r.AddResultViewItem(2, string.Format(Director.Properties.Resources.RequestNotSuccessfull, response.ResponseStatus.ToString()));
                    }
                    else if (r.ExpectedStatusCode != -1 && ((int)response.StatusCode) != r.ExpectedStatusCode)
                    {
                        // Set error message:
                        r.AddResultViewItem(2, string.Format(Director.Properties.Resources.InvalidReturnCode, r.ExpectedStatusCode, (int)response.StatusCode));
                    }
                    else if (res is ParserResult && !res.isSuccess())
                    {
                        foreach (var er in res.getErrors())
                            r.AddResultViewItem(2, er.getMessage());
                    }
                    else
                    {
                        valid = true;
                        r.ClearResults();
                        r.AddResultViewItem(1, Director.Properties.Resources.ResultIsOk);
                    }

                    // Add request content
                    if (RequestBody != null && RequestBody.Length > 0)
                    {
                        r.AddResultViewItem(1, Director.Properties.Resources.RequestTab + ":");

                        // Response
                        String formatted = JSONFormatter.Format(RequestBody);

                        // Request box
                        if (formatted != null)
                            r.AddResultViewItem(3, formatted);
                        else
                            r.AddResultViewItem(3, RequestBody);
                    }

                    // Add response content
                    if (response.Content != null && response.Content.Length > 0)
                    {
                        r.AddResultViewItem(1, Director.Properties.Resources.Response + ":");

                        // Response
                        String formatted = JSONFormatter.Format(response.Content);

                        // Request box
                        if (formatted != null)
                            r.AddResultViewItem(3, formatted);                       
                        else
                            r.AddResultViewItem(3,  response.Content);  
                    }

                    // Refresh UI
                    RefreshCurrentServer = true;

                    // Remove from running
                    RunningObjects.Remove(r.TreePosition);

                    // Set error image
                    if (valid)
                        ChangeIcon(r.TreePosition, OKImage);
                    else
                    {
                        ChangeIcon(r.TreePosition, ERORImage);
                        success = false;
                        break;
                    }
                }

                RunningObjects.Remove(s.TreePosition);
                if (success)
                    ChangeIcon(s.TreePosition, ScenarioImage);
                else
                    ChangeIcon(s.TreePosition, ERORImage);
            }

            // Remove all running objects
            RunningObjects.Clear();

            // Remove thread
            WorkingThread = null;
        }

		/// <summary>
		/// Refresh current server for Timer!
		/// </summary>
		public static bool RefreshCurrentServer = false;

        /// <summary>
        /// Running thread?
        /// </summary>
        public static Thread WorkingThread = null;

        /// <summary>
        /// Change icon or text.
        /// </summary>
        public static void ChangeIcon(TreePosition p, Image i = null, String text = null)
        {
            ChangeTreeImage.Enqueue(new TreeChange() { Position = p, NewImage = i, Text = text });
        }

        /// <summary>
        /// Runn all scenarios.
        /// </summary>
        void RunAllMenu_Clicked(object sender, EventArgs e)
        {
            if (WorkingThread != null)
            {
                MessageDialog.ShowError(Director.Properties.Resources.CanNotStartScenario);
                return;
            }

            WorkingThread = new Thread(ThreadWorker);
            WorkingThread.IsBackground = true;
            WorkingThread.Start(UServer);
        }

        /// <summary>
        /// Stop thread menu.
        /// </summary>
        void StopThreadMenu_Clicked(object sender, EventArgs e)
        {
            try
            {
                WorkingThread.Abort();
            }
            catch
            {

            }
            WorkingThread = null;
            RunningObjects.Clear();
        }

        /// <summary>
        /// Change tree icon on posiiton.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="icon"></param>
        void ChangeTreeIcon(TreePosition pos, Image icon)
        {
            ServerStore.GetNavigatorAt(pos).SetValue(ColumnImage, icon);
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

            RunAllMenu.Sensitive = true;
            RunAllMenu.Clicked += RunAllMenu_Clicked;
            ExportVariablesMenu.Sensitive = true;
            ExportVariablesMenu.Clicked += ExportVariablesMenu_Clicked;
            SaveServerMenu.Sensitive = true;
            SaveServerMenu.Clicked += SaveServer;
            CloseServer.Sensitive = true;
            CloseServer.Clicked += CloseServer_Clicked;
            StopThreadMenu.Sensitive = true;
            StopThreadMenu.Clicked += StopThreadMenu_Clicked;
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
            RunningObjects.Clear();
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

                    String messageType = Director.Properties.Resources.ImportResult;
                    if (UServer == null)
                    {
                        MessageDialog.ShowMessage(messageType, Deserialization.errorMessage);
                        return;
                    }
                }
                catch
                {
                    MessageDialog.ShowError(Director.Properties.Resources.DialogInvalidFile);
                    return;
                }
                // Refresh tree
                RefreshServerTreeView();

                // Enable close sceanrio
                EnableServerButtons();
            }

        }

        /// <summary>
        /// Refresh server tree view.
        /// </summary>
        private void RefreshServerTreeView()
        {                
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
                s.ParentServer = UServer;

                foreach (Request r in s.Requests.OrderBy(n => n.Position))
                {
                    r.TreePosition = CreateTreeItem(s.TreePosition, r.Name, RequestImage, r);
                    r.ParentScenario = s;
                }
            }

            // Select Server and update view
            CurrentServer.SelectRow(tmp);
            UpdateControlView(null, null);
        }


        /// <summary>
        /// Export scenario.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveServer(object sender, EventArgs e)
        {
            ExportWindow ew = new ExportWindow(UServer);
            Content.Sensitive = false;
            ew.Closed += delegate {
                Content.Sensitive = true;
            };
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
        /// Run this scenario.
        /// </summary>
        private void RunScenario(object sender, EventArgs e)
        { 
            TreePosition tmp = CurrentServer.SelectedRow;

            if (tmp == null)
                return;

            var data = ServerStore.GetNavigatorAt(tmp).GetValue(ColumnType);

            if (data is Scenario)
            {
                if (WorkingThread != null)
                {
                    MessageDialog.ShowError(Director.Properties.Resources.CanNotStartScenario);
                    return;
                }

                Scenario s = (Scenario)data;

                WorkingThread = new Thread(ThreadWorker);
                WorkingThread.IsBackground = true;
                WorkingThread.Start(s);
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
