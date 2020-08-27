﻿using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Lifecycle
{
    /// <summary>
    /// Represents a group of associated <see cref="IType"/> definitions, <see cref="Scripts.Script"/>s, and related information, and provides a basis of type info and static <see cref="DataObject"/>s to running <see cref="Scripts.Commands.ICommand"/>s.
    /// </summary>
    public interface ILibrary
    {
        /// <summary>
        /// A collection of <see cref="ILibrary"/> objects which this <see cref="ILibrary"/> relies on.
        /// </summary>
        IEnumerable<ILibrary> DependentLibraries { get; }

        /// <summary>
        /// A collection of the <see cref="IType"/>s that make up the <see cref="ILibrary"/>.
        /// </summary>
        IEnumerable<IType> Definitions { get; }

        /// <summary>
        /// An <see cref="IMemoryGroup"/> generated by the <see cref="ILibrary"/> that contains contextual information, such as static instances of <see cref="IType"/>s.
        /// </summary>
        IMemoryGroup ManagedContext { get; }
    }
}