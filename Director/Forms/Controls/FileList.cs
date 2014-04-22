using Director.DataStructures;
using Director.DataStructures.SupportStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Xwt.Drawing;

namespace Director.Forms.Controls
{
    class FileList : VBox
    {
        /// <summary>
        /// Active file request.
        /// </summary>
        public List<FileItem> ActiveFiles { get; set; }

        /// <summary>
        /// File list content.
        /// </summary>
        public VBox FileListContent { get; set; }

        public FileList(List<FileItem> items)
        {
            // Set active files
            ActiveFiles = items;

            // Expand?
            ExpandVertical = true;
            ExpandVertical = true;

            // Margins
            Margin = 10;

            // Init components
            _initializeComponents();

            // Refresh file list
            RefreshFileList();
        }

        /// <summary>
        /// Refresh file list.
        /// </summary>
        private void RefreshFileList()
        {
            FileListContent.Clear();
            int x = 0;
            foreach (var h in ActiveFiles)
            {
                FileListItem tmp = new FileListItem(this, h, (x % 2 == 0) ? Colors.White : Colors.LightGray);
                FileListContent.PackStart(tmp);
                x++;
            }
        }

        /// <summary>
        /// Init object components.
        /// </summary>
        private void _initializeComponents()
        {
            // Create first line with informations
            HBox FirstLine = new HBox();
            Label FileLabelType = new Label(Director.Properties.Resources.FileName)
            {
                HorizontalPlacement = WidgetPlacement.Center,
                ExpandHorizontal = true,
                ExpandVertical = false,
                MarginLeft = 10
            };
            Label FileLabelValue = new Label(Director.Properties.Resources.FilePath)
            {
                ExpandHorizontal = true,
                ExpandVertical = false,
                HorizontalPlacement = WidgetPlacement.Center
            };
            Label FileEmpty = new Label("")
            {
                MinWidth = 30,
                WidthRequest = 30
            };
            Button NewFile = new Button(Image.FromResource(DirectorImages.ADD_ICON))
            {
                MinWidth = 30,
                WidthRequest = 30,
                MarginRight = 30
            };
            NewFile.Clicked += NewFile_Clicked;
            FirstLine.PackStart(FileLabelType, true, true);
            FirstLine.PackStart(FileLabelValue, true, true);
            FirstLine.PackStart(FileEmpty, false, false);
            FirstLine.PackStart(NewFile, false, false);
            PackStart(FirstLine);

            // Create content
            FileListContent = new VBox()
            {
                BackgroundColor = Colors.White,
                ExpandVertical = true,
                ExpandHorizontal = false
            };

            ScrollView FileListScroll = new ScrollView()
            {
                HorizontalScrollPolicy = ScrollPolicy.Never,
                VerticalScrollPolicy = ScrollPolicy.Always,
                Content = FileListContent,
                BackgroundColor = Colors.LightGray
            };

            // Add item list
            PackStart(FileListScroll, true, true);
        }

        /// <summary>
        /// Add new file.
        /// </summary>
        void NewFile_Clicked(object sender, EventArgs e)
        {
            FileItem NewFileItem = new FileItem();
            ActiveFiles.Add(NewFileItem);
            var tmp = new FileListItem(this, NewFileItem, ((ActiveFiles.Count - 1) % 2 == 0) ? Colors.White : Colors.LightGray);
            FileListContent.PackStart(tmp);
        }

        /// <summary>
        /// Remove file item.
        /// </summary>
        public void RemoveFileItem(FileItem _item)
        {
            ActiveFiles.Remove(_item);
            RefreshFileList();
        }
    }

    class FileListItem : HBox
    {
        /// <summary>
        /// Parent list for destroying.
        /// </summary>
        private FileList ParentList { get; set; }

		/// <summary>
		/// Default background color.
		/// </summary>
		/// <value>The default background.</value>
		private Color DefaultColor { get; set; }

        /// <summary>
        /// File item.
        /// </summary>
        private FileItem ActiveFileItem { get; set; }

        /// <summary>
        /// File name.
        /// </summary>
        private TextEntry FileName { get; set; }

        /// <summary>
        /// File path.
        /// </summary>
        private TextEntry FilePath { get; set; }

        /// <summary>
        /// Select file path.
        /// </summary>
        private Button SelectFile { get; set; }

        /// <summary>
        /// Remove file.
        /// </summary>
        private Button RemoveFile { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="_item"></param>
        /// <param name="_color"></param>
        public FileListItem(FileList _parent, FileItem _item, Color _color)
        {
            // Set attributes and colors
            ParentList = _parent;
            ActiveFileItem = _item;
            BackgroundColor = DefaultColor = _color;

            // Expanding height
            MinHeight = 30;
            ExpandHorizontal = true;
            Margin = 0;

            // Init components
            _initalizeComponents();
        }

        /// <summary>
        /// Initialize components.
        /// </summary>
        private void _initalizeComponents()
        {
            // Name
            FileName = new TextEntry()
            {
                Text = ActiveFileItem.FileName,
                MarginLeft = 5,
                VerticalPlacement = WidgetPlacement.Center,
                HorizontalPlacement = WidgetPlacement.Fill
            };
            FileName.Changed += delegate
            {
                ActiveFileItem.FileName = FileName.Text;
            };
            FileName.LostFocus += FileName_LostFocus;
            PackStart(FileName, true, true);

            // Path
            FilePath = new TextEntry()
            {
                Text = ActiveFileItem.FilePath,
                MarginLeft = 5,
                VerticalPlacement = WidgetPlacement.Center,
                HorizontalPlacement = WidgetPlacement.Fill
            };
            FilePath.Changed += delegate
            {
                ActiveFileItem.FilePath = FilePath.Text;
            };
            PackStart(FilePath, true, true);

            // Select file BTN
            SelectFile = new Button("...")
            {
                HorizontalPlacement = WidgetPlacement.Center,
                VerticalPlacement = WidgetPlacement.Center,
                ExpandHorizontal = false,
                ExpandVertical = false,
                WidthRequest = 25
            };
            SelectFile.Clicked += SelectFile_Clicked;
            PackStart(SelectFile, expand: false, fill: false);

            // Remove File
            RemoveFile = new Button(Image.FromResource(DirectorImages.CROSS_ICON))
            {
                MarginRight = 20,
                HorizontalPlacement = WidgetPlacement.Center,
                VerticalPlacement = WidgetPlacement.Center,
                ExpandVertical = false,
                ExpandHorizontal = false
            };
            RemoveFile.Clicked += RemoveFile_Clicked;
            PackStart(RemoveFile, expand: false, fill: false);
        }

        /// <summary>
        /// Validate if this content already exists.
        /// </summary>
        void FileName_LostFocus(object sender, EventArgs e)
        {
            int count = ParentList.ActiveFiles.Count(n => (n.FileName == FileName.Text && n != ActiveFileItem));
            if (count > 0)
            {
                MessageDialog.ShowError(Director.Properties.Resources.FileNameAlreadyExists);
            }
        }

        /// <summary>
        /// Remove file.
        /// </summary>
        void RemoveFile_Clicked(object sender, EventArgs e)
        {
            ParentList.RemoveFileItem(ActiveFileItem);
        }

        /// <summary>
        /// Select file.
        /// </summary>
        void SelectFile_Clicked(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog(Director.Properties.Resources.DialogOpenScenario);
            dlg.Multiselect = false;
            dlg.Filters.Add(new FileDialogFilter("All files", "*.*"));
            if (dlg.Run())
                FilePath.Text = dlg.FileName;
        }

    }
}
