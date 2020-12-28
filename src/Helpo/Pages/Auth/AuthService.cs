using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Centron.Host.Messages;
using Centron.Host.RestRequests;
using Centron.Interfaces.Administration.Connections;
using CentronSoftware.Centron.WebServices.Connections;
using Helpo.Common;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Raven.Client.Documents;

namespace Helpo.Pages.Auth
{
    public class AuthService
    {
        private readonly CentronWebService _centronWebService;
        private readonly IConfiguration _configuration;
        private readonly IDocumentStore _documentStore;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthService(CentronWebService centronWebService, IConfiguration configuration, IDocumentStore documentStore, AuthenticationStateProvider authenticationStateProvider)
        {
            Guard.NotNull(centronWebService, nameof(centronWebService));
            Guard.NotNull(configuration, nameof(configuration));
            Guard.NotNull(documentStore, nameof(documentStore));
            Guard.NotNull(authenticationStateProvider, nameof(authenticationStateProvider));
            
            this._centronWebService = centronWebService;
            this._configuration = configuration;
            this._documentStore = documentStore;
            this._authenticationStateProvider = authenticationStateProvider;
        }
        
        public async Task<string> LoginAsync(string? username, string? password)
        {
            // First try logging in as a normal user, if Domain login is required, the web-service will handle it already
            var loginRequest = new LoginRequest
            {
                Username = username,
                Password = password,
                Application = this._configuration.GetValue<string>("CentronWebService:ApplicationGuid"),
                Device = Environment.MachineName,
                AppVersion = this.GetType().Assembly.GetName().Version?.ToString() ?? "1.0.0.0",
                LoginKind = WebLoginType.User
            };

            var userLoginResponse = await this._centronWebService.CallAsync(f => f.Login(new Request<LoginRequest>
            {
                Data = loginRequest
            }));

            if (userLoginResponse.Status == StatusCode.Success)
                return userLoginResponse.Result.Single();

            // If that doesn't work, try as a web-account
            loginRequest.LoginKind = WebLoginType.Customer;

            var webAccountLoginResponse = await this._centronWebService.CallAsync(f => f.Login(new Request<LoginRequest> {Data = loginRequest}));

            if (webAccountLoginResponse.Status == StatusCode.Success)
                return webAccountLoginResponse.Result.Single();
            
            // If that doesn't work, throw an exception
            throw new Exception(userLoginResponse.Message);
        }

        public async Task<List<Claim>> GetClaims(string ticket)
        {
            var commonLoginInformationsResult = await this._centronWebService.CallAsync(f => f.GetCommonLoginInformations(new Request {Ticket = ticket}));
            
            if (commonLoginInformationsResult.Status == StatusCode.Failed)
                throw new Exception(commonLoginInformationsResult.Message);

            var loginInformations = commonLoginInformationsResult.Result.Single();

            var name = loginInformations.Firstname + " " + loginInformations.Lastname;
            var appUserI3D = loginInformations.AppUserI3D;
            var contactPersonI3D = loginInformations.ContactPersonI3D;
            
            var user = User.Create(name, appUserI3D, contactPersonI3D);

            using var session = this._documentStore.OpenAsyncSession();

            // TODO: Replace in the future with compare-exchange
            var existingUser = await session.Query<User>()
                .Where(f => f.AppUserI3D == user.AppUserI3D && f.ContactPersonI3D == user.ContactPersonI3D)
                .FirstOrDefaultAsync();
            
            if (existingUser is null)
            {
                await session.StoreAsync(user);
                existingUser = user;
            }
            else
            {
                existingUser.Name = user.Name; // Update Name
            }
            
            await session.SaveChangesAsync();

            await this._centronWebService.CallAsync(f => f.Logout(new Request {Ticket = ticket}));

            return new List<Claim>
            {
                new Claim("HelpoId", existingUser.Id),
                new Claim(ClaimTypes.Name, existingUser.Name),
                //new Claim(ClaimTypes.Role, string.Empty), //TODO: Implement for Admins/Doc-Writers/etc
            };
        }

        public async Task<LoggedInUser?> GetLoggedInUser()
        {
            var state = await this._authenticationStateProvider.GetAuthenticationStateAsync();
            var user = state.User;

            if (user == null)
                return null;

            return new LoggedInUser(user);
        }
    }
}