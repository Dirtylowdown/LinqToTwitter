﻿using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Linq2TwitterDemos_XForms.Models
{
    public class TweetViewModel : INotifyPropertyChanged
    {
        List<Tweet> tweets;
        public List<Tweet> Tweets
        {
            get { return tweets; }
            set
            {
                if (tweets == value) return;

                tweets = value;
                OnPropertyChanged();
            }
        }

        public async Task InitTweetViewModel()
        {
            var auth = new ApplicationOnlyAuthorizer()
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = "",
                    ConsumerSecret = "",
                },
            };
            await auth.AuthorizeAsync();

            var ctx = new TwitterContext(auth);

            var searchResponse = await
                (from search in ctx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == "\"LINQ to Twitter\""
                 select search)
                .SingleAsync();

            Tweets =
                (from tweet in searchResponse.Statuses
                 select new Tweet
                 {
                     StatusID = tweet.StatusID,
                     ScreenName = tweet.User.ScreenNameResponse,
                     Text = tweet.Text,
                     ImageUrl = tweet.User.ProfileImageUrl
                 })
                .ToList();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
                throw new ArgumentNullException("Can't call OnPropertyChanged with a null property name.", propertyName);

            var propChangedHandler = PropertyChanged;
            if (propChangedHandler != null)
                propChangedHandler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
