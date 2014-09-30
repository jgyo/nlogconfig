// <copyright file="FilteringTargetFactory.cs" company="YoderZone.com">
// Copyright (c) 2014 Gil Yoder. All rights reserved.
// </copyright>
// <author>Gil Yoder</author>
// <date>9/26/2014</date>
// <summary>Implements the filtering target factory class</summary>
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
using System.Diagnostics.Contracts;

using global::NLog.Conditions;
using global::NLog.Targets;
using global::NLog.Targets.Wrappers;

#endregion

/// <summary>
///     A filtering target factory.
/// </summary>
public static class FilteringTargetFactory
{
    #region Static Fields

    /// <summary>
    ///     The filtering target wrapper.
    /// </summary>
    private static FilteringTargetWrapper filteringTargetWrapper;

    #endregion

    #region Public Properties

    /// <summary>
    ///     Gets or sets the condition.
    /// </summary>
    /// <value>
    ///     The condition.
    /// </value>
    public static ConditionExpression Condition
    {
        get
        {
            return FilteringTargetWrapper.Condition;
        }
        set
        {
            Contract.Requires<ArgumentNullException>(value != null);
            FilteringTargetWrapper.Condition = value;
        }
    }

    /// <summary>
    ///     Gets or sets the filtering target wrapper.
    /// </summary>
    /// <value>
    ///     The filtering target wrapper.
    /// </value>
    public static FilteringTargetWrapper FilteringTargetWrapper
    {
        get
        {
            Contract.Ensures(Contract.Result<FilteringTargetWrapper>() != null);
            return filteringTargetWrapper
                   ?? (filteringTargetWrapper = new FilteringTargetWrapper());
        }
        set
        {
            Contract.Requires<ArgumentNullException>(value != null);
            filteringTargetWrapper = value;
        }
    }

    /// <summary>
    ///     Gets or sets the name.
    /// </summary>
    /// <value>
    ///     The name.
    /// </value>
    public static string Name
    {
        get
        {
            return FilteringTargetWrapper.Name;
        }
        set
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(value));
            FilteringTargetWrapper.Name = value;
        }
    }

    /// <summary>
    ///     Gets or sets the wrapper target.
    /// </summary>
    /// <value>
    ///     The wrapper target.
    /// </value>
    public static Target WrapperTarget
    {
        get
        {
            return FilteringTargetWrapper.WrappedTarget;
        }
        set
        {
            Contract.Requires<ArgumentNullException>(value != null);
            FilteringTargetWrapper.WrappedTarget = value;
        }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///     Creates contains condition.
    /// </summary>
    /// <param name="container">
    ///     The container.
    /// </param>
    /// <param name="containee">
    ///     The contained.
    /// </param>
    /// <param name="ignoreCase">
    ///     true to ignore case.
    /// </param>
    /// <returns>
    ///     The new contains condition.
    /// </returns>
    public static string CreateContainsCondition(
        string container,
        string containee,
        bool ignoreCase = true)
    {
        return string.Format("contains('{0}', '{1}', {2})", container, containee,
                             ignoreCase);
    }

    /// <summary>
    ///     Creates ends with condition.
    /// </summary>
    /// <param name="container">
    ///     The container.
    /// </param>
    /// <param name="containee">
    ///     The contained.
    /// </param>
    /// <param name="ignoreCase">
    ///     true to ignore case.
    /// </param>
    /// <returns>
    ///     The new ends with condition.
    /// </returns>
    public static string CreateEndsWithCondition(
        string container,
        string containee,
        bool ignoreCase = true)
    {
        return string.Format("endswith('{0}', '{1}', {2})", container, containee,
                             ignoreCase);
    }

    /// <summary>
    ///     Creates equals 2 condition.
    /// </summary>
    /// <param name="container">
    ///     The container.
    /// </param>
    /// <param name="containee">
    ///     The contained.
    /// </param>
    /// <param name="ignoreCase">
    ///     true to ignore case.
    /// </param>
    /// <returns>
    ///     The new equals 2 condition.
    /// </returns>
    public static string CreateEquals2Condition(
        string container,
        string containee,
        bool ignoreCase = false)
    {
        return string.Format("equals2('{0}', '{1}', {2})", container, containee,
                             ignoreCase);
    }

    /// <summary>
    ///     Creates filtering target wrapper.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <param name="wrappedTarget">
    ///     The wrapped target.
    /// </param>
    /// <param name="condition">
    ///     The condition.
    /// </param>
    /// <returns>
    ///     The new filtering target wrapper.
    /// </returns>
    public static FilteringTargetWrapper CreateFilteringTargetWrapper(
        string name,
        Target wrappedTarget,
        string condition)
    {
        FilteringTargetWrapper = new FilteringTargetWrapper
        {
            Name = name,
            WrappedTarget = wrappedTarget,
            Condition = condition
        };

        return FilteringTargetWrapper;
    }

    /// <summary>
    ///     Creates starts with condition.
    /// </summary>
    /// <param name="container">
    ///     The container.
    /// </param>
    /// <param name="containee">
    ///     The contained.
    /// </param>
    /// <param name="ignoreCase">
    ///     true to ignore case.
    /// </param>
    /// <returns>
    ///     The new starts with condition.
    /// </returns>
    public static string CreateStartsWithCondition(
        string container,
        string containee,
        bool ignoreCase)
    {
        return string.Format("startswith('{0}', '{1}', {2})", container,
                             containee, ignoreCase);
    }

    #endregion
}
}