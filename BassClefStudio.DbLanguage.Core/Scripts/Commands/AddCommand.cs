﻿using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Memory;
using BassClefStudio.DbLanguage.Core.Scripts.Info;
using BassClefStudio.DbLanguage.Core.Scripts.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DbLanguage.Core.Scripts.Commands
{
    /// <summary>
    /// Represents a memory ADD command that adds a new item to the <see cref="IWritableMemoryStack"/>.
    /// </summary>
    public class AddCommand : IActionCommand
    {
        /// <inheritdoc/>
        public CapabilitiesCollection Requiredcapabilities { get; }

        /// <summary>
        /// The name of the variable to be created in memory.
        /// </summary>
        public string VarName { get; }

        /// <summary>
        /// The <see cref="DataType"/> of the variable. All <see cref="DataObject"/> items in this memory location must inherit from this type. (see <seealso cref="DataType.Is(DataType)"/>)
        /// </summary>
        public DataType VarType { get; }

        /// <summary>
        /// Creates a new memory ADD command that adds a new <see cref="MemoryItem"/> to the <see cref="Thread"/>'s <see cref="IWritableMemoryStack"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="MemoryItem"/>.</param>
        /// <param name="varType">The <see cref="DataType"/> of the <see cref="MemoryItem"/>.</param>
        public AddCommand(string name, DataType varType)
        {
            VarName = name;
            VarType = varType;
            Requiredcapabilities = new CapabilitiesCollection();
        }

        /// <inheritdoc/>
        public DataObject Execute(DataObject me, IWritableMemoryStack myStack, CapabilitiesCollection capabilities)
        {
            myStack.Add(new MemoryItem(new MemoryProperty(VarName, VarType)));
            return null;
        }
    }
}