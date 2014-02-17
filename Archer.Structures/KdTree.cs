using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archer.Structures
{
    public class KdTree<TItem, TDimension> : ICollection<TItem>
        where TDimension : IComparable<TDimension>
    {
        #region internals
        private KdTreeNode<TItem> _root;
        public KdTreeNode<TItem> Root
        {
            get { return _root; }
        }
        
        public Func<TItem, IList<TDimension>> DimensionProvider 
        { 
            get; 
            set; 
        }
        #endregion

        #region properties
        private int _invalidCount = 0;
        private int _count = 0;
        public int Count
        {
            get { return _count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }
        #endregion

        #region constructors
        public KdTree() { }

        public KdTree(Func<TItem, IList<TDimension>> dimensionProvider)
        {
            DimensionProvider = dimensionProvider;
        }

        public KdTree(IEnumerable<TItem> collection, Func<TItem, IList<TDimension>> dimensionProvider)
            : this(dimensionProvider)
        {
            foreach (var item in collection)
            {
                this.Add(item);
            }
        }
        #endregion

        #region KdTree<TItem, TDimension>
        public KdTreeNode<TItem> Find(TItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            var node = _root;
            var dimensions = Dimensions(item);
            var depth = 0;

            IList<TDimension> nodeDimensions;
            int dimension;
            int comparison;

            while (node != null)
            {
                nodeDimensions = Dimensions(node.Item);
                if (dimensions.SequenceEqual(nodeDimensions))
                {
                    if (node.IsValid)
                        return node;
                    else return null;
                }

                dimension = depth % dimensions.Count;
                comparison = dimensions[dimension].CompareTo(nodeDimensions[dimension]);

                if (comparison <= 0)
                {
                    node = node.LeftChild;
                }
                else
                {
                    node = node.RightChild;
                }
                depth++;
            }
            return null;
        }

        public TItem NearestNeighbor(TItem item)
        {
            return NearestNeighbor(item, 1);
        }

        public TItem NearestNeighbor(TItem item, int neighbors)
        {
            throw new NotImplementedException();
        }

        public IList<TDimension> Dimensions(TItem item)
        {
            return DimensionProvider(item).ToList();
        }
        #endregion

        #region ICollection<T>
        public void Add(TItem item)
        {
            if (_root == null)
            {
                _root = new KdTreeNode<TItem>(item);
                _count++;
                return;
            }

            var node = _root;
            var dimensions = Dimensions(item);
            var depth = 0;

            IList<TDimension> nodeDimensions;
            int dimension;
            int comparison;

            while (node != null)
            {
                nodeDimensions = Dimensions(node.Item);
                if (dimensions.SequenceEqual(nodeDimensions))
                {
                    if (node.IsValid)
                        throw new ArgumentException("An item with this key already exists in tree.");
                    else
                    {
                        node.IsValid = true;
                        node.Item = item;
                        _count++;
                        return;
                    }
                }

                dimension = depth % dimensions.Count;
                comparison = dimensions[dimension].CompareTo(nodeDimensions[dimension]);

                if (comparison <= 0)
                {
                    if (node.LeftChild == null)
                    {
                        node.LeftChild = new KdTreeNode<TItem>(item);
                        _count++;
                        return;
                    }
                    else
                    {
                        node = node.LeftChild;
                        depth++;
                        continue;
                    }
                }
                else
                {
                    if (node.RightChild == null)
                    {
                        node.RightChild = new KdTreeNode<TItem>(item);
                        _count++;
                        return;
                    }
                    else
                    {
                        node = node.RightChild;
                        depth++;
                        continue;
                    }
                }
            }
            node = new KdTreeNode<TItem>(item);
            _count++;
        }

        public void Clear()
        {
            _root = null;
            _count = 0;
            _invalidCount = 0;
        }

        public bool Contains(TItem item)
        {
            return Find(item) != null;
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext() && arrayIndex < array.Length)
                array[arrayIndex++] = enumerator.Current;
        }

        public bool Remove(TItem item)
        {
            var nodeToRemove = Find(item);
            if (nodeToRemove == null)
                return false;

            nodeToRemove.IsValid = false;
            _invalidCount++;
            _count--;

            if (_invalidCount > 0 && _invalidCount / _count > 0.5)
            {
                var rng = new Random();
                var validItems = EnumerateInOrder(_root).ToList();
                _root = null;
                _count = 0;

                foreach (var validItem in validItems.OrderBy(validItem => rng.Next(validItems.Count)))
                    this.Add(validItem);

                _invalidCount = 0;
            }
            return true;
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            foreach (var item in EnumerateInOrder(_root))
                yield return item;
        }

        private IEnumerable<TItem> EnumerateInOrder(KdTreeNode<TItem> node)
        {
            if (node.LeftChild != null)
                foreach (var item in EnumerateInOrder(node.LeftChild))
                    yield return item;

            if (node.IsValid)
                yield return node.Item;

            if (node.RightChild != null)
                foreach (var item in EnumerateInOrder(node.RightChild))
                    yield return item;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        } 
        #endregion
    }
}
