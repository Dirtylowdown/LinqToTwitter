﻿using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using Moq;
using Xunit;

namespace LinqToTwitterXUnitTests.AccountTests
{
    public class AccountExtensionsTests
    {
        Mock<ITwitterExecute> execMock;

        public AccountExtensionsTests()
        {
            TestCulture.SetCulture();
        }
  
        TwitterContext InitTwitterContextWithPostToTwitter<TEntity>(string response)
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(
                exec => exec.PostToTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<Func<string, TEntity>>()))
                    .Returns(response);
            var ctx = new TwitterContext(execMock.Object);
            return ctx;
        }

        TwitterContext InitTwitterContextWithPostTwitterImage()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(
                exec => exec.PostTwitterImage(
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IRequestProcessor<User>>()))
                    .Returns(SingleUserResponse);
            var ctx = new TwitterContext(execMock.Object);
            return ctx;
        }

        [Fact]
        public void UpdateAccountProfile_Invokes_Executor_Execute()
        {
            const string ExpectedName = "Twitter API";
            const string Name = "Twitter API";
            const string Url = "http://www.csharp-station.com";
            const string Location = "San Francisco, CA";
            const string Description = "The Real Twitter API.";
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            User actual = ctx.UpdateAccountProfile(Name, Url, Location, Description, true, SkipStatus);

            execMock.Verify(exec =>
                exec.PostToTwitter(
                    "https://api.twitter.com/1.1/account/update_profile.json",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<Func<string, User>>()),
                Times.Once());
            Assert.Equal(ExpectedName, actual.Name);
        }

        [Fact]
        public void UpdateAccountProfile_Throws_On_Null_Input()
        {
            const string ExpectedParamName = "NoInput";
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = Assert.Throws<ArgumentException>(() => ctx.UpdateAccountProfile(null, null, null, null, true, false));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void UpdateAccountProfile_Throws_On_Name_Over_20_Chars()
        {
            const string ExpectedParamName = "name";
            string name = new string(Enumerable.Repeat('x', 21).ToArray());
            const string Url = "http://www.csharp-station.com";
            const string Location = "San Francisco, CA";
            const string Description = "The Real Twitter API.";
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = Assert.Throws<ArgumentException>(() => ctx.UpdateAccountProfile(name, Url, Location, Description, true, SkipStatus));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void UpdateAccountProfile_Throws_On_Url_Over_100_Chars()
        {
            const string ExpectedParamName = "url";
            const string Name = "Joe";
            var url = new string(Enumerable.Repeat('x', 101).ToArray());
            const string Location = "Denver, CO";
            const string Description = "Open source developer for LINQ to Twitter";
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = Assert.Throws<ArgumentException>(() => ctx.UpdateAccountProfile(Name, url, Location, Description, true, SkipStatus));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void UpdateAccountProfile_Throws_On_Location_Over_30_Chars()
        {
            const string ExpectedParamName = "location";
            const string Name = "Joe";
            const string Url = "http://www.csharp-station.com";
            var location = new string(Enumerable.Repeat('x', 31).ToArray());
            const string Description = "Open source developer for LINQ to Twitter";
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = Assert.Throws<ArgumentException>(() => ctx.UpdateAccountProfile(Name, Url, location, Description, true, SkipStatus));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void UpdateAccountProfile_Throws_On_Description_Over_160_Chars()
        {
            const string ExpectedParamName = "description";
            const string Name = "Joe";
            const string Url = "http://www.csharp-station.com";
            const string Location = "Denver, CO";
            var description = new string(Enumerable.Repeat('x', 161).ToArray());
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = Assert.Throws<ArgumentException>(() => ctx.UpdateAccountProfile(Name, Url, Location, description, true, SkipStatus));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void UpdateAccountImage_Invokes_Executor_Execute()
        {
            const string ImageFilePath = "c:\\image.jpg";
            const string ExpectedName = "Twitter API";
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);
            execMock.Setup(exec =>
                exec.PostTwitterFile(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<string>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(SingleUserResponse);

            User actual = ctx.UpdateAccountImage(ImageFilePath, SkipStatus);

            execMock.Verify(exec =>
                exec.PostTwitterFile(
                    "https://api.twitter.com/1.1/account/update_profile_image.json",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<string>(),
                    It.IsAny<IRequestProcessor<User>>()),
                Times.Once());
            Assert.Equal(ExpectedName, actual.Name);
        }

        [Fact]
        public void UpdateAccountImage_Throws_On_Null_Path()
        {
            const string ExpectedParamName = "imageFilePath";
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = Assert.Throws<ArgumentException>(() => ctx.UpdateAccountImage(null, SkipStatus));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void UpdateAccountColors_Invokes_Executor_Execute()
        {
            const string Background = "9ae4e8";
            const string Text = "#000000";
            const string Link = "#0000ff";
            const string SidebarFill = "#e0ff92";
            const string SidebarBorder = "#87bc44";
            string expectedName = "Twitter API";
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            User actual = ctx.UpdateAccountColors(Background, Text, Link, SidebarFill, SidebarBorder, true, SkipStatus);

            execMock.Verify(exec =>
                exec.PostToTwitter(
                    "https://api.twitter.com/1.1/account/update_profile_colors.json",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<Func<string, User>>()),
                Times.Once());
            Assert.Equal(expectedName, actual.Name);
        }

        [Fact]
        public void UpdateAccountColors_Throws_On_No_Input()
        {
            const string ExpectedParamName = "NoInput";
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = Assert.Throws<ArgumentException>(() => ctx.UpdateAccountColors(null, null, null, null, null, true, SkipStatus));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void UpdateAccountColors_Allows_Null_Parameters()
        {
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            User user = ctx.UpdateAccountColors("#9ae4e8", null, null, null, null, true, SkipStatus);

            Assert.NotNull(user);

            user = ctx.UpdateAccountColors(null, "#9ae4e8", null, null, null, true, SkipStatus);

            Assert.NotNull(user);
        }

        [Fact]
        public void UpdateAccountBackgroundImage_Invokes_Executor_PostTwitterFile()
        {
            const string ImageFilePath = "C:\\image.png";
            const bool Tile = false;
            const bool Use = false;
            const string ExpectedName = "Twitter API";
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);
            execMock.Setup(exec =>
            exec.PostTwitterFile(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<string>(),
                It.IsAny<IRequestProcessor<User>>()))
                .Returns(SingleUserResponse);

            User actual = ctx.UpdateAccountBackgroundImage(ImageFilePath, Tile, Use, true, SkipStatus);

            execMock.Verify(exec =>
                exec.PostTwitterFile(
                    "https://api.twitter.com/1.1/account/update_profile_background_image.json",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<string>(),
                    It.IsAny<IRequestProcessor<User>>()),
                Times.Once());
            Assert.Equal(ExpectedName, actual.Name);
        }

        [Fact]
        public void UpdateAccountBackgroundImage_Throws_On_Null_Path()
        {
            const string ExpectedParamName = "imageFilePath";
            string imageFilePath = string.Empty;
            const bool Tile = false;
            const bool Use = false;
            const bool SkipStatus = true;
            var ctx = InitTwitterContextWithPostTwitterImage();

            var ex = Assert.Throws<ArgumentException>(() => ctx.UpdateAccountBackgroundImage(imageFilePath, Tile, Use, true, SkipStatus));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void UpdateAccountSettings_Invokes_Executor_Execute()
        {
            var ctx = InitTwitterContextWithPostToTwitter<Account>(SettingsResponse);
            var parameters = new Dictionary<string, string>
            {
                { "trend_location_woeid", "1" },
                { "sleep_time_enabled", "True" },
                { "start_sleep_time", "20" },
                { "end_sleep_time", "6" },
                { "time_zone", "MST" },
                { "lang", "en" }
            };

            Account acct = ctx.UpdateAccountSettings(1, true, 20, 6, "MST", "en");

            execMock.Verify(exec =>
                exec.PostToTwitter(
                    "https://api.twitter.com/1.1/account/settings.json",
                    parameters,
                    It.IsAny<Func<string, Account>>()),
                Times.Once());
            Assert.NotNull(acct);
            Settings settings = acct.Settings;
            Assert.NotNull(settings);
            Assert.Equal("en", settings.Language);
        }

        [Fact]
        public void UpdateAccountSettings_Throws_On_No_Input()
        {
            const string ExpectedParamName = "NoInput";
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            var ex = Assert.Throws<ArgumentException>(() => ctx.UpdateAccountSettings(null, null, null, null, null, null));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void UpdateDeliveryDevice_Invokes_Executor_Execute()
        {
            var ctx = InitTwitterContextWithPostToTwitter<Account>(SettingsResponse);
            var parameters = new Dictionary<string, string>
            {
                { "device", DeviceType.Sms.ToString().ToLower() },
                { "include_entities", true.ToString().ToLower() }
            };

            Account acct = ctx.UpdateDeliveryDevice(DeviceType.Sms, true);

            execMock.Verify(exec =>
                exec.PostToTwitter(
                    "https://api.twitter.com/1.1/account/update_delivery_device.json",
                    parameters,
                    It.IsAny<Func<string, Account>>()),
                Times.Once());
            Assert.NotNull(acct);
            Settings settings = acct.Settings;
            Assert.NotNull(settings);
        }

        [Fact]
        public void UpdateProfileBanner_Invokes_Executor_Execute()
        {
            const string ExpectedProfileBannerUrl = "https://si0.twimg.com/profile_images/1438634086/avatar_normal.png";
            byte[] banner = new byte[]{ 1, 2, 3 };
            const string FileName = "MyImage.png";
            const string FileType = "png";
            const int Width = 1252;
            const int Height = 626;
            const int OffsetLeft = 1;
            const int OffsetRight = 1;
            var ctx = InitTwitterContextWithPostTwitterImage();

            User actual = ctx.UpdateProfileBanner(banner, FileName, FileType, Width, Height, OffsetLeft, OffsetRight, null);

            execMock.Verify(exec =>
                exec.PostTwitterImage(
                    "https://api.twitter.com/1.1/account/update_profile_banner.json",
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IRequestProcessor<User>>()),
                Times.Once());
            Assert.NotNull(actual);
            Assert.NotNull(actual.ProfileBannerUrl);
            Assert.Equal(ExpectedProfileBannerUrl, actual.ProfileBannerUrl);
        }

        [Fact]
        public void UpdateProfileBanner_Throws_On_Null_Banner()
        {
            const string ExpectedParamName = "banner";
            byte[] banner = null;
            const string FileName = "MyImage.png";
            const string FileType = "png";
            const int Width = 1252;
            const int Height = 626;
            const int OffsetLeft = 1;
            const int OffsetRight = 1;
            var ctx = InitTwitterContextWithPostTwitterImage();

            var ex = Assert.Throws<ArgumentException>(() => ctx.UpdateProfileBanner(banner, FileName, FileType, Width, Height, OffsetLeft, OffsetRight, null));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void UpdateProfileBanner_Throws_On_Empty_Banner()
        {
            const string ExpectedParamName = "banner";
            byte[] banner = new byte[0];
            const string FileName = "MyImage.png";
            const string FileType = "png";
            const int Width = 1252;
            const int Height = 626;
            const int OffsetLeft = 1;
            const int OffsetRight = 1;
            var ctx = InitTwitterContextWithPostTwitterImage();

            var ex = Assert.Throws<ArgumentException>(() => ctx.UpdateProfileBanner(banner, FileName, FileType, Width, Height, OffsetLeft, OffsetRight, null));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void UpdateProfileBanner_Throws_On_Null_FileName()
        {
            const string ExpectedParamName = "fileName";
            byte[] banner = new byte[] { 1, 2, 3 };
            const string FileName = null;
            const string FileType = "png";
            const int Width = 1252;
            const int Height = 626;
            const int OffsetLeft = 1;
            const int OffsetRight = 1;
            var ctx = InitTwitterContextWithPostTwitterImage();

            var ex = Assert.Throws<ArgumentException>(() => ctx.UpdateProfileBanner(banner, FileName, FileType, Width, Height, OffsetLeft, OffsetRight, null));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void UpdateProfileBanner_Throws_On_Empty_FileName()
        {
            const string ExpectedParamName = "fileName";
            byte[] banner = new byte[] { 1, 2, 3 };
            const string FileName = "";
            const string FileType = "png";
            const int Width = 1252;
            const int Height = 626;
            const int OffsetLeft = 1;
            const int OffsetRight = 1;
            var ctx = InitTwitterContextWithPostTwitterImage();

            var ex = Assert.Throws<ArgumentException>(() => ctx.UpdateProfileBanner(banner, FileName, FileType, Width, Height, OffsetLeft, OffsetRight, null));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void UpdateProfileBanner_Throws_On_Null_FileType()
        {
            const string ExpectedParamName = "imageType";
            byte[] banner = new byte[] { 1, 2, 3 };
            const string FileName = "MyFile.png";
            const string FileType = null;
            const int Width = 1252;
            const int Height = 626;
            const int OffsetLeft = 1;
            const int OffsetRight = 1;
            var ctx = InitTwitterContextWithPostTwitterImage();

            var ex = Assert.Throws<ArgumentException>(() => ctx.UpdateProfileBanner(banner, FileName, FileType, Width, Height, OffsetLeft, OffsetRight, null));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void UpdateProfileBanner_Throws_On_Empty_FileType()
        {
            const string ExpectedParamName = "imageType";
            byte[] banner = new byte[] { 1, 2, 3 };
            const string FileName = "MyFile.png";
            const string FileType = "";
            const int Width = 1252;
            const int Height = 626;
            const int OffsetLeft = 1;
            const int OffsetRight = 1;
            var ctx = InitTwitterContextWithPostTwitterImage();

            var ex = Assert.Throws<ArgumentException>(() => ctx.UpdateProfileBanner(banner, FileName, FileType, Width, Height, OffsetLeft, OffsetRight, null));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void RemoveProfileBanner_Invokes_Executor_Execute()
        {
            var ctx = InitTwitterContextWithPostToTwitter<User>(SingleUserResponse);

            User actual = ctx.RemoveProfileBanner(null);

            execMock.Verify(exec =>
                exec.PostToTwitter(
                    "https://api.twitter.com/1.1/account/remove_profile_banner.json",
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<Func<string, User>>()),
                Times.Once());
            Assert.NotNull(actual);
        }

        const string SingleUserResponse = @"{
   ""id"":6253282,
   ""id_str"":""6253282"",
   ""name"":""Twitter API"",
   ""screen_name"":""twitterapi"",
   ""location"":""San Francisco, CA"",
   ""description"":""The Real Twitter API. I tweet about API changes, service issues and happily answer questions about Twitter and our API. Don't get an answer? It's on my website."",
   ""url"":""http:\/\/dev.twitter.com"",
   ""protected"":false,
   ""followers_count"":1009508,
   ""friends_count"":31,
   ""listed_count"":10361,
   ""created_at"":""Wed May 23 06:01:13 +0000 2007"",
   ""favourites_count"":24,
   ""utc_offset"":-28800,
   ""time_zone"":""Pacific Time (US & Canada)"",
   ""geo_enabled"":true,
   ""verified"":true,
   ""statuses_count"":3278,
   ""lang"":""en"",
   ""status"":{
      ""created_at"":""Mon Apr 30 17:16:17 +0000 2012"",
      ""id"":197011505181507585,
      ""id_str"":""197011505181507585"",
      ""text"":""Developer Teatime is coming to Paris - please sign up to join us on June 16th! https:\/\/t.co\/pQOUNKGD  @rno @jasoncosta"",
      ""source"":""web"",
      ""truncated"":false,
      ""in_reply_to_status_id"":null,
      ""in_reply_to_status_id_str"":null,
      ""in_reply_to_user_id"":null,
      ""in_reply_to_user_id_str"":null,
      ""in_reply_to_screen_name"":null,
      ""geo"":null,
      ""coordinates"":null,
      ""place"":null,
      ""contributors"":[
         14927800
      ],
      ""retweet_count"":25,
      ""favorited"":false,
      ""retweeted"":false,
      ""possibly_sensitive"":false
   },
   ""contributors_enabled"":true,
   ""is_translator"":false,
   ""profile_background_color"":""E8F2F7"",
   ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/229557229\/twitterapi-bg.png"",
   ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/229557229\/twitterapi-bg.png"",
   ""profile_background_tile"":false,
   ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1438634086\/avatar_normal.png"",
   ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1438634086\/avatar_normal.png"",
   ""profile_banner_url"":""https:\/\/si0.twimg.com\/profile_images\/1438634086\/avatar_normal.png"",   
   ""profile_link_color"":""0094C2"",
   ""profile_sidebar_border_color"":""0094C2"",
   ""profile_sidebar_fill_color"":""A9D9F1"",
   ""profile_text_color"":""437792"",
   ""profile_use_background_image"":true,
   ""show_all_inline_media"":false,
   ""default_profile"":false,
   ""default_profile_image"":false,
   ""following"":false,
   ""follow_request_sent"":false,
   ""notifications"":false
}";

        const string SettingsResponse = @"{
   ""screen_name"":""Linq2Tweeter"",
   ""protected"":false,
   ""geo_enabled"":false,
   ""time_zone"":{
      ""name"":""Mountain Time (US & Canada)"",
      ""utc_offset"":-25200,
      ""tzinfo_name"":""America\/Denver""
   },
   ""sleep_time"":{
      ""enabled"":true,
      ""start_time"":20,
      ""end_time"":8
   },
   ""show_all_inline_media"":true,
   ""discoverable_by_email"":true,
   ""trend_location"":[
      {
         ""woeid"":23424977,
         ""name"":""United States"",
         ""country"":""United States"",
         ""countryCode"":""US"",
         ""placeType"":{
            ""name"":""Country"",
            ""code"":12
         },
         ""url"":""http:\/\/where.yahooapis.com\/v1\/place\/23424977"",
         ""parentid"":1
      }
   ],
   ""language"":""en"",
   ""always_use_https"":true
}";
    }
}
