using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;

namespace Director.DataStructures.SupportStructures
{
    /// <summary>
    /// Email store extends IListDataSource for Views.
    /// </summary>
    class EmailStore : IListDataSource
    {
        public int RowCount
        {
            get { throw new NotImplementedException(); }
        }

        public object GetValue(int row, int column)
        {
            throw new NotImplementedException();
        }

        public void SetValue(int row, int column, object value)
        {
            throw new NotImplementedException();
        }

        public Type[] ColumnTypes
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<ListRowEventArgs> RowInserted;

        public event EventHandler<ListRowEventArgs> RowDeleted;

        public event EventHandler<ListRowEventArgs> RowChanged;

        public event EventHandler<ListRowOrderEventArgs> RowsReordered;
    }
}
