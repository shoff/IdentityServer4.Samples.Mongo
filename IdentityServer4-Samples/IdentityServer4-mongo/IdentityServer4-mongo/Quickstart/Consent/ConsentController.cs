// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace QuickstartIdentityServer.Quickstart.Consent
{
    using System.Threading.Tasks;
    using IdentityServer4.Services;
    using IdentityServer4.Stores;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// This controller processes the consent UI
    /// </summary>
    [SecurityHeaders]
    public class ConsentController : Controller
    {
        private readonly ConsentService consent;

        public ConsentController(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IResourceStore resourceStore,
            ILogger<ConsentController> logger)
        {
            this.consent = new ConsentService(interaction, clientStore, resourceStore, logger);
        }

        /// <summary>
        /// Shows the consent screen
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var vm = await this.consent.BuildViewModelAsync(returnUrl);
            if (vm != null)
            {
                return this.View("Index", vm);
            }

            return this.View("Error");
        }

        /// <summary>
        /// Handles the consent screen post-back
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ConsentInputModel model)
        {
            var result = await this.consent.ProcessConsent(model);

            if (result.IsRedirect)
            {
                return this.Redirect(result.RedirectUri);
            }

            if (result.HasValidationError)
            {
                this.ModelState.AddModelError("", result.ValidationError);
            }

            if (result.ShowView)
            {
                return this.View("Index", result.ViewModel);
            }

            return this.View("Error");
        }
    }
}