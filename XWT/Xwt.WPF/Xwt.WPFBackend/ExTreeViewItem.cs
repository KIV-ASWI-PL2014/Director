//
// ExTreeViewItem.cs
//
// Author:
//       Eric Maupin <ermau@xamarin.com>
//       David Karlaš <david.karlas@gmail.com>
//
// Copyright (c) 2012 Xamarin, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Input;
using SWC = System.Windows.Controls;
using WKey = System.Windows.Input.Key;
using Xwt.Backends;

namespace Xwt.WPFBackend
{
	public class ExTreeViewItem
		: TreeViewItem
	{
		public ExTreeViewItem()
		{
			Loaded += OnLoaded;
		}

		public ExTreeViewItem (ExTreeView view)
			: this()
		{
			this.view = view;
		}

		protected override void OnExpanded (RoutedEventArgs e)
		{
			var node = (TreeStoreNode)DataContext;
			view.Backend.Context.InvokeUserCode (delegate {
				((ITreeViewEventSink)view.Backend.EventSink).OnRowExpanding (node);
			});

			base.OnExpanded (e);

			view.Backend.Context.InvokeUserCode (delegate {
				((ITreeViewEventSink)view.Backend.EventSink).OnRowExpanded (node);
			});
		}

		protected override void OnCollapsed(RoutedEventArgs e)
		{
			if (!IsExpanded)
				UnselectChildren((object o, ExTreeViewItem i) =>
				{
					return i != this;
				});
			base.OnCollapsed(e);
		}

		public int Level {
			get {
				if (this.level == -1) {
					ExTreeViewItem parent = ItemsControlFromItemContainer (this) as ExTreeViewItem;
					this.level = (parent != null) ? parent.Level + 1 : 0;
				}

				return this.level;
			}
		}

		public void SelectChildren (Func<object, ExTreeViewItem, bool> selector)
		{
			if (selector(this.DataContext, this))
				TreeView.SelectedItems.Add (DataContext);

			foreach (var item in Items) {
				var treeItem = (ExTreeViewItem)ItemContainerGenerator.ContainerFromItem(item);
				if (treeItem != null && selector (item, treeItem))
					treeItem.SelectChildren (selector);
			}
		}

		public void UnselectChildren (Func<object, ExTreeViewItem, bool> selector)
		{
			if (selector(this.DataContext, this))
				TreeView.SelectedItems.Remove(DataContext);

			foreach (var item in Items) {
				var treeItem = (ExTreeViewItem)ItemContainerGenerator.ContainerFromItem(item);
				if (treeItem != null && selector (item, treeItem))
					treeItem.UnselectChildren (selector);
			}
		}

		private int level = -1;
		private ExTreeView view;

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new ExTreeViewItem (this.view);
		}

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var p = e.GetPosition(view);
            var a = new MouseMovedEventArgs(e.Timestamp, p.X, p.Y);
            view.Backend.Context.InvokeUserCode(delegate
            {
                ((ITreeViewEventSink)view.Backend.EventSink).OnMouseMoved(a);
            });
        }

        class DragDropData
        {
            // Source
            public bool AutodetectDrag;
            public Rect DragRect = Rect.Empty;
            // Target
            public TransferDataType[] TargetTypes = new TransferDataType[0];
        }


        DragDropData dragDropInfo;

        DragDropData DragDropInfo
        {
            get
            {
                if (dragDropInfo == null)
                    dragDropInfo = new DragDropData();

                return dragDropInfo;
            }
        }

        protected override void OnDragOver(System.Windows.DragEventArgs e)
        {
            base.OnDragOver(e);

            Console.WriteLine("Drag OVER item");

            var pos = e.GetPosition(view).ToXwtPoint();
            var proposedAction = DetectDragAction(e.KeyStates);
            var store = new TransferDataStore();

            FillDataStore(store, e.Data, DragDropInfo.TargetTypes);

            var args = new DragOverEventArgs(pos, store, proposedAction);

            view.Backend.Context.InvokeUserCode(delegate
            {
                ((ITreeViewEventSink)view.Backend.EventSink).OnDragOver(args);
            });
        }

        static void FillDataStore(TransferDataStore store, IDataObject data, TransferDataType[] types)
        {
            foreach (var type in types)
            {
                string format = type.ToWpfDataFormat();
                if (!data.GetDataPresent(format))
                {
                    // This is a workaround to support type names which don't include the assembly name.
                    // It eases integration with Windows DND.
                    format = NormalizeTypeName(format);
                    if (!data.GetDataPresent(format))
                        continue;
                }

                var value = data.GetData(format);
                if (type == TransferDataType.Text)
                    store.AddText((string)value);
                else if (type == TransferDataType.Uri)
                {
                    var uris = ((string[])value).Select(f => new Uri(f)).ToArray();
                    store.AddUris(uris);
                }
                else if (value is byte[])
                    store.AddValue(type, (byte[])value);
                else
                    store.AddValue(type, value);
            }
        }

        static string NormalizeTypeName(string dataType)
        {
            // If the string is a fully qualified type name, strip the assembly name
            int i = dataType.IndexOf(',');
            if (i == -1)
                return dataType;
            string asmName = dataType.Substring(i + 1).Trim();
            try
            {
                new System.Reflection.AssemblyName(asmName);
            }
            catch
            {
                return dataType;
            }
            return dataType.Substring(0, i).Trim();
        }


        static DragDropAction DetectDragAction(DragDropKeyStates keys)
        {
            if ((keys & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey)
            {
                if ((keys & DragDropKeyStates.ShiftKey) == DragDropKeyStates.ShiftKey)
                    return DragDropAction.Link;
                else
                    return DragDropAction.Copy;
            }

            return DragDropAction.Move;
        }

		protected override void OnMouseLeftButtonDown (MouseButtonEventArgs e) {
			view.SelectItem(this);
			e.Handled = true;
			base.OnMouseLeftButtonDown(e);

            var args = ToXwtButtonArgs(e);

            view.Backend.Context.InvokeUserCode(delegate
            {
                ((ITreeViewEventSink)view.Backend.EventSink).OnButtonPressed(args);
            });
		}

        ButtonEventArgs ToXwtButtonArgs(MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(view);
            return new ButtonEventArgs()
            {
                X = pos.X,
                Y = pos.Y,
                MultiplePress = e.ClickCount,
                Button = e.ChangedButton.ToXwtButton()
            };
        }


		private ExTreeView TreeView
		{
			get
			{
				if (this.view == null)
					FindParent();

				return this.view;
			}
		}


		private void FindParent()
		{
			FrameworkElement e = this;
			while (this.view == null && e != null) {
				this.view = e.Parent as ExTreeView;
				e = (FrameworkElement)e.Parent;
			}
		}

		private void OnLoaded (object sender, RoutedEventArgs routedEventArgs)
		{
			ItemsControl parent = ItemsControlFromItemContainer (this);
			if (parent == null)
				return;

			int index = parent.Items.IndexOf (DataContext);
			if (index != parent.Items.Count - 1)
				return;

			foreach (var column in this.view.View.Columns) {
				if (Double.IsNaN (column.Width))
					column.Width = column.ActualWidth;

				column.Width = Double.NaN;
			}
		}

		protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
		{
			//We can't allow TreeViewItem(our base class) to get this message(OnKeyDown) because it will mess with our ExTreeView handling
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			BringIntoView();
			//We can't allow TreeViewItem(our base class) to get this message(OnGotFocus) because it will also select this item which we don't want
		}
	}
}
