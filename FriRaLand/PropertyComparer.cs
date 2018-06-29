using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RakuLand {
    public sealed class PropertyComparer<T, U> : IComparer<T> {
        private PropertyDescriptor _property;

        private ListSortDirection _direction;

        private Comparer<U> _comparer;

        public PropertyComparer(PropertyDescriptor property, ListSortDirection direction) {
            this._property = property;
            this._direction = direction;
            this._comparer = Comparer<U>.Default;
        }

        public int Compare(T x, T y) {
            U u = (U)((object)this._property.GetValue(x));
            U u2 = (U)((object)this._property.GetValue(y));
            int result;
            if (this._direction == ListSortDirection.Ascending) {
                result = this._comparer.Compare(u, u2);
            } else {
                result = this._comparer.Compare(u2, u);
            }
            return result;
        }
    }
}
