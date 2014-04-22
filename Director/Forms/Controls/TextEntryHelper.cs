using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xwt;

namespace Director.Forms.Controls
{
    /// <summary>
    /// Text entry helper.
    /// </summary>
    class TextEntryHelper : Widget
    {
        /// <summary>
        /// Internal private field.
        /// </summary>
        private List<String> helpStrings;

        /// <summary>
        /// Help strings.
        /// </summary>
        public List<String> HelpStrings
        {
            get
            {
                return helpStrings;
            }
            set
            {
                helpStrings = value;
                SetItems(helpStrings);
            }
        }

        /// <summary>
        /// Text entry instance.
        /// </summary>
        private TextEntry InnerTextEntry = new TextEntry();

        /// <summary>
        /// Scroll view.
        /// </summary>
        private ScrollView HelperScrollView  { get; set; }

        /// <summary>
        /// Helper list view.
        /// </summary>
        private ListView HelperListView { get; set; }

        /// <summary>
        /// Data fields.
        /// </summary>
        private DataField<String> Values { get; set; }

        /// <summary>
        /// Helper stores.
        /// </summary>
        private ListStore HelperStore { get; set; }

        /// <summary>
        /// Active list help strings.
        /// </summary>
        private Dictionary<int, String> ActiveList { get; set; }

        /// <summary>
        /// Return actual text.
        /// </summary>
        public String Text { 
            get { 
                return InnerTextEntry.Text; 
            }
            set
            {
                InnerTextEntry.Text = value;
            }
        }

        /// <summary>
        /// Changed event handler
        /// </summary>
        EventHandler changed, show_helper, hide_helper;

        /// <summary>
        /// Changed event.
        /// </summary>
        public event EventHandler Changed
        {
            add
            {
                changed += value;
            }
            remove
            {
                changed -= value;
            }
        }


        /// <summary>
        /// Show helper event.
        /// </summary>
        public event EventHandler ShowHelper
        {
            add
            {
                show_helper += value;
            }
            remove
            {
                show_helper -= value;
            }
        }


        /// <summary>
        /// Changed event.
        /// </summary>
        public event EventHandler HideHelper
        {
            add
            {
                hide_helper += value;
            }
            remove
            {
                hide_helper -= value;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TextEntryHelper()
        {
            // Self changed - show menu
            Changed += TextEntryHelper_Changed;

            // Default bind
            InnerTextEntry.Changed += InnerTextEntry_Changed;

            // Hide tree items
            LostFocus += InnerTextEntry_LostFocus;

            // Show tree items
            InnerTextEntry.GotFocus += InnerTextEntry_GotFocus;

            // Key binding events
            InnerTextEntry.KeyReleased += InnerTextEntry_KeyPressed;

            // Helper
            VBox EntryHelper = new VBox();
            EntryHelper.PackStart(InnerTextEntry, true, true);

            // Create list store
            Values = new DataField<string>();
            HelperStore = new ListStore(Values);

            // Create helper list view
            HelperListView = new ListView()
            {
                MinHeight = 75, 
                HeightRequest = 75,
                Visible = false,
                HeadersVisible = false,
                DataSource = HelperStore,
                SelectionMode = SelectionMode.Single
            };
            HelperListView.Columns.Add("Name", Values, Values);

            // Helper list view clicked
            AddHelperListHandler();
            
            // Scroll view to
            EntryHelper.PackStart(HelperListView, true, true);

            // Margins
            MarginTop = 4;
            MarginBottom = 5;

            // Set as content
            Content = EntryHelper;
        }

        /// <summary>
        /// Click event!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void HelperListView_SelectionChanged(object sender, EventArgs e)
        {
            InnerTextEntry.Text = ActiveList[HelperListView.SelectedRow];
            changed(sender, e);
            ShowListHelper(sender, e);
            show_helper(sender, e);
        }

        /// <summary>
        /// Add handler.
        /// </summary>
        void AddHelperListHandler()
        {
            HelperListView.SelectionChanged += HelperListView_SelectionChanged;
        }

        /// <summary>
        /// Remove handler.
        /// </summary>
        void RemoveHelperListHandler()
        {
            HelperListView.SelectionChanged -= HelperListView_SelectionChanged;
        }

        /// <summary>
        /// Key events!
        /// </summary>
        void InnerTextEntry_KeyPressed(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                    SelectionDown();
                    break;

                case Key.Up:
                    SelectionUp();
                    break;

                case Key.Return:
                    InnerTextEntry.Text = ActiveList[HelperListView.SelectedRow];
                    changed(sender, e);
                    break;
            }
        }

        /// <summary>
        /// Selection up.
        /// </summary>
        private void SelectionUp()
        {
            int row = HelperListView.SelectedRow - 1;
            if (ActiveList.ContainsKey(row))
            {
                RemoveHelperListHandler();
                HelperListView.SelectRow(row);
                AddHelperListHandler();

                if (HelperListView.VerticalScrollControl.Value - HelperListView.VerticalScrollControl.StepIncrement >= 0)
                    HelperListView.VerticalScrollControl.Value -= HelperListView.VerticalScrollControl.StepIncrement;
            }
        }

        /// <summary>
        /// Selection down.
        /// </summary>
        private void SelectionDown()
        {
            int row = HelperListView.SelectedRow + 1;
            if (ActiveList.ContainsKey(row))
            {
                RemoveHelperListHandler();
                HelperListView.SelectRow(row);
                AddHelperListHandler();

                if (HelperListView.VerticalScrollControl.Value + HelperListView.VerticalScrollControl.StepIncrement < HelperListView.VerticalScrollControl.UpperValue)
                    HelperListView.VerticalScrollControl.Value += HelperListView.VerticalScrollControl.StepIncrement;
            }
        }

        /// <summary>
        /// Draw list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void InnerTextEntry_GotFocus(object sender, EventArgs e)
        {
            ShowListHelper(sender, e);
            show_helper(sender, e);
        }

        /// <summary>
        /// Hide text entry.
        /// </summary>
        void InnerTextEntry_LostFocus(object sender, EventArgs e)
        {
            HideListHelper(sender, e);
            hide_helper(sender, e);
        }

        /// <summary>
        /// Self changed display list.
        /// </summary>
        void TextEntryHelper_Changed(object sender, EventArgs e)
        {
            List<String> searchedItems = HelpStrings.FindAll(n => n.IndexOf(InnerTextEntry.Text, StringComparison.InvariantCultureIgnoreCase) >= 0);
            SetItems(searchedItems);
        }

        /// <summary>
        /// Show helper.
        /// </summary>
        public void ShowListHelper(object sender, EventArgs e)
        {
            HelperListView.Visible = true;
        }

        /// <summary>
        /// Hide helpers.
        /// </summary>
        public void HideListHelper(object sender, EventArgs e)
        {
            HelperListView.Visible = false; 
        }

        /// <summary>
        /// Set items to data store.
        /// </summary>
        private void SetItems(List<String> items)
        {
            ActiveList = new Dictionary<int, String>();
            HelperStore.Clear();
            foreach (var s in items)
            {
                int row = HelperStore.AddRow();
                HelperStore.SetValue(row, Values, s);
                ActiveList.Add(row, s);
            }
            RemoveHelperListHandler();
            if (items.Count > 0)
                HelperListView.SelectRow(1);
            AddHelperListHandler();
        }

        /// <summary>
        /// Trigger changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void InnerTextEntry_Changed(object sender, EventArgs e)
        {
            changed(sender, e);
        }
    }
}
