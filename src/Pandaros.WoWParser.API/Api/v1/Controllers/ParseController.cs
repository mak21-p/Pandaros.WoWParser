using Autofac;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pandaros.WoWParser.API.Api.v1.ViewModels;
using Pandaros.WoWParser.API.DomainModels;
using Pandaros.WoWParser.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pandaros.WoWParser.API.Api.v1.Controllers
{
    /// <summary>
    ///     
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    public class ParseController : ControllerBase
    {
        private readonly ILogger<ParseController> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public ParseController(ILogger<ParseController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        ///     Gets a character by Id
        /// </summary>
        /// <remarks>
        ///     Gets a character by Id
        /// </remarks>
        /// <response code="201">The Character information</response>
        [HttpPost, Route("Parse")]
        [MapToApiVersion("1.0")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> Parse(IFormFile file)
        {
            try
            {
                Console.WriteLine("FIRED");
                Console.WriteLine(file.Name);

                // Read the file content
                using var reader = file.OpenReadStream();
                var content = "";
                var pct = 0;
                await Task.Run(() =>
                {
                    var builder = new ContainerBuilder();
                    var logger = new PandaLogger("D:/temp/march2");
                    builder.PandarosParserSetup(logger, logger);
                    var Container = builder.Build();
                    var clp = Container.Resolve<CombatLogParser>();


                    _logger.LogDebug("Starting parse");
                    clp.parseContent(reader);
                    content = logger.LogFile;
                });

                return Ok(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing file");
                return BadRequest("Error parsing file");
            }
        }
    }
}
