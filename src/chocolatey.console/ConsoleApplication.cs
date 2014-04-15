﻿namespace chocolatey.console
{
    using System;
    using System.Collections.Generic;
    using chocolatey.infrastructure.app;
    using chocolatey.infrastructure.app.commands;
    using chocolatey.infrastructure.app.configuration;
    using chocolatey.infrastructure.commands;
    using chocolatey.infrastructure.configuration;
    using infrastructure.registration;

    public sealed class ConsoleApplication
    {
        public void run(string[] args, IConfigurationSettings config)
        {
            this.Log().Debug(() => "Passed in arguments: {0}".format_with(string.Join(" ", args)));

            SimpleInjectorContainer.Initialize();

            IList<string> command_args = new List<string>();
            //shift the first arg off 
            int count = 0;
            foreach (var arg in args)
            {
                if (count == 0)
                {
                    count += 1;
                    continue;
                }

                command_args.Add(arg);
            }

            // get the runner you need and go to town
            //load the runners up in the container with their key being the name from commandnametype
            ICommand runner = new ChocolateyInstallCommand();
            //

            ConfigurationOptions.parse_arguments_and_update_configuration(
                command_args,
                config,
                (optionSet) => runner.configure_argument_parser(optionSet, config),
                (unparsedArgs) => runner.handle_unparsed_arguments(unparsedArgs, config),
                () => runner.help_message(config));
            this.Log().Debug(() => "Configuration: {0}".format_with(config.ToString()));

            if (config.HelpRequested)
            {
#if DEBUG
                Console.WriteLine("Press enter to continue...");
                Console.ReadKey();
#endif
                Environment.Exit(-1);
            }
            
            runner.run(config);
        }
    }
}