﻿using BassClefStudio.DbLanguage.Core.Documentation;
using BassClefStudio.DbLanguage.Core.Memory;
using BassClefStudio.DbLanguage.Core.Runtime.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Data
{
    /// <summary>
    /// Represents a concrete type of a <see cref="DataObject"/> in the Db runtime.
    /// </summary>
    public class DataType : IType
    {
        #region TypeDefinition

        /// <inheritdoc/>
        public Namespace TypeName { get; }

        /// <summary>
        /// A collection of <see cref="DataContract"/>s that this <see cref="DataType"/> supports.
        /// </summary>
        public List<DataContract> InheritedContracts { get; }

        /// <summary>
        /// The parent <see cref="DataType"/> of this <see cref="DataType"/>, from which this type inherits all properties and fulfilled <see cref="DataContract"/>s.
        /// </summary>
        public DataType ParentType { get; set; }

        #endregion
        #region Constructor

        /// <summary>
        /// Creates a new <see cref="DataType"/> with the given name.
        /// </summary>
        /// <param name="typeName">The name of the <see cref="DataType"/>.</param>
        public DataType(Namespace typeName)
        {
            TypeName = typeName;
            InheritedContracts = new List<DataContract>();
        }

        /// <summary>
        /// Creates a <see cref="DataType"/> and initializes properties and inheritance.
        /// </summary>
        /// <param name="typeName">The name of the <see cref="DataType"/>.</param>
        /// <param name="publicProperties">A collection of <see cref="MemoryProperty"/> items that a <see cref="DataObject"/> of this <see cref="DataType"/> would publicly have available.</param>
        /// <param name="privateProperties">A collection of <see cref="MemoryProperty"/> items that a <see cref="DataObject"/> of this <see cref="DataType"/> would privately have available.</param>
        /// <param name="inheritedContracts">A collection of <see cref="DataContract"/>s that this <see cref="DataType"/> supports.</param>
        /// <param name="parentType">The parent <see cref="DataType"/> of this <see cref="DataType"/>, from which this type inherits all properties and fulfilled <see cref="DataContract"/>s.</param>
        public DataType(Namespace typeName, IEnumerable<MemoryProperty> publicProperties, IEnumerable<MemoryProperty> privateProperties, IEnumerable<DataContract> inheritedContracts = null, DataType parentType = null)
        : this(typeName)
        {
            if (inheritedContracts != null)
            {
                InheritedContracts.AddRange(inheritedContracts);
            }
            ParentType = parentType;

            InitializeProperties(publicProperties, privateProperties);
        }

        /// <summary>
        /// Creates a <see cref="DataType"/> and initializes properties and inheritance.
        /// </summary>
        /// <param name="typeName">The name of the <see cref="DataType"/>.</param>
        /// <param name="boundType">.NET objects that are binded to a <see cref="DataObject"/> can be type-bound to a <see cref="DataType"/> by using the <see cref="BoundType"/> property. All binded objects in <see cref="DataObject"/>s of this <see cref="DataType"/> must inherit from this .NET <see cref="Type"/>.</param>
        /// <param name="publicProperties">A collection of <see cref="MemoryProperty"/> items that a <see cref="DataObject"/> of this <see cref="DataType"/> would publicly have available.</param>
        /// <param name="privateProperties">A collection of <see cref="MemoryProperty"/> items that a <see cref="DataObject"/> of this <see cref="DataType"/> would privately have available.</param>
        /// <param name="inheritedContracts">A collection of <see cref="DataContract"/>s that this <see cref="DataType"/> supports.</param>
        /// <param name="parentType">The parent <see cref="DataType"/> of this <see cref="DataType"/>, from which this type inherits all properties and fulfilled <see cref="DataContract"/>s.</param>
        public DataType(Namespace typeName, Type boundType, IEnumerable<MemoryProperty> publicProperties, IEnumerable<MemoryProperty> privateProperties, IEnumerable<DataContract> inheritedContracts = null, DataType parentType = null)
        : this(typeName, publicProperties, privateProperties, inheritedContracts, parentType)
        {
            if (boundType != null)
            {
                InitializeTypeBinding(boundType);
            }
        }

        #endregion
        #region Memory

        /// <summary>
        /// A <see cref="Script"/> that is run on a <see cref="DataObject"/> instance of this <see cref="DataType"/> when it is initialized.
        /// </summary>
        public Script Constructor { get; }

        /// <summary>
        /// A list of <see cref="MemoryProperty"/> items that a <see cref="DataObject"/> of this <see cref="DataType"/> would publicly have available.
        /// </summary>
        public List<MemoryProperty> PublicProperties { get; private set; }

        /// <summary>
        /// A list of <see cref="MemoryProperty"/> items that a <see cref="DataObject"/> of this <see cref="DataType"/> would privately have available.
        /// </summary>
        public List<MemoryProperty> PrivateProperties { get; private set; }

        /// <summary>
        /// Initializes the <see cref="PublicProperties"/> and <see cref="PrivateProperties"/> lists from the given new <see cref="MemoryProperty"/> objects as well as parent data. Then checks the property lists against all <see cref="DataContract"/>s and expected conditions.
        /// </summary>
        /// <param name="newPublic">The new public <see cref="MemoryProperty"/> objects to add to the <see cref="DataType"/>.</param>
        /// <param name="newPrivate">The new private <see cref="MemoryProperty"/> objects to add to the <see cref="DataType"/>.</param>
        public void InitializeProperties(IEnumerable<MemoryProperty> newPublic, IEnumerable<MemoryProperty> newPrivate)
        {
            PublicProperties = new List<MemoryProperty>();
            PrivateProperties = new List<MemoryProperty>();

            if(ParentType != null)
            {
                PublicProperties.AddRange(ParentType.PublicProperties);
                PrivateProperties.AddRange(ParentType.PrivateProperties);
            }

            PublicProperties.AddRange(newPublic);
            PrivateProperties.AddRange(newPrivate);

            //// Get any properties that have duplicate paths.
            var allProperties = PublicProperties.Concat(PrivateProperties);
            var duplicates = allProperties.Where(p => allProperties.Count(a => a.Key == p.Key) > 1);
            if (duplicates.Any())
            {
                throw new TypePropertyException($"One or more property keys are used more than once in the same enclosing type {this.TypeName}: {string.Join(",", duplicates.Select(d => d.Key).Distinct())}.");
            }

            //// Get any DataContracts that are missing properties on the type.
            var unfulfilled = InheritedContracts.Where(c => !c.GetProperties().All(p => PublicProperties.Contains(p)));
            if(unfulfilled.Any())
            {
                throw new TypePropertyException($"One or more DataContracts are missing required properties on type {this.TypeName}: {string.Join(",", unfulfilled.Select(u => u.TypeName))}.");
            }
        }

        #endregion
        #region Binding

        /// <summary>
        /// A boolean value indicating whether this <see cref="DataType"/> is bound to a .NET <see cref="Type"/>. For more information, see <see cref="BoundType"/>.
        /// </summary>
        public bool HasTypeBinding => BoundType != null;

        /// <summary>
        /// .NET objects that are binded to a <see cref="DataObject"/> can be type-bound to a <see cref="DataType"/> by using the <see cref="BoundType"/> property. All binded objects in <see cref="DataObject"/>s of this <see cref="DataType"/> must inherit from this .NET <see cref="Type"/>.
        /// </summary>
        public Type BoundType { get; private set; }

        /// <summary>
        /// Stores type information about the <see cref="BoundType"/> for use in reflection (see <see cref="IsBoundType(Type)"/>).
        /// </summary>
        private TypeInfo boundTypeInfo;

        /// <summary>
        /// Returns a boolean indicating whether the provided type <paramref name="t"/> inherits from (or is the same as) the <see cref="BoundType"/> of a bound <see cref="DataType"/>.
        /// </summary>
        /// <param name="t">The .NET type to check for inheritance.</param>
        public bool IsBoundType(Type t)
        {
            return t.GetTypeInfo().IsAssignableFrom(boundTypeInfo);
        }

        /// <summary>
        /// Initializes type binding with the given type, setting up required type info for binding to be successful.
        /// </summary>
        /// <param name="boundType">See <see cref="BoundType"/>.</param>
        public void InitializeTypeBinding(Type boundType)
        {
            BoundType = boundType;
            if (HasTypeBinding)
            {
                boundTypeInfo = BoundType.GetTypeInfo();
            }
        }

        #endregion
        #region Inheritance

        /// <inheritdoc/>
        public bool Is(IType other)
        {
            if (other == this)
            {
                return true;
            }
            else if (InheritedContracts.Any(c => c.Is(other)))
            {
                return true;
            }
            else if (ParentType != null && ParentType.Is(other))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }

    /// <summary>
    /// An <see cref="Exception"/> thrown when setting or creating memory properties for a type fails.
    /// </summary>
    public class TypePropertyException : Exception
    {
        /// <inheritdoc/>
        public TypePropertyException() { }
        /// <inheritdoc/>
        public TypePropertyException(string message) : base(message) { }
        /// <inheritdoc/>
        public TypePropertyException(string message, Exception inner) : base(message, inner) { }
    }
    
    /// <summary>
    /// An <see cref="Exception"/> thrown when type binding to .NET encounters an error.
    /// </summary>
    public class TypeBindingException : Exception
    {
        /// <inheritdoc/>
        public TypeBindingException() { }
        /// <inheritdoc/>
        public TypeBindingException(string message) : base(message) { }
        /// <inheritdoc/>
        public TypeBindingException(string message, Exception inner) : base(message, inner) { }
    }
}
