using FreeSql;
using IdentityServer4.FreeSql.Interfaces;
using IdentityServer4.FreeSql.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.FreeSql.Stores
{
    public class ClientStore : IClientStore
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        protected readonly IConfigurationDbContext Context;

        /// <summary>
        /// 日志
        /// </summary>
        protected readonly ILogger<ClientStore> Logger;

        /// <summary>
        /// 初始化一个 <参阅 cref="ClientStore"/> 类的新实例.
        /// </summary>
        /// <param name="context">数据库上下文</param>
        /// <param name="logger">日志</param>
        /// <exception cref="ArgumentNullException">context</exception>
        public ClientStore(IConfigurationDbContext context, ILogger<ClientStore> logger)
        {
            Context = context ?? throw new ArgumentNullException(paramName: nameof(context));
            Logger = logger;
        }

        /// <summary>
        /// 通过客户端标识查找客户端
        /// </summary>
        /// <param name="clientId">客户端标识</param>
        /// <returns>客户端</returns>
        public virtual async Task<Client> FindClientByIdAsync(string clientId)
        {
            ISelect<Entities.Client> baseQuery = Context.Clients
               .Where(x => x.ClientId == clientId).Take(1);

            var client = (await baseQuery.ToListAsync())
                .SingleOrDefault(x => x.ClientId == clientId);
            if (client == null) return null;
           
            await baseQuery.IncludeMany(x => x.AllowedCorsOrigins).ToListAsync();
            await baseQuery.IncludeMany(x => x.AllowedGrantTypes).ToListAsync();
            await baseQuery.IncludeMany(x => x.AllowedScopes).ToListAsync();
            await baseQuery.IncludeMany(x => x.Claims).ToListAsync();
            await baseQuery.IncludeMany(x => x.ClientSecrets).ToListAsync();
            await baseQuery.IncludeMany(x => x.IdentityProviderRestrictions).ToListAsync();
            await baseQuery.IncludeMany(x => x.PostLogoutRedirectUris).ToListAsync();
            await baseQuery.IncludeMany(x => x.Properties).ToListAsync();
            await baseQuery.IncludeMany(x => x.RedirectUris).ToListAsync();

            var model = client.ToModel();

            Logger.LogDebug("{clientId} found in database: {clientIdFound}", clientId, model != null);

            return model;           
        }
    }
}
