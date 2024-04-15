//------------------------------------------------------------------------------
// <copyright file="GenericEnumRowCollection.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">Microsoft</owner>
// <owner current="true" primary="false">Microsoft</owner>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using System.Diagnostics;

namespace Stimulsoft.System.Data
{
    /// <summary>
    /// Provides an entry point so that Cast operator call can be intercepted within an extension method.
    /// </summary>
    public abstract class EnumerableRowCollection2 : IEnumerable
    {
        internal abstract Type ElementType { get; }
        internal abstract DataTable Table { get; }

        internal EnumerableRowCollection2()
        {
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return null;
        }
    }

    /// <summary>
    /// This class provides a wrapper for DataTables to allow for querying via LINQ.
    /// </summary>
    public class EnumerableRowCollection2<TRow> : IEnumerable<TRow>
    {
        private readonly DataTable _table;
        private readonly IEnumerable<TRow> _enumerableRows;
        private readonly List<Func<TRow, bool>> _listOfPredicates;

        // Stores list of sort expression in the order provided by user. E.g. order by, thenby, thenby descending..
        private readonly SortExpressionBuilder<TRow> _sortExpression;

        private readonly Func<TRow, TRow> _selector;

        #region Properties

        internal Type ElementType
        {
            get
            {
                return typeof(TRow);
            }

        }

        internal IEnumerable<TRow> EnumerableRows
        {
            get
            {
                return _enumerableRows;
            }
        }

        internal DataTable Table
        {
            get
            {
                return _table;
            }
        }


        #endregion Properties

        #region Constructors

        /// <summary>
        /// This constructor is used when Select operator is called with output Type other than input row Type.
        /// Basically fail on GetLDV(), but other LINQ operators must work.
        /// </summary>
        internal EnumerableRowCollection2(IEnumerable<TRow> enumerableRows, bool isDataViewable, DataTable table)
        {
            Debug.Assert(!isDataViewable || table != null, "isDataViewable bug table is null");

            _enumerableRows = enumerableRows;
            if (isDataViewable)
            {
                _table = table;
            }
            _listOfPredicates = new List<Func<TRow, bool>>();
            _sortExpression = new SortExpressionBuilder<TRow>();
        }

        /// <summary>
        /// Basic Constructor
        /// </summary>
        internal EnumerableRowCollection2(DataTable table)
        {
            _table = table;
            _enumerableRows = table.Rows.Cast<TRow>();
            _listOfPredicates = new List<Func<TRow, bool>>();
            _sortExpression = new SortExpressionBuilder<TRow>();
        }

        /// <summary>
        /// Copy Constructor that sets the input IEnumerable as enumerableRows
        /// Used to maintain IEnumerable that has linq operators executed in the same order as the user
        /// </summary>
        internal EnumerableRowCollection2(EnumerableRowCollection2<TRow> source, IEnumerable<TRow> enumerableRows, Func<TRow, TRow> selector)
        {
            Debug.Assert(null != enumerableRows, "null enumerableRows");

            _enumerableRows = enumerableRows;
            _selector = selector;
            if (null != source)
            {
                if (null == source._selector)
                {
                    _table = source._table;
                }
                _listOfPredicates = new List<Func<TRow, bool>>(source._listOfPredicates);
                _sortExpression = source._sortExpression.Clone(); //deep copy the List
            }
            else
            {
                _listOfPredicates = new List<Func<TRow, bool>>();
                _sortExpression = new SortExpressionBuilder<TRow>();
            }
        }

        #endregion Constructors

        #region PublicInterface
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///  This method returns an strongly typed iterator
        ///  for the underlying DataRow collection.
        /// </summary>
        /// <returns>
        ///   A strongly typed iterator.
        /// </returns>
        public IEnumerator<TRow> GetEnumerator()
        {
            return _enumerableRows.GetEnumerator();
        }
        #endregion PublicInterface

        #region Add Single Filter/Sort Expression

        /// <summary>
        /// Used to add a filter predicate.
        /// A conjunction of all predicates are evaluated in LinqDataView
        /// </summary>
        internal void AddPredicate(Func<TRow, bool> pred)
        {
            Debug.Assert(pred != null);
            _listOfPredicates.Add(pred);
        }

        /// <summary>
        /// Adds a sort expression when Keyselector is provided but not Comparer
        /// </summary>
        internal void AddSortExpression<TKey>(Func<TRow, TKey> keySelector, bool isDescending, bool isOrderBy)
        {
            AddSortExpression<TKey>(keySelector, Comparer<TKey>.Default, isDescending, isOrderBy);
        }

        /// <summary>
        /// Adds a sort expression when Keyselector and Comparer are provided.
        /// </summary>
        internal void AddSortExpression<TKey>(
                            Func<TRow, TKey> keySelector,
                            IComparer<TKey> comparer,
                            bool isDescending,
                            bool isOrderBy)
        {
            DataSetUtil.CheckArgumentNull(keySelector, "keySelector");
            DataSetUtil.CheckArgumentNull(comparer, "comparer");

            _sortExpression.Add(
                    delegate(TRow input)
                    {
                        return (object)keySelector(input);
                    },
                    delegate(object val1, object val2)
                    {
                        return (isDescending ? -1 : 1) * comparer.Compare((TKey)val1, (TKey)val2);
                    },
                      isOrderBy);
        }

        #endregion Add Single Filter/Sort Expression
    }
}
