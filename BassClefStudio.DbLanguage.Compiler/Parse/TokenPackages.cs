﻿using BassClefStudio.DbLanguage.Core.Lifecycle;
using BassClefStudio.DbLanguage.Core.Runtime.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BassClefStudio.DbLanguage.Compiler.Parse
{
    #region Headers

    /// <summary>
    /// Represents the location in a document that a particular token was retrieved from.
    /// </summary>
    public class TokenPos
    {
        /// <summary>
        /// The number of the line in the document.
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        /// The column, or character in the line, of the location in the document.
        /// </summary>
        public int ColumnNumber { get; }

        /// <summary>
        /// Creates a new <see cref="TokenPos"/>.
        /// </summary>
        public TokenPos() { }

        /// <summary>
        /// Creates a new <see cref="TokenPos"/>.
        /// </summary>
        /// <param name="lineNumber">The number of the line in the document.</param>
        /// <param name="columnNumber">The column, or character in the line, of the location in the document.</param>
        public TokenPos(int lineNumber, int columnNumber)
        {
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
        }

        /// <summary>
        /// Creates a new <see cref="TokenPos"/>.
        /// </summary>
        /// <param name="sourcePos">A <see cref="Pidgin.SourcePos"/> collected from a <see cref="Pidgin.Parser"/> that represents the position in the document.</param>
        public TokenPos(Pidgin.SourcePos sourcePos)
        {
            LineNumber = sourcePos.Line;
            ColumnNumber = sourcePos.Col;
        }
    }

    /// <summary>
    /// Represents any named item in the Db code model.
    /// </summary>
    public abstract class TokenChild
    {
        /// <summary>
        /// The <see cref="string"/> name of the <see cref="TokenChild"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// An optional <see cref="TokenPos"/> representing the location in the source code where the text representing this <see cref="TokenChild"/> occurs.
        /// </summary>
        public TokenPos SourcePosition { get; set; }
    }
    
    /// <summary>
    /// Represents a <see cref="TokenChild"/> with an accessibility modifier.
    /// </summary>
    public abstract class TokenAccessible : TokenChild
    {
        /// <summary>
        /// A <see cref="bool"/> indicating whether the <see cref="TokenChild"/> should be publicly accessible.
        /// </summary>
        public bool IsPublic { get; set; }
    }

    #endregion
    #region Packages
    
    /// <summary>
    /// Represens a tokenized <see cref="IPackage"/>.
    /// </summary>
    public class TokenPackage
    {
        /// <summary>
        /// Information about the <see cref="IPackage"/> this <see cref="TokenPackage"/> encodes.
        /// </summary>
        public PackageInfo PackageInfo { get; set; }

        /// <summary>
        /// The <see cref="TokenType"/>s for each defined type in the package.
        /// </summary>
        public IEnumerable<TokenType> Types { get; set; }

        /// <summary>
        /// Creates a new <see cref="TokenPackage"/>.
        /// </summary>
        /// <param name="info">Information about the <see cref="IPackage"/> this <see cref="TokenPackage"/> encodes.</param>
        /// <param name="types">The <see cref="TokenType"/>s for each defined type in the package.</param>
        public TokenPackage(PackageInfo info, IEnumerable<TokenType> types)
        {
            PackageInfo = info;
            Types = types;
        }
    }

    #endregion
    #region Types

    /// <summary>
    /// Represents a <see cref="TokenAccessible"/> header for a type.
    /// </summary>
    public class TokenTypeHeader : TokenAccessible
    {
        /// <summary>
        /// A <see cref="bool"/> indicating whether the <see cref="TokenType"/> this header is attached to should be treated as a type (class) or contract (interface).
        /// </summary>
        public bool IsConcrete { get; set; }

        /// <summary>
        /// A list of <see cref="string"/> names of <see cref="TokenType"/>s this type inherits from.
        /// </summary>
        public IEnumerable<string> InheritsFrom { get; set; }
    }

    /// <summary>
    /// Represents a tokenized type or contract.
    /// </summary>
    public class TokenType
    {
        /// <summary>
        /// The <see cref="TokenTypeHeader"/> containing metadata about the type.
        /// </summary>
        public TokenTypeHeader Header { get; set; }

        /// <summary>
        /// The body of the <see cref="TokenType"/>, containing a number of <see cref="TokenChild"/>s (traditionally properties or methods).
        /// </summary>
        public IEnumerable<TokenChild> Children { get; set; }
    }

    /// <summary>
    /// Represents a tokenized property definition for a <see cref="TokenType"/>.
    /// </summary>
    public class TokenProperty : TokenAccessible
    {
        /// <summary>
        /// The <see cref="string"/> name of the type of the value stored in this property.
        /// </summary>
        public string ValueType { get; set; }
    }

    #endregion
    #region Scripts

    /// <summary>
    /// Represents a tokenized method definition for a <see cref="TokenType"/>.
    /// </summary>
    public class TokenScript : TokenAccessible
    {
        /// <summary>
        /// The <see cref="string"/> name of the type this <see cref="TokenScript"/> returns.
        /// </summary>
        public string ReturnType { get; set; }

        /// <summary>
        /// A collection of <see cref="TokenScript"/> inputs to the <see cref="TokenScript"/>.
        /// </summary>
        public IEnumerable<TokenScriptInput> Inputs { get; set; }

        /// <summary>
        /// A collection of <see cref="TokenCommand"/>s representing the content of the <see cref="TokenScript"/> (i.e. the code that should be executed).
        /// </summary>
        public IEnumerable<TokenCommand> Commands { get; set; }
    }

    /// <summary>
    /// Represents a named, strongly-typed input to a <see cref="TokenScript"/>.
    /// </summary>
    public class TokenScriptInput : TokenChild
    {
        /// <summary>
        /// The <see cref="string"/> name of the data type of this <see cref="TokenScriptInput"/>.
        /// </summary>
        public string Type { get; set; }
    }

    /// <summary>
    /// Represents a tokenized command inside of a <see cref="TokenScript"/>, which can map (not necessarily one-to-one) to compiled <see cref="ICommand"/>s.
    /// </summary>
    public abstract class TokenCommand
    {
        /// <summary>
        /// An optional <see cref="TokenPos"/> representing the location in the source code where the text representing this <see cref="TokenChild"/> occurs.
        /// </summary>
        public TokenPos SourcePosition { get; set; }
    }

    /// <summary>
    /// Represents a tokenized THIS command.
    /// </summary>
    public class ThisTokenCommand : TokenCommand
    { }

    /// <summary>
    /// Represents an equals (=) command token (usually for the SET command).
    /// </summary>
    public class EqualTokenCommand : TokenCommand
    { }

    /// <summary>
    /// Represents a tokenized reference to a path or property.
    /// </summary>
    public class PathTokenCommand : TokenCommand
    {
        /// <summary>
        /// Represents the path of the item to retreive, set, or reference.
        /// </summary>
        public string Path { get; set; }
    }

    /// <summary>
    /// Represents a tokenized EXECUTE command with the given inputs.
    /// </summary>
    public class ExecuteTokenCommand : TokenCommand
    {
        /// <summary>
        /// The given inputs, as <see cref="TokenCommand"/>s which, when compiled, can be run to resolve input objects.
        /// </summary>
        public IEnumerable<TokenCommand> Inputs { get; set; }
    }

    #endregion
}
