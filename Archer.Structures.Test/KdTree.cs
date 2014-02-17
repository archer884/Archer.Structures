using Archer.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Archer.Structures.Test
{
    public class KdTreeTests
    {
        #region support
        public class Point
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        public class PointComparer : IEqualityComparer<Point>
        {
            public bool Equals(Point x, Point y)
            {
                return x.X == y.X && x.Y == y.Y;
            }

            public int GetHashCode(Point obj)
            {
                throw new NotImplementedException();
            }
        }

        public static KdTree<Point, int> _staticTestTree;
        public static KdTree<Point, int> StaticTestTree
        {
            get { return _staticTestTree ?? (_staticTestTree = GetTestTree()); }
        }

        public static KdTree<Point, int> GetTestTree(int size = 100)
        {
            var rng = new Random(1);
            var tree = new KdTree<Point, int>(point => new List<int>() { point.X, point.Y });
            for (int i = 0; i < size; i++)
            {
                tree.Add(new Point()
                {
                    X = rng.Next(100),
                    Y = i,
                });
            }
            return tree;
        }
        #endregion

        #region ICollection<T> tests
        [Fact]
        public void Add_and_Contains()
        {
            var point = new Point() { X = 101, Y = 101 };
            var missingPoint = new Point() { X = 102, Y = 102 };
            var tree = (KdTree<Point, int>)null;

            Assert.DoesNotThrow(() => 
            {
                tree = GetTestTree();
                tree.Add(point);
            });

            Assert.NotNull(tree.Root);
            Assert.True(tree.Contains(point));
            Assert.False(tree.Contains(missingPoint));
            Assert.Throws<ArgumentException>(() => tree.Add(point));
        }

        [Fact]
        public void Clear()
        {
            var tree = GetTestTree();

            tree.Clear();
            Assert.Null(tree.Root);
            Assert.Equal(0, tree.Count);
        }

        [Fact]
        public void CopyTo()
        {
            var pointComparer = new PointComparer();

            var tree = StaticTestTree;
            Point[] array = new Point[100];

            tree.CopyTo(array, 0);
            Assert.True(tree.SequenceEqual(array, pointComparer));

            array = new Point[90];
            tree.CopyTo(array, 0);
            Assert.Equal(10, tree.Except(array).Count());

            array = new Point[100];
            tree.CopyTo(array, 10);
            Assert.Equal(10, tree.Except(array).Count());
        }

        [Fact]
        public void Count()
        {
            var tree = GetTestTree();
            Assert.Equal(100, tree.Count);

            tree.Add(new Point() { X = 101, Y = 101 });
            Assert.Equal(101, tree.Count);

            tree.Remove(tree.Root.Item);
            Assert.Equal(100, tree.Count);
        }

        [Fact]
        public void IsReadOnly()
        {
            Assert.False(StaticTestTree.IsReadOnly);
        }

        [Fact]
        public void Remove()
        {
            var tree = GetTestTree();
            var point = new Point() { X = 101, Y = 101 };
            var rootNode = new Point() { X = 24, Y = 0 };

            tree.Add(point);
            Assert.True(tree.Contains(point));

            tree.Remove(point);
            Assert.False(tree.Contains(point));

            Assert.True(tree.Contains(rootNode));
        }
        #endregion

        #region IEnumerable<T> tests
        // do later
        #endregion

        #region KdTree custom method tests
        // do later
        #endregion
    }
}
