﻿using System;
using System.Linq;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    /// <summary>
    /// Shows favorites demos
    /// </summary>
    public class FavoritesDemos
    {
        /// <summary>
        /// Run all favorites related demos
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        public static void Run(TwitterContext twitterCtx)
        {
            FavoritesQueryDemo(twitterCtx);
            //CreateFavoriteDemo(twitterCtx);
            //DestroyFavoriteDemo(twitterCtx);
        }

        private static void DestroyFavoriteDemo(TwitterContext twitterCtx)
        {
            var status = twitterCtx.DestroyFavorite("265675496581373952", true);

            Console.WriteLine("User: {0}, Tweet: {1}", status.User.Name, status.Text);
        }

        /// <summary>
        /// Shows how to create a Favorite
        /// </summary>
        /// <param name="twitterCtx"></param>
        private static void CreateFavoriteDemo(TwitterContext twitterCtx)
        {
            var status = twitterCtx.CreateFavorite("265675496581373952");

            Console.WriteLine("User: {0}, Tweet: {1}", status.User.Name, status.Text);
        }

        /// <summary>
        /// shows how to request a favorites list
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void FavoritesQueryDemo(TwitterContext twitterCtx)
        {
            var favorites =
                (from fav in twitterCtx.Favorites
                 where fav.Type == FavoritesType.Favorites
                 select fav)
                .ToList();

            foreach (var fav in favorites)
            {
                Console.WriteLine(
                    "User Name: {0}, Tweet: {1}",
                    fav.User.Name, fav.Text);
            }
        }
    }
}
