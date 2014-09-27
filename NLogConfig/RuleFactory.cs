// <copyright file="RuleFactory.cs" company="YoderZone.com">
// Copyright (c) 2014 Gil Yoder. All rights reserved.
// </copyright>
// <author>Gil Yoder</author>
// <date>9/26/2014</date>
// <summary>Implements the rule factory class</summary>
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

namespace YoderZone.NLogConfig
{
#region Imports

using System;
using System.Diagnostics.Contracts;

using NLog;
using NLog.Config;
using NLog.Filters;
using NLog.Targets;

#endregion

/// <summary>
///     An NLog rule factory.
/// </summary>
public class RuleFactory
{
    #region Static Fields

    /// <summary>
    ///     The last created rule.
    /// </summary>
    private static LoggingRule rule;

    #endregion

    #region Public Properties

    /// <summary>
    ///     Gets the rule last created by the factory.
    /// </summary>
    /// <value>
    ///     The rule.
    /// </value>
    /// <remarks>
    ///     The purpose of this property is to provide a way for
    ///     the factory to modify a new rule after it has been
    ///     instantiated. It is valid for a given rule only until
    ///     a new rule is created by the factory.
    /// </remarks>
    public static LoggingRule Rule
    {
        get
        {
            Contract.Ensures(Contract.Result<LoggingRule>() != null);
            return rule;
        }
        private set
        {
            Contract.Requires<ArgumentNullException>(value != null);
            rule = value;
        }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///     Adds a filter.
    /// </summary>
    /// <param name="filter">
    ///     Specifies the filter.
    /// </param>
    public static void AddFilter(Filter filter)
    {
        rule.Filters.Add(filter);
    }

    /// <summary>
    ///     Adds a target.
    /// </summary>
    /// <param name="target">
    ///     Target for the.
    /// </param>
    public static void AddTarget(Target target)
    {
        Rule.Targets.Add(target);
    }

    /// <summary>
    ///     Creates a rule with the given name, target, and maximum logging level.
    /// </summary>
    /// <param name="loggerNamePattern">
    ///     A pattern specifying the logger name.
    /// </param>
    /// <param name="target">
    ///     Target for the rule.
    /// </param>
    /// <param name="minLogLevel">
    ///     The maximum log level.
    /// </param>
    /// <returns>
    ///     The new rule.
    /// </returns>
    public static LoggingRule CreateRule(
        string loggerNamePattern,
        Target target,
        LogLevel minLogLevel)
    {
        return Rule = new LoggingRule(loggerNamePattern, minLogLevel, target);

        Rule = new LoggingRule(loggerNamePattern, target);

        if (LogLevel.Debug >= minLogLevel)
        {
            rule.EnableLoggingForLevel(LogLevel.Debug);
        }
        if (LogLevel.Error >= minLogLevel)
        {
            rule.EnableLoggingForLevel(LogLevel.Error);
        }
        if (LogLevel.Fatal >= minLogLevel)
        {
            rule.EnableLoggingForLevel(LogLevel.Fatal);
        }
        if (LogLevel.Info >= minLogLevel)
        {
            rule.EnableLoggingForLevel(LogLevel.Info);
        }
        if (LogLevel.Trace >= minLogLevel)
        {
            rule.EnableLoggingForLevel(LogLevel.Trace);
        }
        if (LogLevel.Warn >= minLogLevel)
        {
            rule.EnableLoggingForLevel(LogLevel.Warn);
        }
        return rule;
    }

    /// <summary>
    ///     Creates a rule with the given name, and target, with a logging level of
    ///     Trace.
    /// </summary>
    /// <param name="loggerNamePattern">
    ///     A pattern specifying the logger name.
    /// </param>
    /// <param name="target">
    ///     Target for the rule.
    /// </param>
    /// <returns>
    ///     The new rule.
    /// </returns>
    public static LoggingRule CreateRule(string loggerNamePattern,
                                         Target target)
    {
        return CreateRule(loggerNamePattern, target, LogLevel.Trace);
    }

    #endregion
}
}