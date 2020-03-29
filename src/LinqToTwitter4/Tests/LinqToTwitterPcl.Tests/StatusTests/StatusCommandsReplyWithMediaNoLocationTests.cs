﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using LinqToTwitterPcl.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;

namespace LinqToTwitterPcl.Tests.StatusTests
{
    [TestClass]
    public class StatusCommandsReplyWithMediaNoLocationTests
    {
        readonly string expectedUploadUrl = "https://api.twitter.com/1.1/statuses/update_with_media.json";

        readonly Mock<TwitterContext> ctx;
        readonly Mock<ITwitterExecute> execMock;
        readonly Mock<IRequestProcessor<Status>> statusReqProc;

        readonly byte[] imageBytes = new byte[] { 0xFF };

        string status = "test";
        bool possiblySensitive = true;
        decimal latitude = 37.78215m;
        ulong inReplyToStatusID = 23030327348932ul;

        public StatusCommandsReplyWithMediaNoLocationTests()
        {
            statusReqProc = new Mock<IRequestProcessor<Status>>();
            statusReqProc.Setup(reqProc => reqProc.ProcessResults(It.IsAny<string>()))
            .Returns(new List<Status> { new Status { Text = "Test" } });

            execMock = new Mock<ITwitterExecute>();
            var authMock = new Mock<IAuthorizer>();
            var tcsResponse = new TaskCompletionSource<string>();
            tcsResponse.SetResult(SingleStatusResponse);
            execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.Authorizer).Returns(authMock.Object);
            execMock.Setup(
                exec => exec.PostMediaAsync(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                    .Returns(tcsResponse.Task);

            ctx = new Mock<TwitterContext>(execMock.Object);
            ctx.Setup(twtrCtx => twtrCtx.CreateRequestProcessor<Status>())
            .Returns(statusReqProc.Object);
        }

        public decimal Latitude
        {
            get
            {
                return latitude;
            }
            set
            {
                latitude = value;
            }
        }

        [TestMethod]
        public async Task ReplyWithMedia_Without_Location_Params_Throws_On_Null_Status()
        {
            status = null;

            ArgumentNullException ex =
            await L2TAssert.Throws<ArgumentNullException>(async () =>
                await ctx.Object.ReplyWithMediaAsync(
                    inReplyToStatusID, status, possiblySensitive, imageBytes));

            Assert.AreEqual("status", ex.ParamName);
        }

        [TestMethod]
        public async Task ReplyWithMedia_Without_Location_Params_Calls_PostMedia()
        {
            await ctx.Object.ReplyWithMediaAsync(
                inReplyToStatusID,
                status, possiblySensitive, imageBytes);

            execMock.Verify(exec =>
                exec.PostMediaAsync(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once(),
                "PostMedia was not called only one time.");
        }

        [TestMethod]
        public async Task ReplyWithMedia_Without_Location_Params_Passes_Properly_Formatted_Url_To_PostMedia()
        {
            await ctx.Object.ReplyWithMediaAsync(
                inReplyToStatusID,
                status, possiblySensitive, imageBytes);

            execMock.Verify(exec =>
                exec.PostMediaAsync(
                    It.Is<string>(url => url == expectedUploadUrl),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once(),
                "ReplyWithMedia didn't pass properly formatted URL.");
        }

        [TestMethod]
        public async Task ReplyWithMedia_Without_Location_Params_Passes_Status_Via_Parameter_Dictionary_To_PostMedia()
        {
            await ctx.Object.ReplyWithMediaAsync(
                inReplyToStatusID,
                status, possiblySensitive, imageBytes);

            execMock.Verify(exec =>
                exec.PostMediaAsync(
                    It.IsAny<string>(),
                    It.Is<IDictionary<string, string>>(parms => parms["status"] == status),
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once(),
                "ReplyWithMedia didn't pass status properly.");
        }

        [TestMethod]
        public async Task ReplyWithMedia_Without_Location_Params_Passes_PossiblySensitive_Via_Parameter_Dictionary_To_PostMedia()
        {
            await ctx.Object.ReplyWithMediaAsync(
                inReplyToStatusID,
                status, possiblySensitive, imageBytes);

            execMock.Verify(exec =>
                exec.PostMediaAsync(
                    It.IsAny<string>(),
                    It.Is<IDictionary<string, string>>(parms => parms["possibly_sensitive"] == possiblySensitive.ToString()),
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once(),
                "ReplyWithMedia didn't pass possiblySensitive parameter properly.");
        }

        [TestMethod]
        public async Task ReplyWithMedia_Without_Location_Params_DoesNot_Pass_False_PossiblySensitive_Via_Parameter_Dictionary_To_PostMedia()
        {
            possiblySensitive = false;

            await ctx.Object.ReplyWithMediaAsync(
                inReplyToStatusID,
                status, possiblySensitive, imageBytes);

            execMock.Verify(exec =>
                exec.PostMediaAsync(
                    It.IsAny<string>(),
                    It.Is<IDictionary<string, string>>(parms => parms["possibly_sensitive"] == null),
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once(),
                "ReplyWithMedia should not have passed possiblySensitive parameter.");
        }

        [TestMethod]
        public async Task ReplyWithMedia_Without_Location_Params_DoesNot_Pass_PlaceID_Via_Parameter_Dictionary_To_PostMedia()
        {
            await ctx.Object.ReplyWithMediaAsync(
                inReplyToStatusID,
                status, possiblySensitive, imageBytes);

            execMock.Verify(exec =>
                exec.PostMediaAsync(
                    It.IsAny<string>(),
                    It.Is<IDictionary<string, string>>(parms => parms["place_id"] == null),
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once(),
                "ReplyWithMedia shouldn't pass placeID parameter.");
        }

        [TestMethod]
        public async Task ReplyWithMedia_Without_Location_Params_DoesNot_Pass_DisplayCoordinates_Via_Parameter_Dictionary_To_PostMedia()
        {
            await ctx.Object.ReplyWithMediaAsync(
                inReplyToStatusID,
                status, possiblySensitive, imageBytes);

            execMock.Verify(exec =>
                exec.PostMediaAsync(
                    It.IsAny<string>(),
                    It.Is<IDictionary<string, string>>(parms => parms["display_coordinates"] == null),
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once(),
                "ReplyWithMedia shouldn't pass displayCoordinates parameter.");
        }

        [TestMethod]
        public async Task ReplyWithMedia_Without_Location_Params_Passes_InReplyToStatusID_Via_Parameter_Dictionary_To_PostMedia()
        {
            await ctx.Object.ReplyWithMediaAsync(
                inReplyToStatusID,
                status, possiblySensitive, imageBytes);

            execMock.Verify(exec =>
                exec.PostMediaAsync(
                    It.IsAny<string>(),
                    It.Is<IDictionary<string, string>>(parms => parms["in_reply_to_status_id"] == inReplyToStatusID.ToString()),
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once(),
                "ReplyWithMedia didn't pass inReplyToStatusID parameter properly.");
        }

        [TestMethod]
        public async Task ReplyWithMedia_Without_Location_Params_DoesNot_Pass_NoReply_InReplyToStatusID_Via_Parameter_Dictionary_To_PostMedia()
        {
            inReplyToStatusID = TwitterContext.NoReply;

            await ctx.Object.ReplyWithMediaAsync(
                inReplyToStatusID,
                status, possiblySensitive, imageBytes);

            execMock.Verify(exec =>
                exec.PostMediaAsync(
                    It.IsAny<string>(),
                    It.Is<IDictionary<string, string>>(parms => parms["in_reply_to_status_id"] == null),
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once(),
                "ReplyWithMedia shouldn't pass inReplyToStatusID parameter.");
        }

        [TestMethod]
        public async Task ReplyWithMedia_Without_Location_Params_DoesNot_Pass_Lat_And_Long_To_PostMedia()
        {
            latitude = TwitterContext.NoCoordinate;

            await ctx.Object.ReplyWithMediaAsync(
                inReplyToStatusID,
                status, possiblySensitive, imageBytes);

            execMock.Verify(exec =>
                exec.PostMediaAsync(
                    It.IsAny<string>(),
                    It.Is<IDictionary<string, string>>(
                        parms =>
                        parms["lat"] == null &&
                        parms["long"] == null),
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once(),
                "Lat and long should be null.");
        }

        [TestMethod]
        public async Task ReplyWithMediaAsync_Without_Location_Params_Passes_Image_To_PostMediaAsync()
        {
            await ctx.Object.ReplyWithMediaAsync(
                inReplyToStatusID,
                status, possiblySensitive, imageBytes);

            execMock.Verify(exec =>
                exec.PostMediaAsync(
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.Is<byte[]>(image => object.ReferenceEquals(image, imageBytes)),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once(),
                "ReplyWithMedia didn't pass mediaItems properly.");
        }

        [TestMethod]
        public async Task ReplyWithMedia_Without_Location_Params_Requires_NonNull_Image()
        {
            byte[] nullImage = null;

            ArgumentNullException ex =
                await L2TAssert.Throws<ArgumentNullException>(async () =>
                    await ctx.Object.ReplyWithMediaAsync(
                        inReplyToStatusID, status, possiblySensitive, nullImage));

            Assert.AreEqual("image", ex.ParamName);
        }

        [TestMethod]
        public async Task ReplyWithMedia_Without_Location_Params_Requires_NonEmpty_Array()
        {
            byte[] imageBytes = new byte[] {};

            ArgumentException ex =
                await L2TAssert.Throws<ArgumentException>(async () =>
                    await ctx.Object.ReplyWithMediaAsync(
                        inReplyToStatusID, status, possiblySensitive, imageBytes));

            Assert.AreEqual("image", ex.ParamName);
        }

        [TestMethod]
        public async Task ReplyWithMedia_Without_Location_Params_Returns_Status()
        {
            Status tweet = await ctx.Object.ReplyWithMediaAsync(
                inReplyToStatusID, status, possiblySensitive, imageBytes);

            Assert.IsTrue(tweet.Text.StartsWith("RT @scottgu: I just blogged about"));
        }

        const string SingleStatusResponse = @"{
      ""retweeted"":false,
      ""in_reply_to_screen_name"":null,
      ""possibly_sensitive"":false,
      ""retweeted_status"":{
         ""retweeted"":false,
         ""in_reply_to_screen_name"":null,
         ""possibly_sensitive"":false,
         ""contributors"":null,
         ""coordinates"":null,
         ""place"":null,
         ""user"":{
            ""id"":41754227,
            ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/565139568\/redshirt_normal.jpg"",
            ""url"":""http:\/\/weblogs.asp.net\/scottgu"",
            ""created_at"":""Fri May 22 04:39:35 +0000 2009"",
            ""followers_count"":57222,
            ""default_profile"":true,
            ""profile_background_color"":""C0DEED"",
            ""lang"":""en"",
            ""utc_offset"":-28800,
            ""name"":""Scott Guthrie"",
            ""profile_background_image_url"":""http:\/\/a0.twimg.com\/images\/themes\/theme1\/bg.png"",
            ""location"":""Redmond, WA"",
            ""profile_link_color"":""0084B4"",
            ""listed_count"":4390,
            ""verified"":false,
            ""protected"":false,
            ""profile_use_background_image"":true,
            ""is_translator"":false,
            ""following"":false,
            ""description"":""I live in Seattle and build a few products for Microsoft"",
            ""profile_text_color"":""333333"",
            ""statuses_count"":3054,
            ""screen_name"":""scottgu"",
            ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/565139568\/redshirt_normal.jpg"",
            ""time_zone"":""Pacific Time (US & Canada)"",
            ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/images\/themes\/theme1\/bg.png"",
            ""friends_count"":86,
            ""default_profile_image"":false,
            ""contributors_enabled"":false,
            ""profile_sidebar_border_color"":""C0DEED"",
            ""id_str"":""41754227"",
            ""geo_enabled"":false,
            ""favourites_count"":44,
            ""profile_background_tile"":false,
            ""notifications"":false,
            ""show_all_inline_media"":false,
            ""profile_sidebar_fill_color"":""DDEEF6"",
            ""follow_request_sent"":false
         },
         ""retweet_count"":393,
         ""id_str"":""184793217231880192"",
         ""in_reply_to_user_id"":null,
         ""favorited"":false,
         ""in_reply_to_status_id_str"":null,
         ""in_reply_to_status_id"":null,
         ""source"":""web"",
         ""created_at"":""Wed Mar 28 00:05:10 +0000 2012"",
         ""in_reply_to_user_id_str"":null,
         ""truncated"":false,
         ""id"":184793217231880192,
         ""geo"":null,
         ""text"":""I just blogged about http:\/\/t.co\/YWHGwOq6 MVC, Web API, Razor and Open Source - Now with Contributions: http:\/\/t.co\/qpevLMZd""
      },
      ""contributors"":null,
      ""coordinates"":null,
      ""place"":null,
      ""user"":{
         ""id"":15411837,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
         ""url"":""http:\/\/www.mayosoftware.com"",
         ""created_at"":""Sun Jul 13 04:35:50 +0000 2008"",
         ""followers_count"":1102,
         ""default_profile"":false,
         ""profile_background_color"":""0099B9"",
         ""lang"":""en"",
         ""utc_offset"":-25200,
         ""name"":""Joe Mayo"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
         ""location"":""Denver, CO"",
         ""profile_link_color"":""0099B9"",
         ""listed_count"":112,
         ""verified"":false,
         ""protected"":false,
         ""profile_use_background_image"":true,
         ""is_translator"":false,
         ""following"":true,
         ""description"":""Independent .NET Consultant; author of 6 books; Microsoft Visual C# MVP"",
         ""profile_text_color"":""3C3940"",
         ""statuses_count"":1906,
         ""screen_name"":""JoeMayo"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
         ""time_zone"":""Mountain Time (US & Canada)"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
         ""friends_count"":211,
         ""default_profile_image"":false,
         ""contributors_enabled"":false,
         ""profile_sidebar_border_color"":""5ED4DC"",
         ""id_str"":""15411837"",
         ""geo_enabled"":true,
         ""favourites_count"":44,
         ""profile_background_tile"":false,
         ""notifications"":true,
         ""show_all_inline_media"":false,
         ""profile_sidebar_fill_color"":""95E8EC"",
         ""follow_request_sent"":false
      },
      ""retweet_count"":393,
      ""id_str"":""184835136037191681"",
      ""in_reply_to_user_id"":null,
      ""favorited"":false,
      ""in_reply_to_status_id_str"":null,
      ""in_reply_to_status_id"":null,
      ""source"":""web"",
      ""created_at"":""Wed Mar 28 02:51:45 +0000 2012"",
      ""in_reply_to_user_id_str"":null,
      ""truncated"":false,
      ""id"":184835136037191681,
      ""geo"":null,
      ""text"":""RT @scottgu: I just blogged about http:\/\/t.co\/YWHGwOq6 MVC, Web API, Razor and Open Source - Now with Contributions: http:\/\/t.co\/qpevLMZd""
   }";
    }
}
