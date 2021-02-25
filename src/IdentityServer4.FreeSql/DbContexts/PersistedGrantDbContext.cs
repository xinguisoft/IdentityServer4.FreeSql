using System;
using System.Threading.Tasks;
using IdentityServer4.FreeSql.Entities;
using IdentityServer4.FreeSql.Interfaces;
using IdentityServer4.FreeSql.Options;
using FreeSql;

namespace IdentityServer4.FreeSql.DbContexts
{
    /// <summary>
    /// DbContext for the IdentityServer operational data.
    /// </summary>
    /// <seealso cref="FreeSql.DbContext" />
    /// <seealso cref="IdentityServer4.FreeSql.Interfaces.IPersistedGrantDbContext" />
    public class PersistedGrantDbContext : PersistedGrantDbContext<PersistedGrantDbContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistedGrantDbContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="storeOptions">The store options.</param>
        /// <exception cref="ArgumentNullException">storeOptions</exception>
        public PersistedGrantDbContext(IFreeSql<PersistedGrantDbContext> freeSql, OperationalStoreOptions storeOptions)
            : base(freeSql, storeOptions)
        {
        }
    }

    /// <summary>
    /// DbContext for the IdentityServer operational data.
    /// </summary>
    /// <seealso cref="FreeSql.DbContext" />
    /// <seealso cref="IdentityServer4.FreeSql.Interfaces.IPersistedGrantDbContext" />
    public class PersistedGrantDbContext<TContext> : DbContext, IPersistedGrantDbContext
        where TContext : DbContext, IPersistedGrantDbContext
    {
        private readonly IFreeSql<PersistedGrantDbContext> freeSql;
        private readonly OperationalStoreOptions storeOptions;
             
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistedGrantDbContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="storeOptions">The store options.</param>
        /// <exception cref="ArgumentNullException">storeOptions</exception>
        public PersistedGrantDbContext(IFreeSql<PersistedGrantDbContext> freeSql, OperationalStoreOptions storeOptions)
            : base(freeSql, null)
        {
            this.freeSql = freeSql;
            if (storeOptions == null) throw new ArgumentNullException(nameof(storeOptions));
            this.storeOptions = storeOptions;
        }

        /// <summary>
        /// Gets or sets the persisted grants.
        /// </summary>
        /// <value>
        /// The persisted grants.
        /// </value>
        public DbSet<PersistedGrant> PersistedGrants { get; set; }

        /// <summary>
        /// Gets or sets the device codes.
        /// </summary>
        /// <value>
        /// The device codes.
        /// </value>
        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

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
            //builder.UseOptions(options);
            base.OnConfiguring(builder);
        }
    }

}
