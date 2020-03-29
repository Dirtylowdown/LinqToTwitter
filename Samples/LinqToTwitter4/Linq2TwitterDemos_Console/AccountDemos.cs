﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Linq2TwitterDemos_Console
{
    class AccountDemos
    {
        internal static async Task RunAsync(TwitterContext twitterCtx)
        {
            char key;

            do
            {
                ShowMenu();

                key = Console.ReadKey(true).KeyChar;

                switch (key)
                {
                    case '0':
                        Console.WriteLine("\n\tVerifying Credentials...\n");
                        await VerifyCredentialsAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tRequesting settings....\n");
                        await AccountSettingsAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tUpdating colors...\n");
                        await UpdateAccountColorsAsync(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\n\tUpdating image...\n");
                        await UpdateAccountImageAsync(twitterCtx);
                        break;
                    case '4':
                        Console.WriteLine("\n\tUpdating image...\n");
                        await UpdateAccountBackgroundImageAsync(twitterCtx);
                        break;
                    case '5':
                        Console.WriteLine("\n\tUpdating account...\n");
                        await UpdateAccountProfileAsync(twitterCtx);
                        break;
                    case '6':
                        Console.WriteLine("\n\tUpdating account...\n");
                        await UpdateAccountSettingsAsync(twitterCtx);
                        break;
                    case '7':
                        Console.WriteLine("\n\tUpdating device...\n");
                        await UpdateDeliveryDeviceAsync(twitterCtx);
                        break;
                    case '8':
                        Console.WriteLine("\n\tUpdating banner...\n");
                        await UpdateProfileBannerAsync(twitterCtx);
                        break;
                    case '9':
                        Console.WriteLine("\n\tRemoving banner...\n");
                        await RemoveProfileBannerAsync(twitterCtx);
                        break;
                    case 'q':
                    case 'Q':
                        Console.WriteLine("\nReturning...\n");
                        break;
                    default:
                        Console.WriteLine(key + " is unknown");
                        break;
                }

            } while (char.ToUpper(key) != 'Q');
        }

        static void ShowMenu()
        {
            Console.WriteLine("\nAccount Demos - Please select:\n");

            Console.WriteLine("\t 0. Verify Credentials");
            Console.WriteLine("\t 1. Get Account Settings");
            Console.WriteLine("\t 2. Update Account Colors");
            Console.WriteLine("\t 3. Update Account Image");
            Console.WriteLine("\t 4. Update Account Background Image");
            Console.WriteLine("\t 5. Update Account Profile");
            Console.WriteLine("\t 6. Update Account Settings");
            Console.WriteLine("\t 7. Update Delivery Device");
            Console.WriteLine("\t 8. Update Profile Banner");
            Console.WriteLine("\t 9. Remove Profile Banner");
            Console.WriteLine();
            Console.WriteLine("\t Q. Return to Main menu");
        }

        static async Task VerifyCredentialsAsync(TwitterContext twitterCtx)
        {
            try
            {
                var verifyResponse =
                    await
                        (from acct in twitterCtx.Account
                         where acct.Type == AccountType.VerifyCredentials
                         select acct)
                        .SingleOrDefaultAsync();

                if (verifyResponse != null && verifyResponse.User != null)
                {
                    User user = verifyResponse.User;

                    Console.WriteLine(
                        "Credentials are good for {0}.",
                        user.ScreenNameResponse); 
                }
            }
            catch (TwitterQueryException tqe)
            {
                Console.WriteLine(tqe.Message);
            }
        }

        static async Task AccountSettingsAsync(TwitterContext twitterCtx)
        {
            var settingsResponse =
                await
                (from acct in twitterCtx.Account
                 where acct.Type == AccountType.Settings
                 select acct)
                .SingleOrDefaultAsync();

            if (settingsResponse != null && 
                settingsResponse.Settings != null &&
                settingsResponse.Settings.TrendLocation != null &&
                settingsResponse.Settings.SleepTime != null)
            {
                var settings = settingsResponse.Settings;

                Console.WriteLine(
                    "Trend Location: {0}\nGeo Enabled: {1}\nSleep Enabled: {2}",
                    settings.TrendLocation.Name,
                    settings.GeoEnabled,
                    settings.SleepTime.Enabled); 
            }
        }

        static async Task UpdateAccountColorsAsync(TwitterContext twitterCtx)
        {
            var user = await twitterCtx.UpdateAccountColorsAsync(
                background: null, text: "#000000", link: "#0000ff", 
                sidebarFill: "#e0ff92", sidebarBorder: "#87bc44", 
                includeEntities: true, skipStatus: true);

            if (user != null)
            {
                Console.WriteLine("\nAccount Colors:\n");

                Console.WriteLine("Background:     " + user.ProfileBackgroundColor);
                Console.WriteLine("Text:           " + user.ProfileTextColor);
                Console.WriteLine("Link:           " + user.ProfileLinkColor);
                Console.WriteLine("Sidebar Fill:   " + user.ProfileSidebarFillColor);
                Console.WriteLine("Sidebar Border: " + user.ProfileSidebarBorderColor); 
            }
        }

        static async Task UpdateAccountImageAsync(TwitterContext twitterCtx)
        {
            byte[] imageBytes = File.ReadAllBytes(@"..\..\Images\200xColor_2.png");

            var user = await twitterCtx.UpdateAccountImageAsync(
                imageBytes, "200xColor_2.png", "png", false);

            if (user != null)
                Console.WriteLine("User Image: " + user.ProfileImageUrl); 
        }

        static async Task UpdateAccountBackgroundImageAsync(TwitterContext twitterCtx)
        {
            byte[] imageBytes = File.ReadAllBytes(@"..\..\Images\200xColor_2.png");

            var user = 
                await twitterCtx.UpdateAccountBackgroundImageAsync(
                    image: imageBytes, 
                    fileName: "200xColor_2.png",
                    imageType: "png", 
                    tile: false, 
                    use: true, 
                    skipStatus: true);

            if (user != null)
                Console.WriteLine("User Image: " + user.ProfileImageUrl); 
        }

        static async Task UpdateAccountProfileAsync(TwitterContext twitterCtx)
        {
            var user = await twitterCtx.UpdateAccountProfileAsync(
                "Joe Mayo",
                "http://linqtotwitter.codeplex.com",
                "Las Vegas, NV",
                "Testing the Account Profile Update with LINQ to Twitter.",
                true,
                true);

            if (user != null)
                Console.WriteLine(
                    "Name: {0}\nURL: {1}\nLocation: {2}\nDescription: {3}",
                    user.Name, user.Url, user.Location, user.Description); 
        }

        static async Task UpdateAccountSettingsAsync(TwitterContext twitterCtx)
        {
            Account acct = 
                await twitterCtx.UpdateAccountSettingsAsync(
                    null, true, 20, 6, null, null);

            if (acct != null && 
                acct.Settings != null && 
                acct.Settings.SleepTime != null)
            {
                SleepTime sleep = acct.Settings.SleepTime;
                Console.WriteLine(
                    "Enabled: {0}, Start: {1}, End: {2}",
                    sleep.Enabled, sleep.StartHour, sleep.EndHour); 
            }
        }

        static async Task UpdateDeliveryDeviceAsync(TwitterContext twitterCtx)
        {
            Account acct = 
                await twitterCtx.UpdateDeliveryDeviceAsync(
                    DeviceType.None, null);

            if (acct != null && acct.User != null)
                Console.WriteLine("Update for: ", acct.User.ScreenNameResponse); 
        }
        
        static async Task UpdateProfileBannerAsync(TwitterContext twitterCtx)
        {
            byte[] fileBytes = File.ReadAllBytes(@"..\..\images\13903749474_86bd1290de_o.jpg");

            var user = 
                await twitterCtx.UpdateProfileBannerAsync(
                    fileBytes, "13903749474_86bd1290de_o.jpg", "jpg", 1500, 500, 0, 0);

            if (user != null)
                Console.WriteLine("User Image: " + user.ProfileBannerUrl); 
        }

        static async Task RemoveProfileBannerAsync(TwitterContext twitterCtx)
        {
            var user = await twitterCtx.RemoveProfileBannerAsync();

            if (user != null)
                Console.WriteLine("Profile Banner: " + user.ProfileBannerUrl ?? "None"); 
        }
    }
}
