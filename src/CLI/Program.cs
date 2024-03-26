// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Autofac;
using McMaster.Extensions.CommandLineUtils;
using Pandaros.WoWParser.Parser;

namespace SubcommandSample
{
    /// <summary>
    /// In this example, subcommands are defined using the builder API.
    /// Defining subcommands is possible by using the return value of app.Command().
    /// </summary>
    class Npm
    {
        public static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "Combat Log Parser",
                Description = "A combat log parser",
            };

            app.HelpOption(inherited: true);

            app.Command("parse", parseCmd => {
                var fileName = parseCmd.Argument("fileName", "Name of File").IsRequired();
                var outputName = parseCmd.Argument("outputName", "Name of Output").IsRequired();

                parseCmd.OnExecute(() =>
                {

                    var builder = new ContainerBuilder();
                    var logger = new PandaLogger(Directory.GetCurrentDirectory(), outputName.Value);
                    builder.PandarosParserSetup(logger, logger);

                    var Container = builder.Build();

                    var clp = Container.Resolve<CombatLogParser>();

                    logger.Log("Starting Parse.");
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    clp.ParseToEnd(Directory.GetCurrentDirectory() + "/" + fileName.Value);
                    sw.Stop();
                    logger.Log($"Parsed in {sw.Elapsed}.");
                    Thread.Sleep(1000);
                    return 1;
                });
            });

            app.Command("config", configCmd =>
            {
                configCmd.OnExecute(() =>
                {
                    Console.WriteLine("Specify a subcommand");
                    configCmd.ShowHelp();
                    return 1;
                });

                configCmd.Command("set", setCmd =>
                {
                    setCmd.Description = "Set config value";
                    var key = setCmd.Argument("key", "Name of the config").IsRequired();
                    var val = setCmd.Argument("value", "Value of the config").IsRequired();
                    setCmd.OnExecute(() =>
                    {
                        Console.WriteLine($"Setting config {key.Value} = {val.Value}");
                    });
                });

                configCmd.Command("list", listCmd =>
                {
                    var json = listCmd.Option("--json", "Json output", CommandOptionType.NoValue);
                    listCmd.OnExecute(() =>
                    {
                        if (json.HasValue())
                        {
                            Console.WriteLine("{\"dummy\": \"value\"}");
                        }
                        else
                        {
                            Console.WriteLine("dummy = value");
                        }
                    });
                });
            });

            app.OnExecute(() =>
            {
                Console.WriteLine("Specify a subcommand");
                app.ShowHelp();
                var builder = new ContainerBuilder();
                var logger = new PandaLogger("/mnt/c/users/maqso/desktop/test/");
                builder.PandarosParserSetup(logger, logger);

                var Container = builder.Build();

                var clp = Container.Resolve<CombatLogParser>();

                logger.Log("Starting Parse.");
                Stopwatch sw = new Stopwatch();
                sw.Start();
                clp.ParseToEnd("/mnt/c/users/maqso/desktop/test/log6.txt");
                sw.Stop();
                logger.Log($"Parsed in {sw.Elapsed}.");
                Console.WriteLine("DONE");
                return 1;
            });

            return app.Execute(args);
        }
    }
}