using System;
using System.Threading.Tasks;
using GraceCaching.Application;
using Microsoft.AspNetCore.Mvc;

namespace GraceCaching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnyController : ControllerBase
    {
        private readonly CachingService<ReturnableResult> _cachingService;

        public AnyController(CachingService<ReturnableResult> cachingService)
        {
            _cachingService = cachingService;
        }

        public Task<ReturnableResult> Get([FromQuery] Guid guid)
        {
            return _cachingService.Execute(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(15));
                return new ReturnableResult(guid, DateTime.Now);
            }, guid.ToString());
        }
    }
}