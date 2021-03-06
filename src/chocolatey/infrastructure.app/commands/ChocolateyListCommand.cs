﻿// Copyright © 2011 - Present RealDimensions Software, LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
//
// You may obtain a copy of the License at
//
// 	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace chocolatey.infrastructure.app.commands
{
    using System.Collections.Generic;
    using attributes;
    using commandline;
    using configuration;
    using domain;
    using infrastructure.commands;
    using logging;
    using services;

    [CommandFor(CommandNameType.list)]
    [CommandFor(CommandNameType.search)]
    public sealed class ChocolateyListCommand : ICommand
    {
        private readonly IChocolateyPackageService _packageService;

        public ChocolateyListCommand(IChocolateyPackageService packageService)
        {
            _packageService = packageService;
        }

        public void configure_argument_parser(OptionSet optionSet, ChocolateyConfiguration configuration)
        {
            optionSet
                .Add("s=|source=",
                     "Source - Source location for install. Can include special 'webpi'. Defaults to sources.",
                     option => configuration.Sources = option.remove_surrounding_quotes())
                .Add("l|lo|localonly|local-only",
                     "LocalOnly - Only search against local machine items.",
                     option => configuration.ListCommand.LocalOnly = option != null)
                .Add("pre|prerelease",
                     "Prerelease - Include Prereleases? Defaults to false.",
                     option => configuration.Prerelease = option != null)
                .Add("p|includeprograms|include-programs",
                     "IncludePrograms - Used in conjunction with LocalOnly, filters out apps chocolatey has listed as packages and includes those in the list. Defaults to false.",
                     option => configuration.ListCommand.IncludeRegistryPrograms = option != null)
                .Add("a|all|allversions|all-versions",
                     "AllVersions - include results from all versions.",
                     option => configuration.AllVersions = option != null)
                ;
            //todo exact name
        }

        public void handle_additional_argument_parsing(IList<string> unparsedArguments, ChocolateyConfiguration configuration)
        {
            configuration.Input = string.Join(" ", unparsedArguments);

            if (configuration.ListCommand.LocalOnly)
            {
                configuration.Sources = ApplicationParameters.PackagesLocation;
                configuration.Prerelease = true;
            }
        }

        public void handle_validation(ChocolateyConfiguration configuration)
        {
        }

        public void help_message(ChocolateyConfiguration configuration)
        {
            this.Log().Info(ChocolateyLoggers.Important, "List/Search Command");
            this.Log().Info(@"
Chocolatey will perform a search for a package local or remote. Some 
 may prefer to use `clist` as a shortcut for `choco list`.
");

            "chocolatey".Log().Info(ChocolateyLoggers.Important, "Usage");
            "chocolatey".Log().Info(@"
    choco search <filter> [<options/switches>]
    choco list <filter> [<options/switches>]
    clist <filter> [<options/switches>]
");

            "chocolatey".Log().Info(ChocolateyLoggers.Important, "Examples");
            "chocolatey".Log().Info(@"
    choco list --local-only
    choco list -lp
    choco list -lap
    choco search git
    choco search git -s ""https://somewhere/out/there""

");

            "chocolatey".Log().Info(ChocolateyLoggers.Important, "Options and Switches");
        }

        public void noop(ChocolateyConfiguration configuration)
        {
            _packageService.list_noop(configuration);
        }

        public void run(ChocolateyConfiguration configuration)
        {
            _packageService.list_run(configuration, logResults: true);
        }
    }
}
