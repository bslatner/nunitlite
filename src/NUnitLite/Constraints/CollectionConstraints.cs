// *****************************************************
// Copyright 2007, Charlie Poole
//
// Licensed under the Open Software License version 3.0
// *****************************************************

using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{
    #region CollectionConstraint
    /// <summary>
    /// CollectionConstraint is the abstract base class for
    /// constraints that operate on collections.
    /// </summary>
    public abstract class CollectionConstraint : Constraint
    {
        public CollectionConstraint() { }

        public CollectionConstraint(object arg) : base(arg) { }

        /// <summary>
        /// Determines whether the specified enumerable is empty.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>
        /// 	<c>true</c> if the specified enumerable is empty; otherwise, <c>false</c>.
        /// </returns>
        protected static bool IsEmpty(IEnumerable enumerable)
        {
            ICollection collection = enumerable as ICollection;
            if (collection != null)
                return collection.Count == 0;
            else
                return !enumerable.GetEnumerator().MoveNext();
        }

        /// <summary>
        /// CollectionTally counts (tallies) the number of
        /// occurences of each object in one or more enuerations.
        /// </summary>
        protected internal class CollectionTally
        {
            // Internal hash used to count occurences
            private Hashtable hash = new Hashtable();

            // We use this for any null entries found, since
            // the key to a hash may not be null.
            static object NULL = new object();

            private int getTally(object obj)
            {
                if (obj == null) obj = NULL;
                object val = hash[obj];
                return val == null ? 0 : (int)val;
            }

            private void setTally(object obj, int tally)
            {
                if (obj == null) obj = NULL;
                hash[obj] = tally;
            }

            /// <summary>
            /// Construct a CollectionTally object from a collection
            /// </summary>
            /// <param name="c"></param>
            public CollectionTally(IEnumerable c)
            {
                foreach (object obj in c)
                    setTally(obj, getTally(obj) + 1);
            }

            /// <summary>
            /// Remove the counts for a collection from the tally,
            /// so long as their are sufficient items to remove.
            /// The tallies are not permitted to become negative.
            /// </summary>
            /// <param name="c">The collection to remove</param>
            /// <returns>True if there were enough items to remove, otherwise false</returns>
            public bool CanRemove(IEnumerable c)
            {
                foreach (object obj in c)
                {
                    int tally = getTally(obj);
                    if (tally > 0)
                        setTally(obj, tally - 1);
                    else
                        return false;
                }

                return true;
            }

            /// <summary>
            /// Test whether all the counts are equal to a given value
            /// </summary>
            /// <param name="count">The value to be looked for</param>
            /// <returns>True if all counts are equal to the value, otherwise false</returns>
            public bool AllCountsEqualTo(int count)
            {
                foreach (DictionaryEntry entry in hash)
                    if ((int)entry.Value != count)
                        return false;

                return true;
            }

            /// <summary>
            /// Get the count of the number of times an object is present in the tally
            /// </summary>
            public int this[object obj]
            {
                get { return getTally(obj); }
            }
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override bool Matches(object actual)
        {
            this.actual = actual;

            ICollection collection = actual as ICollection;
            if (collection == null)
                throw new ArgumentException("The actual value must be a collection", "actual");

            return doMatch(collection);
        }

        /// <summary>
        /// Protected method to be implemented by derived classes
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        protected abstract bool doMatch(ICollection collecton);
    }
    #endregion

    #region EmptyCollectionConstraint
    /// <summary>
    /// EmptyCollectionConstraint tests whether a colletion is empty. 
    /// </summary>
    public class EmptyCollectionConstraint : CollectionConstraint
    {
        /// <summary>
        /// Check that the collection is empty
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        protected override bool doMatch(ICollection collection)
        {
            return IsEmpty(collection);
        }

        /// <summary>
        /// Write the constraint description to a MessageWriter
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.Write("<empty>");
        }
    }
    #endregion

    #region UniqueItemsConstraint
    /// <summary>
    /// UniqueItemsConstraint tests whether all the items in a 
    /// collection are unique.
    /// </summary>
    public class UniqueItemsConstraint : CollectionConstraint
    {
        /// <summary>
        /// Apply the item constraint to each item in the collection,
        /// failing if any item fails.
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        protected override bool doMatch(ICollection actual)
        {
            return new CollectionTally(actual).AllCountsEqualTo(1);
        }

        /// <summary>
        /// Write a description of this constraint to a MessageWriter
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.Write("all items unique");
        }
    }
    #endregion

    #region CollectionContainsConstraint
    /// <summary>
    /// CollectionContainsConstraint is used to test whether a collection
    /// contains an expected object as a member.
    /// </summary>
    public class CollectionContainsConstraint : CollectionConstraint
    {
        private object expected;

        /// <summary>
        /// Construct a CollectionContainsConstraint
        /// </summary>
        /// <param name="expected"></param>
        public CollectionContainsConstraint(object expected)
            : base(expected)
        {
            this.expected = expected;
            this.DisplayName = "contains";
        }

        /// <summary>
        /// Test whether the expected item is contained in the collection
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        protected override bool doMatch(ICollection actual)
        {
            foreach (object obj in actual)
            {
#if NETCF_1_0
        		if ( expected == null && obj == null )
                    return true;
                if ( expected != null && expected.Equals( obj ) )
                    return true;
                    
#else
                if (Object.Equals(obj, expected))
                    return true;
#endif
            }

            return false;
        }

        /// <summary>
        /// Write a descripton of the constraint to a MessageWriter
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WritePredicate( "collection containing " );
            writer.WriteExpectedValue(expected);
        }
    }
    #endregion

    #region CollectionEquivalentConstraint
    /// <summary>
    /// CollectionEquivalentCOnstraint is used to determine whether two
    /// collections are equivalent.
    /// </summary>
    public class CollectionEquivalentConstraint : CollectionConstraint
    {
        private IEnumerable expected;

        /// <summary>
        /// Construct a CollectionEquivalentConstraint
        /// </summary>
        /// <param name="expected"></param>
        public CollectionEquivalentConstraint(IEnumerable expected) : base(expected)
        {
            this.expected = expected;
	    this.DisplayName = "equivalent";
        }

        /// <summary>
        /// Test whether two collections are equivalent
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        protected override bool doMatch(ICollection actual)
        {
            // This is just an optimization
            if (expected is ICollection)
                if (actual.Count != ((ICollection)expected).Count)
                    return false;

            CollectionTally tally = new CollectionTally(expected);
            return tally.CanRemove(actual) && tally.AllCountsEqualTo(0);
        }

        /// <summary>
        /// Write a description of this constraint to a MessageWriter
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WritePredicate("equivalent to");
            writer.WriteExpectedValue(expected);
        }
    }
    #endregion

    #region CollectionSubsetConstraint
    /// <summary>
    /// CollectionSubsetConstraint is used to determine whether
    /// one collection is a subset of another
    /// </summary>
    public class CollectionSubsetConstraint : CollectionConstraint
    {
        private IEnumerable expected;

        /// <summary>
        /// Construct a CollectionSubsetConstraint
        /// </summary>
        /// <param name="expected">The collection that the actual value is expected to be a subset of</param>
        public CollectionSubsetConstraint(IEnumerable expected) : base(expected)
        {
            this.expected = expected;
            this.DisplayName = "subsetof";
        }

        /// <summary>
        /// Test whether the actual collection is a subset of 
        /// the expected collection provided.
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        protected override bool doMatch(ICollection actual)
        {
            return new CollectionTally(expected).CanRemove(actual);
        }

        /// <summary>
        /// Write a description of this constraint to a MessageWriter
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WritePredicate( "subset of" );
            writer.WriteExpectedValue(expected);
        }
    }
    #endregion
}
