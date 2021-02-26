using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.FreeSql.Interfaces;
using IdentityServer4.FreeSql.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using FreeSql;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.FreeSql.Stores
{
    /// <summary>
    /// Implementation of IResourceStore thats uses FreeSql.
    /// </summary>
    /// <seealso cref="IdentityServer4.Stores.IResourceStore" />
    public class ResourceStore : IResourceStore
    {
        /// <summary>
        /// The DbContext.
        /// </summary>
        protected readonly IConfigurationDbContext Context;

        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger<ResourceStore> Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceStore"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">context</exception>
        public ResourceStore(IConfigurationDbContext context, ILogger<ResourceStore> logger)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Logger = logger;
        }

        /// <summary>
        /// Finds the API resources by name.
        /// </summary>
        /// <param name="apiResourceNames">The names.</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            if (apiResourceNames == null) throw new ArgumentNullException(nameof(apiResourceNames));

            var query =
              from apiResource in Context.ApiResources
              where apiResourceNames.Contains(apiResource.Name)
              select apiResource;

            var apis = query
                .IncludeMany(x => x.Secrets)
                .IncludeMany(x => x.Scopes)
                .IncludeMany(x => x.UserClaims)
                .IncludeMany(x => x.Properties).NoTracking();

            var result = (await apis.ToListAsync())
                .Where(x => apiResourceNames.Contains(x.Name))
                .Select(x => x.ToModel()).ToArray();

            if (result.Any())
            {
                Logger.LogDebug("Found {apis} API resource in database", result.Select(x => x.Name));
            }
            else
            {
                Logger.LogDebug("Did not find {apis} API resource in database", apiResourceNames);
            }

            return result;
        }

        /// <summary>
        /// Gets API resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var names = scopeNames.ToArray();

            var query =
                from api in Context.ApiResources
                where api.Scopes.AsSelect().Where(x => names.Contains(x.Scope)).Any()
                select api;

            var apis = query
                .IncludeMany(x => x.Secrets)
                .IncludeMany(x => x.Scopes)
                .IncludeMany(x => x.UserClaims)
                .IncludeMany(x => x.Properties);


            var results = (await apis.ToListAsync())
               .Where(api => api.Scopes.Any(x => names.Contains(x.Scope)));
            var models = results.Select(x => x.ToModel()).ToArray();

            Logger.LogDebug("Found {apis} API resources in database", models.Select(x => x.Name));

            return models;
        }


        /// <summary>
        /// Gets identity resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var scopes = scopeNames.ToArray();

            var query =
                from identityResource in Context.IdentityResources
                where scopes.Contains(identityResource.Name)
                select identityResource;

            var resources = query
                .IncludeMany(x => x.UserClaims)
                .IncludeMany(x => x.Properties)
                .NoTracking();

            var results = (await resources.ToListAsync())
                .Where(x => scopes.Contains(x.Name));

            Logger.LogDebug("Found {scopes} identity scopes in database", results.Select(x => x.Name));

            return results.Select(x => x.ToModel()).ToArray();
        }


        /// <summary>
        /// Gets scopes by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var scopes = scopeNames.ToArray();

            var query =
                from scope in Context.ApiScopes
                where scopes.Contains(scope.Name)
                select scope;

            var resources = query
                .IncludeMany(x => x.UserClaims)
                .IncludeMany(x => x.Properties)
                .NoTracking();

            var results = (await resources.ToListAsync())
                .Where(x => scopes.Contains(x.Name));

            Logger.LogDebug("Found {scopes} scopes in database", results.Select(x => x.Name));

            return results.Select(x => x.ToModel()).ToArray();
        }


        /// <summary>
        /// Gets all resources.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<Resources> GetAllResourcesAsync()
        {
            var identity = Context.IdentityResources.Select
                .IncludeMany(x => x.UserClaims)
                .IncludeMany(x => x.Properties);

            var apis = Context.ApiResources.Select
                .IncludeMany(x => x.Secrets)
                .IncludeMany(x => x.Scopes)
                .IncludeMany(x => x.UserClaims)
                .IncludeMany(x => x.Properties).NoTracking();

            var scopes = Context.ApiScopes.Select
              .IncludeMany(x => x.UserClaims)
              .IncludeMany(x => x.Properties)
              .NoTracking();

            var result = new Resources(
                (await identity.ToListAsync()).Select(x => x.ToModel()),
                (await apis.ToListAsync()).Select(x => x.ToModel()),
                (await scopes.ToListAsync()).Select(x=>x.ToModel())
            );

            Logger.LogDebug("Found {scopes} as all scopes, and {apis} as API resources",
                result.IdentityResources.Select(x => x.Name).Union(result.ApiScopes.Select(x => x.Name)),
                result.ApiResources.Select(x => x.Name));

            return result;
        }
    }
}