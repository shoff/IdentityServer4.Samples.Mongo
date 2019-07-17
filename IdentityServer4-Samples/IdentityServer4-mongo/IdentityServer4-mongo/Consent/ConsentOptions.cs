// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace MongoDbIdentityServer.Consent
{
    public class ConsentOptions
    {
        public static bool enableOfflineAccess = true;
        public static string offlineAccessDisplayName = "Offline Access";
        public static string offlineAccessDescription = "Access to your applications and resources, even when you are offline";

        public static readonly string MuchChooseOneErrorMessage = "You must pick at least one permission";
        public static readonly string InvalidSelectionErrorMessage = "Invalid selection";
    }
}
