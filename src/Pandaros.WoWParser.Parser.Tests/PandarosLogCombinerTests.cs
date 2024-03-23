using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pandaros.WoWParser.Parser;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Pandaros.WoWParser.Tests
{
    [TestClass()]
    public class PandarosLogCombinerTests
    {
        [TestMethod()]
        public void PandarosCombinerSetupTest()
        {
            var builder = new ContainerBuilder();
            var logger = new PandaLogger("D:/temp/march2/");
            builder.PandarosParserSetup(logger, logger);

            var Container = builder.Build();

            var clp = Container.Resolve<CombatLogCombiner>();

            logger.Log("Starting Parse.");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            // clp.ParseToEnd(@"D:/logs/");
            sw.Stop();
            logger.Log($"Parsed in {sw.Elapsed}.");
            Thread.Sleep(1000);
        }
    }
}