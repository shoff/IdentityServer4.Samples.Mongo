// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace QuickstartIdentityServer.Quickstart.Home
{
    using System.Threading.Tasks;
    using IdentityServer4.Services;
    using Microsoft.AspNetCore.Mvc;

    [SecurityHeaders]
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService interaction;

        public HomeController(IIdentityServerInteractionService interaction)
        {
            this.interaction = interaction;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        /// <summary>
        /// Shows the error page
        /// </summary>
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            // retrieve error details from identityserver
            var message = await this.interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;
            }

            return this.View("Error", vm);
        }
    }
}