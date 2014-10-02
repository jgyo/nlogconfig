// <copyright file="Program.cs" company="YoderZone.com">
// Copyright (c) 2014 Gil Yoder. All rights reserved.
// </copyright>
// <author>Gil Yoder</author>
// <date>9/26/2014</date>
// <summary>Implements the program class</summary>
// <remarks>
// Licensed under the Microsoft Public License (Ms-PL); you may not
// use this file except in compliance with the License. You may obtain a copy
// of the License at
//
// https://remarker.codeplex.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations
// under the License.
// </remarks>
namespace YoderZone.NLogConfig.Test
{
using NLog;

using YoderZone.Extensions.NLog;

/// <summary>
/// A program to test NLogConfig.
/// </summary>
static class Program
{
    /// <summary>
    /// The logger.
    /// </summary>
    private static readonly Logger logger = SettingsHelper.CreateLogger();

    /// <summary>
    /// Main entry-point for this application.
    /// </summary>
    static void Main()
    {
        // Create a SettingsHelper instance by calling SettingsHelper
        // .NewConfiguration, as you see here, for each plug-in, if that
        // is the scenario you are working on, or for each application,
        // where you want to log data. It will help to make sure that
        // changes to the configuration in one plug-in don't interfere
        // with another NLog configuration for another plug-in.
        var newConfiguration = SettingsHelper.NewConfiguration("NLogConfigTest",
                               "YoderZone");

        // You will need to create at least one target and one rule. The factory
        // classes in NLogConfig can help with both.
        var fileTarget = FileTargetFactory.CreateFileTarget("TestLog",
                         "Test${shortdate}.log", newConfiguration);

        // The loggerNamePattern in CreateRule must match the name of the
        // Logger that will save messages for this rule. Usually the logger will
        // be named the same as the class it has been created in. In this test
        // for example the name of the Logger is "YoderZone.NLogConfig.Text.Program."
        // You can give it any name you wish, but remember to configure a pattern
        // to match its name in the rule.
        var loggingRule = RuleFactory.CreateRule("YoderZone.NLogConfig.Test.*",
                          fileTarget);

        // The targets and rules need to be added to the SettingsHelper
        // instance created with the NewConfiguration command. Set the
        // deferUpdate parameter to true for all additions until the last one.
        // When the last addition occurs, the SettingsHelper instance will
        // update NLog so it knows about the new settings.
        newConfiguration.AddTarget(fileTarget, true);
        newConfiguration.AddRule("TestLog", loggingRule);

        // Then use the Logger in the class as you would normally by
        // calling its methods so send logging messages.
        logger.Debug("Debug");
        logger.Error("Error");
        logger.Info("Info");
    }
}
}
