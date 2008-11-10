// *****************************************************
// Copyright 2007, Charlie Poole
//
// Licensed under the Open Software License version 3.0
// *****************************************************

using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// BinaryConstraint is the abstract base of all constraints
    /// that combine two other constraints in some fashion.
    /// </summary>
    public abstract class BinaryConstraint : Constraint
    {
        /// <summary>
        /// The first constraint being combined
        /// </summary>
        protected Constraint left;
        /// <summary>
        /// The second constraint being combined
        /// </summary>
        protected Constraint right;

        /// <summary>
        /// Construct a BinaryConstraint from two other constraints
        /// </summary>
        /// <param name="left">The first constraint</param>
        /// <param name="right">The second constraint</param>
        public BinaryConstraint(Constraint left, Constraint right) : base(left, right)
        {
            this.left = left;
            this.right = right;
        }
    }

    /// <summary>
    /// AndConstraint succeeds only if both members succeed.
    /// </summary>
    public class AndConstraint : BinaryConstraint
    {
        /// <summary>
        /// Create an AndConstraint from two other constraints
        /// </summary>
        /// <param name="left">The first constraint</param>
        /// <param name="right">The second constraint</param>
        public AndConstraint(Constraint left, Constraint right) : base(left, right) { }

        /// <summary>
        /// Apply both member constraints to an actual value, succeeding 
        /// succeeding only if both of them succeed.
        /// </summary>
        /// <param name="actual">The actual value</param>
        /// <returns>True if the constraints both succeeded</returns>
        public override bool Matches(object actual)
        {
            this.actual = actual;
            return left.Matches(actual) && right.Matches(actual);
        }

        /// <summary>
        /// Write a description for this contraint to a MessageWriter
        /// </summary>
        /// <param name="writer">The MessageWriter to receive the description</param>
        public override void WriteDescriptionTo(MessageWriter writer)
        {
            left.WriteDescriptionTo(writer);
            writer.WriteConnector("and");
            right.WriteDescriptionTo(writer);
        }
    }

    /// <summary>
    /// OrConstraint succeeds if either member succeeds
    /// </summary>
    public class OrConstraint : BinaryConstraint
    {
        /// <summary>
        /// Create an OrConstraint from two other constraints
        /// </summary>
        /// <param name="left">The first constraint</param>
        /// <param name="right">The second constraint</param>
        public OrConstraint(Constraint left, Constraint right) : base(left, right) { }

        /// <summary>
        /// Apply the member constraints to an actual value, succeeding 
        /// succeeding as soon as one of them succeeds.
        /// </summary>
        /// <param name="actual">The actual value</param>
        /// <returns>True if either constraint succeeded</returns>
        public override bool Matches(object actual)
        {
            this.actual = actual;
            return left.Matches(actual) || right.Matches(actual);
        }

        /// <summary>
        /// Write a description for this contraint to a MessageWriter
        /// </summary>
        /// <param name="writer">The MessageWriter to receive the description</param>
        public override void WriteDescriptionTo(MessageWriter writer)
        {
            left.WriteDescriptionTo(writer);
            writer.WriteConnector("or");
            right.WriteDescriptionTo(writer);
        }
    }
}
