// <copyright file="SettingsHelper.cs" company="YoderZone.com">
// Copyright (c) 2014 Gil Yoder. All rights reserved.
// </copyright>
// <author>Gil Yoder</author>
// <date>9/26/2014</date>
// <summary>Implements the settings helper class</summary>
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

namespace YoderZone.Extensions.NLog
{
#region Imports

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using global::NLog;
using global::NLog.Config;
using global::NLog.Targets;

#endregion

/// <summary>
///     The NLog settings helper.
/// </summary>
public class SettingsHelper
{
    #region Constants

    /// <summary>
    ///     Name of the application.
    /// </summary>
    private const string constAppName = "NLogConfig";

    /// <summary>
    ///     The company.
    /// </summary>
    private const string constCompany = "YoderZone";

    /// <summary>
    ///     The log file name layout.
    /// </summary>
    private const string constLogFile = "NLogConfig${shortdate}.log";

    /// <summary>
    ///     The class's logger name.
    /// </summary>
    private const string constLogger = "YoderZone.NLogConfig.SettingsHelper";

    /// <summary>
    ///     The constant rule.
    /// </summary>
    private const string constRule = "NLogLogRule";

    /// <summary>
    ///     The class's file target name.
    /// </summary>
    private const string constTarget = "NLogTarget";

    #endregion

    #region Static Fields

    /// <summary>
    ///     The SettingsHelper Logger for logging diagnostic messages.
    /// </summary>
    private static readonly Logger settingsHelperLogger = CreateLogger(
                constLogger);

    private static bool initialized;

    /// <summary>
    ///     A list of all configured instances still in use.
    /// </summary>
    private static Dictionary<string, SettingsHelper> instanceList;

    /// <summary>
    ///     The is n log configuration log enabled.
    /// </summary>
    private static bool isNLogConfigLogEnabled;

    /// <summary>
    ///     The log configuration log level.
    /// </summary>
    /// ###
    /// <remarks>
    ///     Prevent all NLogConfig data from logging by setting this to
    ///     LogLevel.Off. The default value is LogLevel.Info which will
    ///     generally log a little information, unless exceptions are raised
    ///     when used to log other programs. NLogConfig logging can be
    ///     programmatically disabled by calling Setting
    ///     SettingsHelper.IsNLogConfigLogEnabled to false.
    /// </remarks>
    private static LogLevel nLogConfigLogLevel = LogLevel.Off;

    #endregion

    #region Fields

    /// <summary>
    ///     The configuration lock.
    /// </summary>
    private readonly object configLock = new object();

    /// <summary>
    ///     The logging configuration for one application.
    /// </summary>
    private LoggingConfiguration config;

    /// <summary>
    ///     The rules for one application.
    /// </summary>
    private Dictionary<string, LoggingRule> rules;

    /// <summary>
    ///     The targets for one application.
    /// </summary>
    private Dictionary<string, Target> targets;

    #endregion

    #region Constructors and Destructors

    static SettingsHelper()
    {
        ApplicationDataFolder =
            Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData,
                Environment.SpecialFolderOption.None);
    }

    /// <summary>
    ///     Prevents a default instance of the
    ///     YoderZone.NLogConfig.SettingsHelper class from being created.
    ///     Used by CreateLogger to configure static properties within the
    ///     SettingsHelper class.
    /// </summary>
    private SettingsHelper()
    {
        LogManager.ConfigurationChanged += this.LogManager_ConfigurationChanged;
    }

    /// <summary>
    ///     Finalizes an instance of the YoderZone.NLogConfig.SettingsHelper
    ///     class.
    /// </summary>
    ~SettingsHelper()
    {
        this.Close();
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///     Gets the pathname of the application data folder.
    /// </summary>
    /// <value>
    ///     The pathname of the application data folder.
    /// </value>
    public static string ApplicationDataFolder { get; private set; }

    /// <summary>
    ///     Gets a list of SettingsHelpers.
    /// </summary>
    /// <value>
    ///     The list.
    /// </value>
    public static Dictionary<string, SettingsHelper> InstanceList
    {
        get
        {
            return instanceList ?? (instanceList = new
            Dictionary<string, SettingsHelper>());
        }
    }

    /// <summary>
    ///     Gets or sets a value indicating whether a n log configuration log
    ///     is enabled.
    /// </summary>
    /// <value>
    ///     true if a n log configuration log is enabled, false if not.
    /// </value>
    public static bool IsNLogConfigLogEnabled
    {
        get
        {
            return isNLogConfigLogEnabled;
        }
        set
        {
            if (isNLogConfigLogEnabled == value)
            {
                return;
            }

            isNLogConfigLogEnabled = value;

            if (value)
            {
                if (NLogConfigLogLevel == LogLevel.Off)
                {
                    NLogConfigLogLevel = LogLevel.Info;
                    return;
                }

                SetRuleLoggingLevel(constAppName, constRule, NLogConfigLogLevel);
            }
            else
            {
                SetRuleLoggingLevel(constAppName, constRule, LogLevel.Off);
            }

            LogManager.ReconfigExistingLoggers();
        }
    }

    /// <summary>
    ///     Gets or sets the log configuration log level.
    /// </summary>
    /// <value>
    ///     The n log configuration log level.
    /// </value>
    public static LogLevel NLogConfigLogLevel
    {
        get
        {
            return nLogConfigLogLevel;
        }
        set
        {
            // NLogConfigLogLevel setter guard
            if (nLogConfigLogLevel == value)
            {
                return;
            }

            nLogConfigLogLevel = value;

            if (value == LogLevel.Off)
            {
                IsNLogConfigLogEnabled = false;
                return;
            }

            SetRuleLoggingLevel(constAppName, constRule, value);
            if (IsNLogConfigLogEnabled)
            {
                LogManager.ReconfigExistingLoggers();
            }
            else
            {
                IsNLogConfigLogEnabled = true;
            }
        }
    }

    /// <summary>
    ///     Gets or sets the name of the application.
    /// </summary>
    /// <value>
    ///     The name of the application.
    /// </value>
    public string ApplicationName { get; set; }

    /// <summary>
    ///     Gets or sets the name of the company.
    /// </summary>
    /// <value>
    ///     The name of the company.
    /// </value>
    public string CompanyName { get; set; }

    /// <summary>
    ///     Gets or sets the pathname of the log files folder. Used to
    ///     specify a folder name within the application folder. Note this is
    ///     not the full path to the folder; just its name.
    /// </summary>
    /// <value>
    ///     The pathname of the log files folder.
    /// </value>
    public string LogFilesFolder { get; set; }

    /// <summary>
    ///     Gets the full pathname of the log files file.
    /// </summary>
    /// <value>
    ///     The full pathname of the log files file. Built by combining the
    ///     values of the ApplicationDataFolder, CompanyName, ApplicationName,
    ///     and LogFilesFolder properties.
    /// </value>
    public string LogFilesPath
    {
        get
        {
            return Path.Combine(
                       ApplicationDataFolder,
                       this.CompanyName,
                       this.ApplicationName,
                       this.LogFilesFolder);
        }
    }

    #endregion

    #region Properties

    /// <summary>
    ///     Gets a list of log levels.
    /// </summary>
    /// <value>
    ///     A List of log levels.
    /// </value>
    private static IEnumerable<LogLevel> LogLevelList
    {
        get
        {
            yield return LogLevel.Debug;
            yield return LogLevel.Error;
            yield return LogLevel.Fatal;
            yield return LogLevel.Info;
            yield return LogLevel.Trace;
            yield return LogLevel.Warn;
        }
    }

    /// <summary>
    ///     Gets or sets the current LoggingConfiguration object.
    /// </summary>
    /// <value>
    ///     The configuration.
    /// </value>
    private LoggingConfiguration Configuration
    {
        get
        {
            return this.config
                   ?? (this.config =
                           LogManager.Configuration
                           ?? (LogManager.Configuration = new LoggingConfiguration()));
        }
        set
        {
            lock (this.configLock)
            {
                this.config = value;
            }
        }
    }

    /// <summary>
    ///     Gets the rules for one application.
    /// </summary>
    /// <value>
    ///     The rules.
    /// </value>
    private Dictionary<string, LoggingRule> Rules
    {
        get
        {
            return this.rules ?? (this.rules = new Dictionary<string, LoggingRule>());
        }
    }

    /// <summary>
    ///     Gets the targets for one application.
    /// </summary>
    /// <value>
    ///     The targets.
    /// </value>
    private Dictionary<string, Target> Targets
    {
        get
        {
            return this.targets ?? (this.targets = new Dictionary<string, Target>());
        }
    }

    public static bool IsInternalLoggingInitialized
    {
        get
        {
            return initialized;
        }
        set
        {
            initialized = value;
        }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///     A factory method to create a class Logger.
    /// </summary>
    /// <param name="name">
    ///     The name to give the Logger. By default this defaults to the
    ///     caller's type name.
    /// </param>
    /// <returns>
    ///     The new logger.
    /// </returns>
    public static Logger CreateLogger(string name = null)
    {
        return LogManager.GetLogger(name ?? GetNameForLogger());
    }

    /// <summary>
    ///     Gets a name for a logger.
    /// </summary>
    /// <remarks>
    ///     This method is mostly a duplicate of
    ///     LogManger.GetCurrentClassLogger().
    /// </remarks>
    /// <returns>
    ///     The name for a logger.
    /// </returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static string GetNameForLogger()
    {
        int skipFrames = 2;
        Type declaringType;
        string name;

        do
        {
            // Get the method name off the stack skipping the number of frames
            // specified.
            MethodBase method = new StackFrame(skipFrames, false).GetMethod();
            // Get the declaring type of the selected method.
            declaringType = method.DeclaringType;
            // If the declaring type is null, we will settle for the method name.
            if (declaringType == null)
            {
                name = method.Name;
                break;
            }

            // Otherwise we take the full name of the declaring type, but
            // we check to make sure we aren't at mscorlib.dll. If so, we
            // start over skipping an additional frame.
            ++skipFrames;
            name = declaringType.FullName;
        }
        while (declaringType.Module.Name.Equals(
                    "mscorlib.dll",
                    StringComparison.OrdinalIgnoreCase));

        return name;
    }

    public static void InitializeInternalLogging(IEnumerable<LogLevel> levels)
    {
        settingsHelperLogger.Debug("Entered static method.");

        if (initialized)
        {
            settingsHelperLogger.Info("Internal logging already initialized.");
            return;
        }

        initialized = true;

        SettingsHelper configuration = NewConfiguration(constAppName,
                                       constCompany);

        // Set up a FileTarget to log NLogConfig data under AppData\YoderZone\NLogConfig.
        FileTarget fileTarget = FileTargetFactory.CreateFileTarget(
                                    // Name of the target.
                                    constTarget,
                                    // The filename layout.
                                    constLogFile,
                                    // The logger's configuration instance.
                                    configuration);

        configuration.AddTarget(fileTarget, true);

        LoggingRule rule = RuleFactory.CreateRule(constLogger, fileTarget,
                           LogLevel.Off);

        foreach (var logLevel in levels)
        {
            rule.EnableLoggingForLevel(logLevel);
        }

        configuration.AddRule(constRule, rule);

        settingsHelperLogger.Trace("Internal logging initialized.");
        settingsHelperLogger.Trace("Rule: {0}", constRule);
        settingsHelperLogger.Trace("ApplicationDataFolder = {0}",
                                   ApplicationDataFolder);
        settingsHelperLogger.Trace("ApplicationName       = {0}",
                                   configuration.ApplicationName);
        settingsHelperLogger.Trace("CompanyName           = {0}",
                                   configuration.CompanyName);
        settingsHelperLogger.Trace("fileTarget.Layout     = {0}",
                                   fileTarget.Layout);
        settingsHelperLogger.Trace("LogFilesFolder        = {0}",
                                   configuration.LogFilesFolder);
        settingsHelperLogger.Trace("LogFilesPath          = {0}",
                                   configuration.LogFilesPath);
        settingsHelperLogger.Info("*** {0} ***", constAppName);
        settingsHelperLogger.Info("Copyright 2014 Gil Yoder");
        settingsHelperLogger.Info("Licensed under Microsoft Public License (Ms-PL)");
    }

    /// <summary>
    ///     Creates a new NLog configuration.
    /// </summary>
    /// <param name="applicationName">
    ///     Name of the application.
    /// </param>
    /// <param name="companyName">
    ///     Name of the company.
    /// </param>
    /// <param name="logFilesFolder">
    ///     Folder name where o store log files.
    /// </param>
    /// <returns>
    ///     A SettingsHelper.
    /// </returns>
    public static SettingsHelper NewConfiguration(
        string applicationName,
        string companyName,
        string logFilesFolder = "logs")
    {
        if (settingsHelperLogger != null)
        {
            settingsHelperLogger.Debug("Entered static method.");
            settingsHelperLogger.Trace("applicationName: {0}", applicationName);
            settingsHelperLogger.Trace("companyName:     {0}", companyName);
            settingsHelperLogger.Trace("logFilesFolder:  {0}", logFilesFolder);
        }

        var nLogConfiguration = new SettingsHelper
        {
            ApplicationName = applicationName,
            CompanyName = companyName,
            LogFilesFolder = logFilesFolder
        };

        InstanceList.Add(applicationName, nLogConfiguration);
        return nLogConfiguration;
    }

    /// <summary>
    ///     Sets rule level.
    /// </summary>
    /// <param name="applicationName">
    ///     Name of the application.
    /// </param>
    /// <param name="ruleName">
    ///     Name of the rule.
    /// </param>
    /// <param name="logLevel">
    ///     The log level.
    /// </param>
    /// <param name="enable">
    ///     true to enable, false to disable.
    /// </param>
    public static void SetRuleLevel(
        string applicationName,
        string ruleName,
        LogLevel logLevel,
        bool enable)
    {
        settingsHelperLogger.Debug("Entered static method.");

        if (!InstanceList.ContainsKey(applicationName))
        {
            settingsHelperLogger.Info("Configuration instance {0} does not exist.",
                                      applicationName);
            return;
        }

        LoggingRule loggingRule = InstanceList[applicationName].Rules[ruleName];
        if (enable)
        {
            loggingRule.EnableLoggingForLevel(logLevel);
        }
        else
        {
            loggingRule.DisableLoggingForLevel(logLevel);
        }
    }

    /// <summary>
    ///     Sets rule logging level.
    /// </summary>
    /// <param name="applicationName">
    ///     Name of the application.
    /// </param>
    /// <param name="ruleName">
    ///     Name of the rule.
    /// </param>
    /// <param name="logLevel">
    ///     The log level.
    /// </param>
    public static void SetRuleLoggingLevel(
        string applicationName,
        string ruleName,
        LogLevel logLevel)
    {
        LoggingRule loggingRule = InstanceList[applicationName].Rules[ruleName];
        foreach (var item in LogLevelList)
        {
            if (item >= logLevel)
            {
                loggingRule.EnableLoggingForLevel(item);
            }
            else
            {
                loggingRule.DisableLoggingForLevel(item);
            }
        }
    }

    /// <summary>
    ///     Adds a rule with an option to defer the update to the NLog
    ///     configuration.
    /// </summary>
    /// <param name="name">
    ///     The name of the rule.
    /// </param>
    /// <param name="rule">
    ///     The rule.
    /// </param>
    /// <param name="deferUpdate">
    ///     true to defer updating until several rules or targets have been
    ///     configured.
    /// </param>
    public void AddRule(string name, LoggingRule rule,
                        bool deferUpdate = false)
    {
        lock (this.configLock)
        {
            this.Rules.Add(name, rule);
        }

        if (deferUpdate)
        {
            return;
        }

        this.UpdateConfiguration();
    }

    /// <summary>
    ///     Adds a target with an option to defer the update to the NLog
    ///     configuration.
    /// </summary>
    /// <param name="target">
    ///     Target for the.
    /// </param>
    /// <param name="deferUpdate">
    ///     true to defer updating until several rules or targets have been
    ///     configured.
    /// </param>
    public void AddTarget(Target target, bool deferUpdate = false)
    {
        Contract.Requires<ArgumentNullException>(target != null);
        lock (this.configLock)
        {
            this.Targets.Add(target.Name, target);
        }

        if (deferUpdate)
        {
            return;
        }

        this.UpdateConfiguration();
    }

    /// <summary>
    ///     Closes this YoderZone.NLogConfig.SettingsHelper.
    /// </summary>
    public void Close()
    {
        lock (this.configLock)
        {
            LogManager.ConfigurationChanged -= this.LogManager_ConfigurationChanged;

            if (this.rules != null)
            {
                foreach (var loggingRule in
                         this.rules.Where(
                             loggingRule =>
                             this.Configuration.LoggingRules.Contains(loggingRule.Value)))
                {
                    this.Configuration.LoggingRules.Remove(loggingRule.Value);
                }

                this.Rules.Clear();
                this.rules = null;
            }

            if (this.targets == null)
            {
                return;
            }

            foreach (var target in
                     this.targets.Where(
                         target => this.Configuration.ConfiguredNamedTargets.Contains(
                             target.Value)))
            {
                this.config.RemoveTarget(target.Key);
            }

            this.targets.Clear();
            this.targets = null;
            InstanceList.Remove(this.ApplicationName);
            LogManager.ReconfigExistingLoggers();
        }
    }

    /// <summary>
    ///     Gets a rule.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <returns>
    ///     The rule.
    /// </returns>
    public LoggingRule GetRule(string name)
    {
        LoggingRule value;
        return this.Rules.TryGetValue(name, out value) == false ? null : value;
    }

    /// <summary>
    ///     Gets a target.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <returns>
    ///     The target.
    /// </returns>
    public Target GetTarget(string name)
    {
        Target value;
        return this.Targets.TryGetValue(name, out value) == false ? null : value;
    }

    /// <summary>
    ///     Removes the specified rule.
    /// </summary>
    /// <param name="rule">
    ///     The rule to remove.
    /// </param>
    /// <param name="deferUpdate">
    ///     true to defer the update until several changes can be updated
    ///     together.
    /// </param>
    public void RemoveRule(LoggingRule rule, bool deferUpdate = false)
    {
        lock (this.configLock)
        {
            string ruleName = "";
            foreach (var loggingRule in this.rules)
            {
                if (loggingRule.Value == rule)
                {
                    ruleName = loggingRule.Key;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(ruleName))
            {
                this.rules.Remove(ruleName);
            }

            this.config.LoggingRules.Remove(rule);

            if (deferUpdate)
            {
                return;
            }

            this.UpdateConfiguration();
        }
    }

    /// <summary>
    ///     Removes the specified target.
    /// </summary>
    /// <param name="target">
    ///     The target to remove.
    /// </param>
    /// <param name="deferUpdate">
    ///     true to defer the update until several changes can be updated
    ///     together.
    /// </param>
    public void RemoveTarget(Target target, bool deferUpdate)
    {
        Contract.Requires<ArgumentNullException>(target != null);
        lock (this.configLock)
        {
            LoggingConfiguration c = this.Configuration;
            c.RemoveTarget(target.Name);
            this.targets.Remove(target.Name);
        }

        if (deferUpdate)
        {
            return;
        }

        this.UpdateConfiguration();
    }

    /// <summary>
    ///     Removes the target described by name.
    /// </summary>
    /// <param name="name">
    ///     The name of the target to remove.
    /// </param>
    /// <param name="deferUpdate">
    ///     true to defer the update until several changes can be updated
    ///     together.
    /// </param>
    public void RemoveTarget(string name, bool deferUpdate = false)
    {
        this.RemoveTarget(this.targets[name], deferUpdate);
    }

    /// <summary>
    ///     Updates the configuration.
    /// </summary>
    public void UpdateConfiguration()
    {
        lock (this.configLock)
        {
            LoggingConfiguration c = this.Configuration;

            // Insure that the targets and rules are included in the NLog.Configuration.
            foreach (
                var target in this.Targets.Where(target => !c.AllTargets.Contains(
                        target.Value))
            )
            {
                c.AddTarget(target.Value.Name, target.Value);
            }

            foreach (var rule in this.Rules.Where(rule => !c.LoggingRules.Contains(
                    rule.Value)))
            {
                c.LoggingRules.Add(rule.Value);
            }

            LogManager.ReconfigExistingLoggers();
        }
    }

    #endregion

    #region Methods

    /// <summary>
    ///     Event handler. Called by LogManager for configuration changed
    ///     events.
    /// </summary>
    /// <param name="sender">
    ///     Source of the event.
    /// </param>
    /// <param name="e">
    ///     Logging configuration changed event information.
    /// </param>
    private void LogManager_ConfigurationChanged(
        object sender,
        LoggingConfigurationChangedEventArgs e)
    {
        if (e.NewConfiguration == this.Configuration)
        {
            return;
        }

        this.Configuration = e.NewConfiguration;

        this.UpdateConfiguration();
    }

    #endregion
}
}