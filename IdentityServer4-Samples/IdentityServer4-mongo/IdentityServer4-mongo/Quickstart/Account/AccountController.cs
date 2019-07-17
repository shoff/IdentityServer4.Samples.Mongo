// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace QuickstartIdentityServer.Quickstart.Account
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using IdentityModel;
    using IdentityServer4;
    using IdentityServer4.Events;
    using IdentityServer4.Extensions;
    using IdentityServer4.Services;
    using IdentityServer4.Stores;
    using IdentityServer4.Test;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// This sample controller implements a typical login/logout/provision workflow for local and external accounts.
    /// The login service encapsulates the interactions with the user data store. This data store is in-memory only and cannot be used for production!
    /// The interaction service provides a way for the UI to communicate with identityserver for validation and context retrieval
    /// </summary>
    [SecurityHeaders]
    public class AccountController : Controller
    {
        private readonly TestUserStore users;
        private readonly IIdentityServerInteractionService interaction;
        private readonly AccountService account;
        private readonly IEventService events;


        public AccountController(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider, 
            IEventService events,
            TestUserStore users = null)
        {
            // if the TestUserStore is not in DI, then we'll just use the global users collection
            this.users = users ?? new TestUserStore(TestUsers.users);
            this.interaction = interaction;
            this.account = new AccountService(interaction, clientStore, schemeProvider);
            this.events = events;
        }

        /// <summary>
        /// Show login page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var vm = await this.account.BuildLoginViewModelAsync(returnUrl);

            if (vm.IsExternalLoginOnly)
            {
                // only one option for logging in
                return await this.ExternalLogin(vm.ExternalProviders.First().AuthenticationScheme, returnUrl);
            }

            return this.View(vm);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            if (this.ModelState.IsValid)
            {
                // validate username/password against in-memory store
                if (this.users.ValidateCredentials(model.Username, model.Password))
                {
                    AuthenticationProperties props = null;
                    // only set explicit expiration here if persistent. 
                    // otherwise we reply upon expiration configured in cookie middleware.
                    if (AccountOptions.allowRememberLogin && model.RememberLogin)
                    {
                        props = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.rememberMeLoginDuration)
                        };
                    }

                    // issue authentication cookie with subject ID and username
                    var user = this.users.FindByUsername(model.Username);
                    // issue authentication cookie with subject ID and username
                    await this.HttpContext.SignInAsync(user.SubjectId, user.Username, props);
                    // make sure the returnUrl is still valid, and if yes - redirect back to authorize endpoint
                    if (this.interaction.IsValidReturnUrl(model.ReturnUrl))
                    {
                        return this.Redirect(model.ReturnUrl);
                    }

                    return this.Redirect("~/");
                }

                this.ModelState.AddModelError("", AccountOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            var vm = await this.account.BuildLoginViewModelAsync(model);
            return this.View(vm);
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var vm = await this.account.BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // no need to show prompt
                return await this.Logout(vm);
            }

            return this.View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            // build a model so the logged out page knows what to display
            var vm = await this.BuildLoggedOutViewModelAsync(model.LogoutId);

            if (this.User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await this.HttpContext.SignOutAsync();

                // raise the logout event
                await this.events.RaiseAsync(new UserLogoutSuccessEvent(this.User.GetSubjectId(), this.User.GetDisplayName()));
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = this.Url.Action("Logout", new { logoutId = vm.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return this.SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            return this.View("LoggedOut", vm);
        }

        /// <summary>
        /// initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl)
        {
            returnUrl = this.Url.Action("ExternalLoginCallback", new { returnUrl });

            // windows authentication is modeled as external in the asp.net core authentication manager, so we need special handling
            if (AccountOptions.WindowsAuthenticationSchemes.Contains(provider))
            {
                // but they don't support the redirect uri, so this URL is re-triggered when we call challenge
                if (this.HttpContext.User is WindowsPrincipal)
                {
                    var props = new AuthenticationProperties();
                    props.Items.Add("scheme", this.HttpContext.User.Identity.AuthenticationType);

                    var id = new ClaimsIdentity(provider);
                    id.AddClaim(new Claim(ClaimTypes.NameIdentifier, this.HttpContext.User.Identity.Name));
                    id.AddClaim(new Claim(ClaimTypes.Name, this.HttpContext.User.Identity.Name));

                  
                    await this.HttpContext.SignInAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme, new ClaimsPrincipal(id), props);
                    return this.Redirect(returnUrl);
                }
                else
                {
                    // this triggers all of the windows auth schemes we're supporting so the browser can use what it supports
                    return new ChallengeResult(AccountOptions.WindowsAuthenticationSchemes);
                }
            }
            else
            {
                // start challenge and roundtrip the return URL
                var props = new AuthenticationProperties
                {
                    RedirectUri = returnUrl,
                    Items = { { "scheme", provider } }
                };
                return new ChallengeResult(provider, props);
            }
        }

        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            // read external identity from the temporary cookie
            var info = await this.HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            var tempUser = info?.Principal;
            if (tempUser == null)
            {
                throw new Exception("External authentication error");
            }

            // retrieve claims of the external user
            var claims = tempUser.Claims.ToList();

            // try to determine the unique id of the external user - the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject);
            if (userIdClaim == null)
            {
                userIdClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            }
            if (userIdClaim == null)
            {
                throw new Exception("Unknown userid");
            }

            // remove the user id claim from the claims collection and move to the userId property
            // also set the name of the external authentication provider
            claims.Remove(userIdClaim);
            var provider = info.Properties.Items["scheme"];
            var userId = userIdClaim.Value;

            // check if the external user is already provisioned
            var user = this.users.FindByExternalProvider(provider, userId) ?? 
                this.users.AutoProvisionUser(provider, userId, claims);

            var additionalClaims = new List<Claim>();

            // if the external system sent a session id claim, copy it over
            var sid = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                additionalClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // if the external provider issued an id_token, we'll keep it for signout
            AuthenticationProperties props = null;
            var idToken = info.Properties.GetTokenValue("id_token");
            if (idToken != null)
            {
                props = new AuthenticationProperties();
                props.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
            }

            // issue authentication cookie for user
            await this.HttpContext.SignInAsync(user.SubjectId, user.Username,  provider, props, additionalClaims.ToArray());

            // delete temporary cookie used during external authentication
            await this.HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            string returnUrl;
            
            if (info.Properties.Items.ContainsKey("returnUrl"))
            {
                // validate return URL and redirect back to authorization endpoint
                returnUrl = info.Properties.Items["returnUrl"];
            }
            else
            {
                //If could not retrieve from Properties, get from QueryString
                returnUrl = this.Request.Query["returnUrl"];
            }
          
            if (this.interaction.IsValidReturnUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }

            return this.Redirect("~/");
        }


        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await this.interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.automaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (this.User == null)
            {
                return vm;
            }

            if (this.User.Identity.IsAuthenticated)
            {
                var idp = this.User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await this.HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await this.interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }

    }
}