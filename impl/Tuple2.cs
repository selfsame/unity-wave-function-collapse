// ----------------------------------------------------------------------------
// Tuple structs for use in .NET Not-Quite-3.5 (e.g. Unity3D).
//
// Used Chapter 3 in http://functional-programming.net/ as a starting point.
//
// Note: .NET 4.0 Tuples are immutable classes so they're *slightly* different.
// ----------------------------------------------------------------------------

using System;

namespace Eppy
{
    /// <summary>
    /// Represents a functional tuple that can be used to store
    /// two values of different types inside one object.
    /// </summary>
    /// <typeparam name="T1">The type of the first element</typeparam>
    /// <typeparam name="T2">The type of the second element</typeparam>
    public sealed class Tuple<T1, T2>
    {
        private readonly T1 item1;
        private readonly T2 item2;

        /// <summary>
        /// Retyurns the first element of the tuple
        /// </summary>
        public T1 Item1
        {
            get { return item1; }
        }

        /// <summary>
        /// Returns the second element of the tuple
        /// </summary>
        public T2 Item2
        {
            get { return item2; }
        }

        /// <summary>
        /// Create a new tuple value
        /// </summary>
        /// <param name="item1">First element of the tuple</param>
        /// <param name="second">Second element of the tuple</param>
        public Tuple(T1 item1, T2 item2)
        {
            this.item1 = item1;
            this.item2 = item2;
        }

        public override string ToString()
        {
            return string.Format("Tuple({0}, {1})", Item1, Item2);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + (item1 == null ? 0 : item1.GetHashCode());
            hash = hash * 23 + (item2 == null ? 0 : item2.GetHashCode());
            return hash;
        }

        public override bool Equals(object o)
        {
            if (!(o is Tuple<T1, T2>)) {
                return false;
            }

            var other = (Tuple<T1, T2>) o;

            return this == other;
        }

        public bool Equals(Tuple<T1, T2> other)
        {
            return this == other;
        }

        public static bool operator==(Tuple<T1, T2> a, Tuple<T1, T2> b)
        {
            if (object.ReferenceEquals(a, null)) {
                return object.ReferenceEquals(b, null);
            }
            if (a.item1 == null && b.item1 != null) return false;
            if (a.item2 == null && b.item2 != null) return false;
            return
                a.item1.Equals(b.item1) &&
                a.item2.Equals(b.item2);
        }

        public static bool operator!=(Tuple<T1, T2> a, Tuple<T1, T2> b)
        {
            return !(a == b);
        }

        public void Unpack(Action<T1, T2> unpackerDelegate)
        {
            unpackerDelegate(Item1, Item2);
        }
    }
}