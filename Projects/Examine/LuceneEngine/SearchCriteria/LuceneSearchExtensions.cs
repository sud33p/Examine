﻿using System;
using System.Security;
using Examine.SearchCriteria;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using System.Linq;

namespace Examine.LuceneEngine.SearchCriteria
{
    /// <summary>
    /// A set of helpers for working with Lucene.Net in Examine
    /// </summary>
    public static class LuceneSearchExtensions
    {
        /// <summary>
        /// Used to order results by the specified fields 
        /// </summary>
        /// <param name="qry"></param>
        /// <param name="fields">The fields to sort by and the type to sort them on</param>
        /// <returns></returns>
        public static IBooleanOperation OrderBy(this IQuery qry, params SortableField[] fields)
        {
            return qry.OrderBy(
                fields.Select(x => x.FieldName + "[Type=" + x.SortType.ToString().ToUpper() + "]").ToArray());
        }

        /// <summary>
        /// Used to order results by the specified fields 
        /// </summary>
        /// <param name="qry"></param>
        /// <param name="fields">The fields to sort by and the type to sort them on</param>
        /// <returns></returns>
        public static IBooleanOperation OrderByDescending(this IQuery qry, params SortableField[] fields)
        {
            return qry.OrderByDescending(
                fields.Select(x => x.FieldName + "[Type=" + x.SortType.ToString().ToUpper() + "]").ToArray());
        }

        /// <summary>
        /// Adds a single character wildcard to the string for Lucene wildcard matching
        /// </summary>
        /// <param name="s">The string to wildcard.</param>
        /// <returns>An IExamineValue for the required operation</returns>
        /// <exception cref="System.ArgumentException">Thrown when the string is null or empty</exception>
        public static IExamineValue SingleCharacterWildcard(this string s)
        {
            if (System.String.IsNullOrEmpty(s))
                throw new ArgumentException("Supplied string is null or empty.", "s");

            return new ExamineValue(Examineness.SimpleWildcard, s + "?");
        }

        /// <summary>
        /// Adds a multi-character wildcard to a string for Lucene wildcard matching
        /// </summary>
        /// <param name="s">The string to wildcard.</param>
        /// <returns>An IExamineValue for the required operation</returns>
        /// <exception cref="System.ArgumentException">Thrown when the string is null or empty</exception>
        public static IExamineValue MultipleCharacterWildcard(this string s)
        {
            if (String.IsNullOrEmpty(s))
                throw new ArgumentException("Supplied string is null or empty.", "s");
            return new ExamineValue(Examineness.ComplexWildcard, s + "*");
        }

        /// <summary>
        /// Configures the string for fuzzy matching in Lucene using the default fuzziness level
        /// </summary>
        /// <param name="s">The string to configure fuzzy matching on.</param>
        /// <returns>An IExamineValue for the required operation</returns>
        /// <exception cref="System.ArgumentException">Thrown when the string is null or empty</exception>
        public static IExamineValue Fuzzy(this string s)
        {
            return Fuzzy(s, 0.5f);
        }

        /// <summary>
        /// Configures the string for fuzzy matching in Lucene using the supplied fuzziness level
        /// </summary>
        /// <param name="s">The string to configure fuzzy matching on.</param>
        /// <param name="fuzzieness">The fuzzieness level.</param>
        /// <returns>
        /// An IExamineValue for the required operation
        /// </returns>
        /// <exception cref="System.ArgumentException">Thrown when the string is null or empty</exception>
        public static IExamineValue Fuzzy(this string s, float fuzzieness)
        {
            if (String.IsNullOrEmpty(s))
                throw new ArgumentException("Supplied string is null or empty.", "s");
            return new ExamineValue(Examineness.Fuzzy, s, fuzzieness);
        }

        /// <summary>
        /// Configures the string for boosting in Lucene
        /// </summary>
        /// <param name="s">The string to wildcard.</param>
        /// <param name="boost">The boost level.</param>
        /// <returns>
        /// An IExamineValue for the required operation
        /// </returns>
        /// <exception cref="System.ArgumentException">Thrown when the string is null or empty</exception>
        public static IExamineValue Boost(this string s, float boost)
        {
            if (String.IsNullOrEmpty(s))
                throw new ArgumentException("Supplied string is null or empty.", "s");
            return new ExamineValue(Examineness.Boosted, s, boost);
        }

        /// <summary>
        /// Configures the string for proximity matching
        /// </summary>
        /// <param name="s">The string to wildcard.</param>
        /// <param name="proximity">The proximity level.</param>
        /// <returns>
        /// An IExamineValue for the required operation
        /// </returns>
        /// <exception cref="System.ArgumentException">Thrown when the string is null or empty</exception>
        public static IExamineValue Proximity(this string s, int proximity)
        {
            if (String.IsNullOrEmpty(s))
                throw new ArgumentException("Supplied string is null or empty.", "s");
            return new ExamineValue(Examineness.Proximity, s, Convert.ToSingle(proximity));
        }

        /// <summary>
        /// Escapes the string within Lucene
        /// </summary>
        /// <param name="s">The string to wildcard.</param>
        /// <returns>An IExamineValue for the required operation</returns>
        /// <exception cref="System.ArgumentException">Thrown when the string is null or empty</exception>
        [SecuritySafeCritical]
        public static IExamineValue Escape(this string s)
        {
            if (String.IsNullOrEmpty(s))
                throw new ArgumentException("Supplied string is null or empty.", "s");
            return new ExamineValue(Examineness.Escaped, QueryParser.Escape(s));
        }

        ///// <summary>
        ///// Sets up an <see cref="IExamineValue"/> for an additional Examiness
        ///// </summary>
        ///// <param name="examineValue">The IExamineValue to continue working with.</param>
        ///// <param name="s">The string to postfix.</param>
        ///// <returns>Combined strings</returns>
        //public static string Then(this IExamineValue examineValue, string s)
        //{
        //    if (examineValue == null)
        //        throw new ArgumentNullException("examineValue", "examineValue is null.");
        //    if (String.IsNullOrEmpty(s))
        //        throw new ArgumentException("Supplied string is null or empty.", "s");
        //    return examineValue.Value + s;
        //}

        ///// <summary>
        ///// Sets up an <see cref="IExamineValue"/> for an additional Examiness
        ///// </summary>
        ///// <param name="examineValue">The IExamineValue to continue working with.</param>
        ///// <returns>Combined strings</returns>
        //public static string Then(this IExamineValue examineValue)
        //{
        //    return Then(examineValue, string.Empty);
        //}

        /// <summary>
        /// Converts an Examine boolean operation to a Lucene representation
        /// </summary>
        /// <param name="o">The operation.</param>
        /// <returns>The translated Boolean operation</returns>
        [SecuritySafeCritical]
        public static BooleanClause.Occur ToLuceneOccurance(this BooleanOperation o)
        {
            switch (o)
            {
                case BooleanOperation.And:
                    return BooleanClause.Occur.MUST;
                case BooleanOperation.Not:
                    return BooleanClause.Occur.MUST_NOT;
                case BooleanOperation.Or:
                default:
                    return BooleanClause.Occur.SHOULD;
            }
        }

        /// <summary>
        /// Converts a Lucene boolean occurrence to an Examine representation
        /// </summary>
        /// <param name="o">The occurrence to translate.</param>
        /// <returns>The translated boolean occurrence</returns>
        [SecuritySafeCritical]
        public static BooleanOperation ToBooleanOperation(this BooleanClause.Occur o)
        {
            if (o == BooleanClause.Occur.MUST)
            {
                return BooleanOperation.And;
            }
            else if (o == BooleanClause.Occur.MUST_NOT)
            {
                return BooleanOperation.Not;
            }
            else
            {
                return BooleanOperation.Or;
            }
        }
    }
}
