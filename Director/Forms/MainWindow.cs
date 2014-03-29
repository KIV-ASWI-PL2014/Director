using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;

namespace Director.Forms
{
    class MainWindow : Window
    {

        public MainWindow()
        {
            _initializeComponents();
        }

        private void _initializeComponents() {
            /// Title and Initial size
            Title = "API Director";
            Width = 500;
            Height = 400;

            // Center screen
            InitialLocation = WindowLocation.CenterScreen;

            // Initialize menu
            _initializeMenu();
        }

        private void _initializeMenu() 
        {
            Menu menu = new Menu();

            var file = new MenuItem("_File");
            file.SubMenu = new Menu();
            file.SubMenu.Items.Add(new MenuItem("_Open"));
            file.SubMenu.Items.Add(new MenuItem("_New"));
            MenuItem mi = new MenuItem("_Close");
            mi.Clicked += delegate
            {
                Application.Exit();
            };
            file.SubMenu.Items.Add(mi);
            menu.Items.Add(file);

            var edit = new MenuItem("_Edit");
            edit.SubMenu = new Menu();
            edit.SubMenu.Items.Add(new MenuItem("_Copy"));
            edit.SubMenu.Items.Add(new MenuItem("Cu_t"));
            edit.SubMenu.Items.Add(new MenuItem("_Paste"));
            menu.Items.Add(edit);

            MainMenu = menu;
        }
    }
}
