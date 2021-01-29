using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace core_api.Controllers
{
    [Route("")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        IDatabase db;
        public ValuesController()
        {
            string endpoint = Environment.GetEnvironmentVariable("endpoint");

            ConfigurationOptions options = new ConfigurationOptions
            {
                EndPoints =
                {
                    {endpoint, 6379 }
                },
                ConnectTimeout = 10000,
                SyncTimeout = 10000,
                KeepAlive = 10000
            };

            ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(options);
            db = conn.GetDatabase(0);

            conn.ConfigurationChanged += (s, e) => {
                Console.WriteLine($"Configuration changed: {e.EndPoint}" +
                    $" Configuration: {((ConnectionMultiplexer)s).Configuration} ClientName:{((ConnectionMultiplexer)s).ClientName}");
            };

            conn.ConfigurationChangedBroadcast += (s, e) => {
                Console.WriteLine($"ConfigurationChangedBroadcast changed: {e.EndPoint}" +
                    $" Configuration: {((ConnectionMultiplexer)s).Configuration} ClientName:{((ConnectionMultiplexer)s).ClientName}");
            };

            conn.ConnectionFailed += (s, e) => {
                Console.WriteLine($"ConnectionFailed Endpoint:{e.EndPoint} ConnectionType:{e.ConnectionType} Exception:{e.Exception} FailureType:{e.FailureType}" +
                    $" Configuration:{((ConnectionMultiplexer)s).Configuration} ClientName:{((ConnectionMultiplexer)s).ClientName}");
            };

            conn.ConnectionRestored += (s, e) => {
                Console.WriteLine($"ConnectionRestored Endpoint:{e.EndPoint} ConnectionType:{e.ConnectionType} Exception:{e.Exception} FailureType:{e.FailureType}" +
                    $" Configuration:{((ConnectionMultiplexer)s).Configuration} ClientName:{((ConnectionMultiplexer)s).ClientName}");
            };

            conn.ErrorMessage += (s, e) => {
                Console.WriteLine($"ErrorMessage Endpoint:{e.EndPoint} Message:{e.Message}" +
                    $" Configuration:{((ConnectionMultiplexer)s).Configuration} ClientName:{((ConnectionMultiplexer)s).ClientName}");
            };

            conn.HashSlotMoved += (s, e) => {
                Console.WriteLine($"HashSlotMoved HashSlot:{e.HashSlot} NewEndPoint:{e.NewEndPoint} OldEndPoint:{e.OldEndPoint}" +
                    $" Configuration:{((ConnectionMultiplexer)s).Configuration} ClientName:{((ConnectionMultiplexer)s).ClientName}");
            };

            conn.InternalError += (s, e) => {
                Console.WriteLine($"InternalError ConnectionType:{e.ConnectionType} EndPoint:{e.EndPoint} Exception:{e.Exception} Origin:{e.Origin}" +
                    $" Configuration:{((ConnectionMultiplexer)s).Configuration} ClientName:{((ConnectionMultiplexer)s).ClientName}");
            };

        }

        [HttpGet("hit")]
        //VERB hit
        public ActionResult<string> hit()
        {
            var inc = db.StringIncrement("hit");
            return Request.Method + " hit INC = " + inc.ToString();
        }

        [HttpGet("key")]
        //VERB key?key=X
        public ActionResult<string> key([FromQuery] string key)
        {
            var inc = db.StringGet(key);
            return Request.Method + $" {key} = " + inc;
        }

        [HttpPost("key")]
        //VERB key?key=X&value=Y
        public ActionResult<string> add([FromQuery] string key, [FromQuery] string value)
        {
            var inc = db.StringSet(key, value);
            return Request.Method + " Added " + key + ":"+value+" = " + inc;
        }
    }
}
