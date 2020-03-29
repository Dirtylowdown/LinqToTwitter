﻿using System;
using System.Linq;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    /// <summary>
    /// Shows saved search demos
    /// </summary>
    public class SavedSearchDemos
    {
        /// <summary>
        /// Run all saved search related demos
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        public static void Run(TwitterContext twitterCtx)
        {
            QuerySavedSearchesDemo(twitterCtx);
            //QuerySavedSearchesShowDemo(twitterCtx);
            //CreateSavedSearchDemo(twitterCtx);
            //DestroySavedSearchDemo(twitterCtx);
        }

        /// <summary>
        /// Shows how to delete a saved search
        /// </summary>
        /// <remarks>
        /// Trying to delete a saved search that doesn't exist results
        /// in a TwitterQueryException with HTTP Status 404 (Not Found)
        /// </remarks>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void DestroySavedSearchDemo(TwitterContext twitterCtx)
        {
            var savedSearch = twitterCtx.DestroySavedSearch(167246334);

            Console.WriteLine("ID: {0}, Search: {1}", savedSearch.ID, savedSearch.Name);
        }

        /// <summary>
        /// shows how to create a Saved Search
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void CreateSavedSearchDemo(TwitterContext twitterCtx)
        {
            var savedSearch = twitterCtx.CreateSavedSearch("#csharp");

            Console.WriteLine("ID: {0}, Search: {1}", savedSearch.IDString, savedSearch.Query);
        }

        /// <summary>
        /// shows how to retrieve a single search
        /// </summary>
        /// <remarks>
        /// Trying to delete a saved search that doesn't exist results
        /// in a TwitterQueryException with HTTP Status 404 (Not Found)
        /// </remarks>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void QuerySavedSearchesShowDemo(TwitterContext twitterCtx)
        {
            var savedSearches =
                from search in twitterCtx.SavedSearch
                where search.Type == SavedSearchType.Show &&
                      search.ID == "3275867"
                select search;

            var savedSearch = savedSearches.FirstOrDefault();

            Console.WriteLine("ID: {0}, Search: {1}", savedSearch.ID, savedSearch.Name);
        }

        /// <summary>
        /// shows how to retrieve all searches
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void QuerySavedSearchesDemo(TwitterContext twitterCtx)
        {
            var savedSearches =
                from search in twitterCtx.SavedSearch
                where search.Type == SavedSearchType.Searches
                select search;

            foreach (var search in savedSearches)
            {
                Console.WriteLine("ID: {0}, Search: {1}", search.IDString, search.Name);
            }
        }
    }
}
