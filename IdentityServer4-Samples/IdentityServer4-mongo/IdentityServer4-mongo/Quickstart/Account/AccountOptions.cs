// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace QuickstartIdentityServer.Quickstart.Account
{
    using System;

    public class AccountOptions
    {
        public static bool allowLocalLogin = true;
        public static bool allowRememberLogin = true;
        public static TimeSpan rememberMeLoginDuration = TimeSpan.FromDays(30);

        public static bool showLogoutPrompt = true;
        public static bool automaticRedirectAfterSignOut = false;

        public static bool windowsAuthenticationEnabled = true;
        // specify the Windows authentication schemes you want to use for authentication
        public static readonly string[] WindowsAuthenticationSchemes = new[] { "Negotiate", "NTLM" };
        public const string windowsAuthenticationDisplayName = "Windows";

        public const string InvalidCredentialsErrorMessage = "Invalid username or password";
    }
}
