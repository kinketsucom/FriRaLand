using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RakuLand {
    public class SortableBindingList<T> : BindingList<T> {
        private PropertyDescriptor _sortProp = null;

        private ListSortDirection _sortDir = ListSortDirection.Ascending;

        private bool _isSorted = false;

        protected override bool SupportsSortingCore {
            get {
                return true;
            }
        }

        protected override bool IsSortedCore {
            get {
                return this._isSorted;
            }
        }

        protected override PropertyDescriptor SortPropertyCore {
            get {
                return this._sortProp;
            }
        }

        protected override ListSortDirection SortDirectionCore {
            get {
                return this._sortDir;
            }
        }

        public SortableBindingList() {
        }

        public SortableBindingList(IList<T> list)
            : base(list) {
        }


        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction) {
            List<T> list = base.Items as List<T>;
            if (list != null) {
                list.Sort(PropertyComparerFactory.Factory<T>(property, direction));
                this._isSorted = true;
                this._sortProp = property;
                this._sortDir = direction;
                this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }

        protected override void RemoveSortCore() {
        }
    }
}
