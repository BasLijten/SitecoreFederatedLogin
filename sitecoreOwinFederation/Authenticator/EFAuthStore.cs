using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataHandler;
using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sitecore.Security.Authentication;
using SitecoreOwinFederator.Pipelines.HttpRequest;
using System.Security.Claims;

/// <summary>
/// This implementation is a literal copy paste from Vittorio Bertocci
/// original source: https://gist.github.com/vibronet/90d4646c273930ff12d4
/// </summary>
namespace SitecoreOwinFederator.Authenticator
{
    /// <summary>
    /// Model for use with the session authentication store. data will be stored into database
    /// </summary>
    public class AuthSessionEntry
    {
        [Key]
        public string Key { get; set; }
        public DateTimeOffset? ValidUntil { get; set; }
        public string TicketString { get; set; }
    }

    public class SQLAuthSessionStoreContext : DbContext
    {
        public SQLAuthSessionStoreContext(string init = "AuthSessionStoreContext")
            : base(init)
        { }
        public DbSet<AuthSessionEntry> Entries { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    public class SqlAuthSessionStoreInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<SQLAuthSessionStoreContext>
    {
    }

    /// <summary>
    /// Session store to be used with the OWIN cookie authentication middleware
    /// </summary>
    public class SqlAuthSessionStore : IAuthenticationSessionStore
    {
        private string _connectionString;
        private TicketDataFormat _formatter;
        private Timer _gcTimer;
        public SqlAuthSessionStore(TicketDataFormat tdf, string cns = "AuthSessionStoreContext")
        {
            _connectionString = cns;
            _formatter = tdf;
            _gcTimer = new Timer(new TimerCallback(GarbageCollect), null, TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(15));
        }

        private void GarbageCollect(object state)
        {
            DateTimeOffset now = DateTimeOffset.Now.ToUniversalTime();
            using (SQLAuthSessionStoreContext _store = new SQLAuthSessionStoreContext(_connectionString))
            {
                foreach (var entry in _store.Entries)
                {
                    var expiresAt = _formatter.Unprotect(entry.TicketString).Properties.ExpiresUtc;
                    if (expiresAt < now)
                    {
                        _store.Entries.Remove(entry);
                    }
                }
                _store.SaveChanges();
            }
        }

        public Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            string key = Guid.NewGuid().ToString();
            using (SQLAuthSessionStoreContext _store = new SQLAuthSessionStoreContext(_connectionString))
            {                
                _store.Entries.Add(new AuthSessionEntry { Key = key, TicketString = _formatter.Protect(ticket), ValidUntil = ticket.Properties.ExpiresUtc });
                _store.SaveChanges();                
            }
            
            return Task.FromResult(key);
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            using (SQLAuthSessionStoreContext _store = new SQLAuthSessionStoreContext(_connectionString))
            {
                AuthSessionEntry myEntry = _store.Entries.FirstOrDefault(a => a.Key == key);
                if (myEntry != null)
                {
                    myEntry.TicketString = _formatter.Protect(ticket);
                }
                else
                {
                    _store.Entries.Add(new AuthSessionEntry { Key = key, TicketString = _formatter.Protect(ticket) });
                }
                _store.SaveChanges();
                
            }
            return Task.FromResult(0);
        }

        public Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            AuthenticationTicket ticket = null;
            using (SQLAuthSessionStoreContext _store = new SQLAuthSessionStoreContext(_connectionString))
            {
                AuthSessionEntry myEntry = _store.Entries.FirstOrDefault(a => a.Key == key);
                if (myEntry != null)
                    ticket = _formatter.Unprotect(myEntry.TicketString);
                return Task.FromResult(ticket);
            }
        }

        public Task RemoveAsync(string key)
        {
            using (SQLAuthSessionStoreContext _store = new SQLAuthSessionStoreContext(_connectionString))
            {
                AuthSessionEntry myEntry = _store.Entries.FirstOrDefault(a => a.Key == key);
                if (myEntry != null)
                {
                    _store.Entries.Remove(myEntry);
                    _store.SaveChanges();
                }                
            }

            return Task.FromResult(0);
        }
    }


}