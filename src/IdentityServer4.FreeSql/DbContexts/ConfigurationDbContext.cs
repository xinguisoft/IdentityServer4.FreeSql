using FreeSql;
using IdentityServer4.FreeSql.Entities;
using IdentityServer4.FreeSql.Interfaces;
using IdentityServer4.FreeSql.Options;
using System;
using System.Threading.Tasks;

namespace IdentityServer4.FreeSql.DbContexts
{

    /// <summary>
    /// DbContext for the IdentityServer configuration data.
    /// </summary>
    /// <seealso cref="FreeSql.DbContext" />
    /// <seealso cref="IdentityServer4.FreeSql.Interfaces.IConfigurationDbContext" />
    public class ConfigurationDbContext : ConfigurationDbContext<ConfigurationDbContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationDbContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="storeOptions">The store options.</param>
        /// <exception cref="ArgumentNullException">storeOptions</exception>
        public ConfigurationDbContext(IFreeSql<ConfigurationDbContext> freeSql, ConfigurationStoreOptions storeOptions)
            : base(freeSql, storeOptions)
        {
        }
    }

    /// <summary>
    /// DbContext for the IdentityServer configuration data.
    /// </summary>
    /// <seealso cref="Free.DbContext" />
    /// <seealso cref="IdentityServer4.EntityFramework.Interfaces.IConfigurationDbContext" />
    public class ConfigurationDbContext<TContext> : DbContext, IConfigurationDbContext
        where TContext : DbContext, IConfigurationDbContext
    {
        private readonly IFreeSql<ConfigurationDbContext> freeSql;
        private readonly ConfigurationStoreOptions storeOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationDbContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="storeOptions">The store options.</param>
        /// <exception cref="ArgumentNullException">storeOptions</exception>
        public ConfigurationDbContext(IFreeSql<ConfigurationDbContext> freeSql, ConfigurationStoreOptions storeOptions)
            : base(freeSql, null)
        {
            this.freeSql = freeSql;
            this.storeOptions = storeOptions ?? throw new ArgumentNullException(nameof(storeOptions));
        }

        /// <summary>
        /// Gets or sets the clients.
        /// </summary>
        /// <value>
        /// The clients.
        /// </value>
        public DbSet<Client> Clients { get; set; }

        /// <summary>
        /// Gets or sets the clients' CORS origins.
        /// </summary>
        /// <value>
        /// The clients CORS origins.
        /// </value>
        public DbSet<ClientCorsOrigin> ClientCorsOrigins { get; set; }

        /// <summary>
        /// Gets or sets the identity resources.
        /// </summary>
        /// <value>
        /// The identity resources.
        /// </value>
        public DbSet<IdentityResource> IdentityResources { get; set; }

        /// <summary>
        /// Gets or sets the API resources.
        /// </summary>
        /// <value>
        /// The API resources.
        /// </value>
        public DbSet<ApiResource> ApiResources { get; set; }

        /// <summary>
        /// Gets or sets the API scopes.
        /// </summary>
        /// <value>
        /// The API resources.
        /// </value>
        public DbSet<ApiScope> ApiScopes { get; set; }


        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <returns></returns>
        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }


        protected override void OnModelCreating(ICodeFirst codefirst)
        {
            base.OnModelCreating(codefirst);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseFreeSql(orm: freeSql);
            //builder.UseOptions(options: options);
            base.OnConfiguring(builder);
        }

    }

}
