﻿using System.Collections.Generic;
using System.Linq;

namespace SudokuSharp
{
    public partial class Board
    {
        public partial class _FindClass
        {
            /// <summary>
            /// Looks for Naked Singles. These are cells with only a single candidate
            /// </summary>
            /// <returns>An enumeration of tuples containing a <see cref="Location"/> and <see cref="int"/></returns>
            public IEnumerable<(Location Cell, int Value)> NakedSingles()
            {
                return NakedSingles(AllCandidates());
            }

            /// <summary>
            /// Looks for Naked Singles. These are cells with only a single candidate.
            /// This version is intended to be called by other reducing functions (such as LockedCandidates)
            /// </summary>
            /// <param name="Possibilities">A set of candidates</param>
            /// <returns>An enumeration of tuples containing a <see cref="Location"/> and <see cref="int"/></returns>
            public IEnumerable<(Location Cell, int Value)> NakedSingles(IEnumerable<KeyValuePair<Location, List<int>>> Possibilities)
            {
                foreach (var test in Possibilities)
                {
                    if (test.Value.Count == 1)
                        yield return (test.Key, test.Value.First());
                }
            }

            /// <summary>
            /// Looks for Hidden Singles. These are digits which may only be placed in one cell within a row, column, or zone
            /// </summary>
            /// <returns>An enumeration of tuples containing a <see cref="Location"/> and <see cref="int"/></returns>
            public IEnumerable<(Location Cell, int Value)> HiddenSingles()
            {
                return HiddenSingles(AllCandidates());
            }

            /// <summary>
            /// Looks for Hidden Singles. These are digits which may only be placed in one cell within a row, column, or zone
            /// This version is intended to be called by other reducing functions (such as LockedCandidates)
            /// </summary>
            /// <param name="Possibilities">A set of candidates</param>
            /// <returns>An enumeration of tuples containing a <see cref="Location"/> and <see cref="int"/></returns>
            public IEnumerable<(Location Cell, int Value)> HiddenSingles(IEnumerable<KeyValuePair<Location, List<int>>> Possibilities)
            {
                Dictionary<Location, int> results = new Dictionary<Location, int>();

                for (int number = 1; number < 10; number++)
                {
                    var locationsForThisNumber = from item in Possibilities
                                                 where item.Value.Contains(number)
                                                 select item.Key;

                    for (int test = 0; test < 9; test++)
                    {
                        var possible = from item in locationsForThisNumber where item.Zone == test select item;
                        if (possible.Count() == 1) results[possible.First()] = number;
                        possible = from item in locationsForThisNumber where item.Row == test select item;
                        if (possible.Count() == 1) results[possible.First()] = number;
                        possible = from item in locationsForThisNumber where item.Column == test select item;
                        if (possible.Count() == 1) results[possible.First()] = number;

                        if (((from item in locationsForThisNumber where item.Zone == test select item).Count() == 1)
                            || ((from item in locationsForThisNumber where item.Row == test select item).Count() == 1)
                            || ((from item in locationsForThisNumber where item.Column == test select item).Count() == 1))
                            yield return (possible.First(), number);
                    }
                }
            }

            /// <summary>
            /// Returns a the results of both Naked Singles and Hidden Singles
            /// </summary>
            /// <returns>An enumeration of tuples containing a <see cref="Location"/> and <see cref="int"/></returns>
            public IEnumerable<(Location Cell, int Value)> AllSingles()
            {
                foreach (var result in AllSingles(AllCandidates()))
                    yield return result;
            }

            /// <summary>
            /// Returns a the results of both Naked Singles and Hidden Singles
            /// This version is intended to be called by other reducing functions (such as LockedCandidates)
            /// </summary>
            /// <param name="Possibilities">A set of candidates</param>
            /// <returns>An enumeration of tuples containing a <see cref="Location"/> and <see cref="int"/></returns>
            public IEnumerable<(Location Cell, int Value)> AllSingles(IEnumerable<KeyValuePair<Location, List<int>>> Possibilities)
            {
                foreach (var result in NakedSingles(Possibilities))
                    yield return result;

                foreach (var result in HiddenSingles(Possibilities))
                    yield return result;
            }
        }
    }
}
