using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xwt;

namespace Director.Forms.Controls
{
    /// <summary>
    /// Text entry helper widget.
    /// </summary>
    internal class TextEntryHelper : Widget
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
            get { return helpStrings; }
            set
            {
                helpStrings = value;
                SetItems(helpStrings);
            }
        }

        /// <summary>
        /// Text entry instance.
        /// </summary>
        private TextEntry InnerTextEntry { get; set; }

        /// <summary>
        /// Scroll view.
        /// </summary>
        private ScrollView HelperScrollView { get; set; }

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
        public String Text
        {
            get { return InnerTextEntry.Text; }
            set { InnerTextEntry.Text = value; }
        }

        /// <summary>
        /// Changed event handler
        /// </summary>
        private EventHandler changed, got_focus;

        /// <summary>
        /// Changed event.
        /// </summary>
        public event EventHandler Changed
        {
            add { changed += value; }
            remove { changed -= value; }
        }

        /// <summary>
        /// Got item focus.
        /// </summary>
        public event EventHandler GotFocus
        {
            add { got_focus += value; }
            remove { got_focus -= value; }
        }

        /// <summary>
        /// Show and hide helper visibility.
        /// </summary>
        public Boolean HelperVisibility
        {
            get { return HelperListView.Visible; }
            set { HelperListView.Visible = value; }
        }

        /// <summary>
        /// Use tab indexes!
        /// </summary>
        public int TabIndex
        {
            set { InnerTextEntry.TabIndex = value; }
            get { return InnerTextEntry.TabIndex; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TextEntryHelper()
        {
            // Create text entry
            InnerTextEntry = new TextEntry();

            // Default bind
            InnerTextEntry.Changed += InnerTextEntry_Changed;

            // Self changed - show menu
            Changed += TextEntryHelper_Changed;

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
            HelperListView.Columns.Add("Name", Values);

            // Click selection
            HelperListView.ButtonReleased += HelperListView_ButtonPressed;

            // Scroll view to
            EntryHelper.PackStart(HelperListView, true, true);

            // Margins
            MarginTop = 4;
            MarginBottom = 5;

            // Set as content
            Content = EntryHelper;

            // Show tree items
            InnerTextEntry.GotFocus += GotFocusHandler;
        }

        /// <summary>
        /// Got focus handler!
        /// </summary>
        private void GotFocusHandler(object sender, EventArgs e)
        {
            HelperVisibility = true;
            got_focus(sender, e);
        }

        /// <summary>
        /// Click on specific item in list view.
        /// </summary>
        private void HelperListView_ButtonPressed(object sender, ButtonEventArgs e)
        {
            if (e.Button == PointerButton.Left)
            {
                InnerTextEntry.Text = ActiveList[HelperListView.SelectedRow];
                changed(sender, e);
            }
        }

        /// <summary>
        /// Key events!
        /// </summary>
        private void InnerTextEntry_KeyPressed(object sender, KeyEventArgs e)
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
                    if (HelperListView.SelectedRow >= 0)
                    {
                        InnerTextEntry.Text = ActiveList[HelperListView.SelectedRow];
                        changed(sender, e);
                    }
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
                HelperListView.SelectRow(row);

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
                HelperListView.SelectRow(row);

                if (HelperListView.VerticalScrollControl.Value + HelperListView.VerticalScrollControl.StepIncrement <
                    HelperListView.VerticalScrollControl.UpperValue)
                    HelperListView.VerticalScrollControl.Value += HelperListView.VerticalScrollControl.StepIncrement;
            }
        }


        /// <summary>
        /// Self changed display list.
        /// </summary>
        private void TextEntryHelper_Changed(object sender, EventArgs e)
        {
            if (HelpStrings != null && InnerTextEntry != null)
            {
                List<String> searchedItems =
                    HelpStrings.FindAll(
                        n => n.IndexOf(InnerTextEntry.Text, StringComparison.InvariantCultureIgnoreCase) >= 0);
                SetItems(searchedItems);
            }
        }

        /// <summary>
        /// Set items to data store.
        /// </summary>
        private void SetItems(List<String> items)
        {
            ActiveList = new Dictionary<int, String>();
            HelperListView.UnselectAll();
            HelperStore.Clear();
            foreach (var s in items)
            {
                int row = HelperStore.AddRow();
                HelperStore.SetValue(row, Values, s);
                ActiveList.Add(row, s);
            }


            if (items.Count > 0)
                HelperListView.SelectRow(0);
        }

        /// <summary>
        /// Trigger changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InnerTextEntry_Changed(object sender, EventArgs e)
        {
            changed(sender, e);
        }
    }
}