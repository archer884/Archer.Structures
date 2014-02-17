using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archer.Structures
{
    public class KdTreeNode<T>
    {
        #region internals
        public T Item { get; internal set; }
        public bool IsValid { get; set; }
        public KdTreeNode<T> LeftChild { get; internal set; }
        public KdTreeNode<T> RightChild { get; internal set; } 
        #endregion

        #region constructors
        public KdTreeNode() { IsValid = true; }

        public KdTreeNode(T item)
            : this()
        {
            Item = item;
        }
        #endregion

        #region methods
        public override string ToString()
        {
            const string nullNodeString = "-";
            return string.Format("{0} -> {1} ; {2}", this.Item,
                this.LeftChild == null ? nullNodeString : this.LeftChild.Item.ToString(),
                this.RightChild == null ? nullNodeString : this.RightChild.Item.ToString());
        }
        #endregion
    }
}
