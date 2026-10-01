using System;
using System.Web;
using System.Configuration;
using StackExchange.Redis;

namespace DBProject.Helpers
{
    /// <summary>
    /// Helper class that provides Redis-backed session access to replace
    /// in-process ASP.NET Session (InProc) for container-safe stateless operation.
    /// The Redis connection string is read from the REDIS_CONNECTION_STRING
    /// environment variable (injected via Azure Key Vault CSI Driver / AKS Workload Identity).
    /// </summary>
    public static class RedisSessionHelper
    {
        private static readonly Lazy<ConnectionMultiplexer> _lazyConnection =
            new Lazy<ConnectionMultiplexer>(() =>
            {
                // Read connection string from environment variable (injected by Azure Key Vault CSI Driver)
                string redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");

                if (string.IsNullOrEmpty(redisConnectionString))
                {
                    // Fallback to appSettings for local development
                    redisConnectionString = ConfigurationManager.AppSettings["RedisConnectionString"];
                }

                if (string.IsNullOrEmpty(redisConnectionString))
                {
                    throw new InvalidOperationException(
                        "Redis connection string is not configured. " +
                        "Set the REDIS_CONNECTION_STRING environment variable " +
                        "(injected via Azure Key Vault CSI Driver on AKS).");
                }

                return ConnectionMultiplexer.Connect(redisConnectionString);
            });

        private static ConnectionMultiplexer Connection => _lazyConnection.Value;

        private static IDatabase RedisDb => Connection.GetDatabase();

        /// <summary>
        /// Returns a session-scoped Redis key using the ASP.NET session ID.
        /// </summary>
        private static string BuildKey(string sessionId, string key)
        {
            return $"session:{sessionId}:{key}";
        }

        /// <summary>
        /// Gets the current ASP.NET session ID from the HTTP context.
        /// </summary>
        private static string GetSessionId()
        {
            var context = HttpContext.Current;
            if (context == null || context.Session == null)
                throw new InvalidOperationException("HttpContext or Session is not available.");
            return context.Session.SessionID;
        }

        /// <summary>
        /// Sets a value in Redis for the current session.
        /// </summary>
        public static void Set(string key, string value, TimeSpan? expiry = null)
        {
            string sessionId = GetSessionId();
            string redisKey = BuildKey(sessionId, key);
            TimeSpan ttl = expiry ?? TimeSpan.FromMinutes(20);
            RedisDb.StringSet(redisKey, value, ttl);
        }

        /// <summary>
        /// Gets a string value from Redis for the current session.
        /// Returns null if the key does not exist.
        /// </summary>
        public static string Get(string key)
        {
            string sessionId = GetSessionId();
            string redisKey = BuildKey(sessionId, key);
            RedisValue value = RedisDb.StringGet(redisKey);
            return value.IsNullOrEmpty ? null : (string)value;
        }

        /// <summary>
        /// Gets an integer value from Redis for the current session.
        /// Returns 0 if the key does not exist or cannot be parsed.
        /// </summary>
        public static int GetInt(string key)
        {
            string raw = Get(key);
            if (raw == null) return 0;
            return int.TryParse(raw, out int result) ? result : 0;
        }

        /// <summary>
        /// Removes a key from Redis for the current session.
        /// </summary>
        public static void Remove(string key)
        {
            string sessionId = GetSessionId();
            string redisKey = BuildKey(sessionId, key);
            RedisDb.KeyDelete(redisKey);
        }

        /// <summary>
        /// Checks whether the Redis connection is healthy.
        /// </summary>
        public static bool IsHealthy()
        {
            try
            {
                RedisDb.Ping();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
