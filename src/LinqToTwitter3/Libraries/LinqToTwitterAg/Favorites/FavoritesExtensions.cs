﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToTwitter
{
    public static class FavoritesExtensions
    {
        /// <summary>
        /// Adds a favorite to the logged-in user's profile
        /// </summary>
        /// <param name="id">id of status to add to favorites</param>
        /// <returns>status of favorite</returns>
        public static Status CreateFavorite(this TwitterContext ctx, string id)
        {
            return CreateFavorite(ctx, id, true, null);
        }

        /// <summary>
        /// Adds a favorite to the logged-in user's profile
        /// </summary>
        /// <param name="id">id of status to add to favorites</param>
        /// <param name="includeEntities">Response doesn't include entities when false. (default: true)</param>
        /// <returns>status of favorite</returns>
        public static Status CreateFavorite(this TwitterContext ctx, string id, bool includeEntities)
        {
            return CreateFavorite(ctx, id, true, null);
        }

        /// <summary>
        /// Adds a favorite to the logged-in user's profile
        /// </summary>
        /// <param name="id">id of status to add to favorites</param>
        /// <param name="includeEntities">Response doesn't include entities when false. (default: true)</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>status of favorite</returns>
        public static Status CreateFavorite(this TwitterContext ctx, string id, bool includeEntities, Action<TwitterAsyncResponse<Status>> callback)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var favoritesUrl = ctx.BaseUrl + "favorites/create.json";

            var reqProc = new StatusRequestProcessor<Status>();

            ITwitterExecute twitExe = ctx.TwitterExecutor;

            twitExe.AsyncCallback = callback;
            var resultsJson =
                twitExe.PostToTwitter(
                    favoritesUrl,
                    new Dictionary<string, string>
                    {
                        {"id", id},
                        {"include_entities", includeEntities.ToString()}
                    },
                    response => reqProc.ProcessActionResult(response, FavoritesAction.SingleStatus));

            Status result = reqProc.ProcessActionResult(resultsJson, FavoritesAction.SingleStatus);
            return result;
        }

        /// <summary>
        /// Deletes a favorite from the logged-in user's profile
        /// </summary>
        /// <param name="id">id of status to add to favorites</param>
        /// <returns>status of favorite</returns>
        public static Status DestroyFavorite(this TwitterContext ctx, string id)
        {
            return DestroyFavorite(ctx, id, true, null);
        }

        /// <summary>
        /// Deletes a favorite from the logged-in user's profile
        /// </summary>
        /// <param name="id">id of status to add to favorites</param>
        /// <param name="includeEntities">Response doesn't include entities when false. (default: true)</param>
        /// <returns>status of favorite</returns>
        public static Status DestroyFavorite(this TwitterContext ctx, string id, bool includeEntities)
        {
            return DestroyFavorite(ctx, id, includeEntities, null);
        }

        /// <summary>
        /// Deletes a favorite from the logged-in user's profile
        /// </summary>
        /// <param name="id">id of status to add to favorites</param>
        /// <param name="includeEntities">Response doesn't include entities when false. (default: true)</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>status of favorite</returns>
        public static Status DestroyFavorite(this TwitterContext ctx, string id, bool includeEntities, Action<TwitterAsyncResponse<Status>> callback)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var favoritesUrl = ctx.BaseUrl + "favorites/destroy.json";

            var reqProc = new StatusRequestProcessor<Status>();

            ITwitterExecute twitExe = ctx.TwitterExecutor;

            twitExe.AsyncCallback = callback;
            var resultsJson =
                twitExe.PostToTwitter(
                    favoritesUrl,
                    new Dictionary<string, string>
                    {
                        {"id", id},
                        {"include_entities", includeEntities.ToString()}
                    },
                    response => reqProc.ProcessActionResult(response, FavoritesAction.SingleStatus));

            Status result = reqProc.ProcessActionResult(resultsJson, FavoritesAction.SingleStatus);
            return result;
        }

    }
}
