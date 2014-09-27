// <copyright file="FileTargetFactory.cs" company="YoderZone.com">
// Copyright (c) 2014 Gil Yoder. All rights reserved.
// </copyright>
// <author>Gil Yoder</author>
// <date>9/26/2014</date>
// <summary>Implements the file target factory class</summary>
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
using System.IO;
using System.Text;

using NLog.Layouts;
using NLog.Targets;

#endregion

/// <summary>
///     A file target factory.
/// </summary>
public class FileTargetFactory
{
    #region Static Fields

    /// <summary>
    ///     The file target.
    /// </summary>
    private static FileTarget fileTarget;

    #endregion

    #region Public Properties

    /// <summary>
    ///     Gets the file target.
    /// </summary>
    /// <value>
    ///     The file target.
    /// </value>
    public static FileTarget FileTarget
    {
        get
        {
            Contract.Ensures(Contract.Result<FileTarget>() != null);
            return fileTarget;
        }
        private set
        {
            Contract.Requires<ArgumentNullException>(value != null);
            fileTarget = value;
        }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///     Creates file target.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <param name="fileNameLayout">
    ///     The file name layout.
    /// </param>
    /// <param name="configuration">
    ///     The configuration.
    /// </param>
    /// <param name="layout">
    ///     The layout.
    /// </param>
    /// <returns>
    ///     The new file target.
    /// </returns>
    public static FileTarget CreateFileTarget(
        string name,
        string fileNameLayout,
        SettingsHelper configuration,
        string layout)
    {
        Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(name));
        Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(
                    fileNameLayout));
        Contract.Requires<ArgumentNullException>(configuration != null);
        Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(layout));
        Contract.Ensures(Contract.Result<FileTarget>() != null);

        string filePathLayout = Path.Combine(configuration.LogFilesPath,
                                             "FILENAME.LOG")
                                .Replace("FILENAME.LOG", fileNameLayout);
        FileTarget = new FileTarget
        {
            FileName = new SimpleLayout(filePathLayout),
            Name = name,
            Encoding = Encoding.UTF8,
            LineEnding = LineEndingMode.CRLF,
            CreateDirs = true
        };

        if (string.IsNullOrEmpty(layout))
        {
            return FileTarget;
        }

        FileTarget.Layout = layout;

        return FileTarget;
    }

    /// <summary>
    ///     Creates file target with a default layout.
    /// </summary>
    /// <param name="name">
    ///     The name of the target.
    /// </param>
    /// <param name="fileNameLayout">
    ///     The file name layout.
    /// </param>
    /// <param name="configuration">
    ///     The configuration.
    /// </param>
    /// <returns>
    ///     The new file target.
    /// </returns>
    public static FileTarget CreateFileTarget(
        string name,
        string fileNameLayout,
        SettingsHelper configuration)
    {
        Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(name));
        Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(
                    fileNameLayout));
        Contract.Requires<ArgumentNullException>(configuration != null);
        Contract.Ensures(Contract.Result<FileTarget>() != null);

        string filePathLayout = Path.Combine(configuration.LogFilesPath,
                                             "FILENAME.LOG")
                                .Replace("FILENAME.LOG", fileNameLayout);
        FileTarget = new FileTarget
        {
            FileName = new SimpleLayout(filePathLayout),
            Name = name,
            Encoding = Encoding.UTF8,
            LineEnding = LineEndingMode.CRLF,
            CreateDirs = true
        };

        return FileTarget;
    }

    #endregion
}
}