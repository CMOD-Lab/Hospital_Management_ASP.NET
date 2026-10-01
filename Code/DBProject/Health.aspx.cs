using System;
using System.Web;
using System.Web.UI;
using DBProject.Helpers;

namespace DBProject
{
    /// <summary>
    /// Health check endpoint for container liveness/readiness probes.
    /// Accessible at GET /Health.aspx
    /// Returns HTTP 200 with JSON {"status":"healthy"} when the application
    /// and Redis session store are operational.
    /// Returns HTTP 503 with JSON {"status":"unhealthy"} when Redis is unavailable.
    /// </summary>
    public partial class Health : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/json";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            bool redisHealthy = false;
            string redisStatus = "unavailable";

            try
            {
                redisHealthy = RedisSessionHelper.IsHealthy();
                redisStatus = redisHealthy ? "healthy" : "unhealthy";
            }
            catch (Exception ex)
            {
                redisStatus = "error: " + ex.Message;
            }

            bool overallHealthy = redisHealthy;

            if (overallHealthy)
            {
                Response.StatusCode = 200;
                Response.Write("{\"status\":\"healthy\",\"components\":{\"redis\":\"" + redisStatus + "\"}}");
            }
            else
            {
                Response.StatusCode = 503;
                Response.Write("{\"status\":\"unhealthy\",\"components\":{\"redis\":\"" + redisStatus + "\"}}");
            }

            Response.End();
        }
    }
}
