/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Piranha.Data;
using System;

namespace Piranha
{
    /// <summary>
    /// The main application api.
    /// </summary>
    public interface IApi : IDisposable
    {
        /// <summary>
        /// Gets the media repository.
        /// </summary>
        /// <returns></returns>
        Repositories.IMediaRepository Media { get; }

        /// <summary>
        /// Gets the page repository.
        /// </summary>
        Repositories.IPageRepository Pages { get; }

        /// <summary>
        /// Gets the page type repository.
        /// </summary>
        Repositories.IPageTypeRepository PageTypes { get; }

        /// <summary>
        /// Gets the param repository.
        /// </summary>
        Repositories.IParamRepository Params { get; }

        /// <summary>
        /// Gets the site repository.
        /// </summary>
        Repositories.ISiteRepository Sites { get; }
    }
}
