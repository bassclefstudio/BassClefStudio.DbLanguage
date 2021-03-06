﻿using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.DbLanguage.Tests.Core
{
    [TestClass]
    public class TypeUnitTests
    {
        #region Inheritance

        [TestMethod]
        public void InheritType()
        {
            DataType p = new DataType("ParentType", new MemoryProperty[0], new MemoryProperty[0]);
            DataType t = new DataType("ChildType", new MemoryProperty[0], new MemoryProperty[0], parentType: p);
            Assert.AreEqual(p, t.ParentType, "Child type does not inherit from parent type.");
        }

        [TestMethod]
        public void InheritTypeWithProperty()
        {
            DataType propType = new DataType("PropType", new MemoryProperty[0], new MemoryProperty[0]);
            MemoryProperty prop = new MemoryProperty("Prop", propType);
            DataType p = new DataType("ParentType", new MemoryProperty[] { prop }, new MemoryProperty[0]);
            DataType t = new DataType("ChildType", new MemoryProperty[0], new MemoryProperty[0], parentType: p);
            Assert.AreEqual(p, t.ParentType, "Child type does not inherit from parent type.");
            Assert.IsTrue(p.PublicProperties.Contains(prop), "Parent type does not contain property.");
            Assert.IsTrue(t.PublicProperties.Contains(prop), "Child type does not contain property.");
        }

        [TestMethod]
        public void IsType()
        {
            DataType t = new DataType("TestType", new MemoryProperty[0], new MemoryProperty[0]);
            Assert.IsTrue(t.Is(t), "Type IS not itself.");
            DataType dT = new DataType("DerivedType", new MemoryProperty[0], new MemoryProperty[0], null, t);
            Assert.IsTrue(dT.Is(t), "Derived type IS not parent type.");
            Assert.IsFalse(t.Is(dT), "Parent type IS derived type.");
        }

        [TestMethod]
        public void ImplementContract()
        {
            DataContract c = new DataContract("TestContract", new MemoryProperty[0], new DataContract[0]);
            DataType t = new DataType("ChildType", new MemoryProperty[0], new MemoryProperty[0], new DataContract[] { c });
            Assert.IsTrue(t.InheritedContracts.Contains(c), "Implementing type does not inherit from contract.");
        }

        [TestMethod]
        public void ImplementContractWithProperty()
        {
            DataType propType = new DataType("PropType", new MemoryProperty[0], new MemoryProperty[0]);
            MemoryProperty prop = new MemoryProperty("Prop", propType);
            DataContract c = new DataContract("TestContract", new MemoryProperty[] { prop }, new DataContract[0]);
            DataType t = new DataType("ChildType", new MemoryProperty[] { prop }, new MemoryProperty[0], new DataContract[] { c });
            Assert.IsTrue(t.InheritedContracts.Contains(c), "Implementing type does not inherit from contract.");
        }

        [TestMethod]
        public void IsContract()
        {
            DataContract c = new DataContract("TestContract", new MemoryProperty[0], new DataContract[0]);
            Assert.IsTrue(c.Is(c), "Contract IS not itself.");
            DataContract dC = new DataContract("DerivedContract", new MemoryProperty[0], new DataContract[] { c });
            Assert.IsTrue(dC.Is(c), "Derived contract IS not parent contract.");
            Assert.IsFalse(c.Is(dC), "Parent contract IS derived contract.");
            DataType t = new DataType("TestType", typeof(string), new MemoryProperty[0], new MemoryProperty[0], new DataContract[] { dC });
            Assert.IsTrue(t.Is(dC), "Implementing type IS not parent contract.");
            Assert.IsTrue(t.Is(c), "Implementing type IS not parent contract's parent.");
            Assert.IsFalse(dC.Is(t), "Contract IS implementing type.");
        }

        #endregion
        #region Properties

        [TestMethod]
        public void MissingPropertyFromContract()
        {
            DataType propType = new DataType("PropType", new MemoryProperty[0], new MemoryProperty[0]);
            MemoryProperty prop = new MemoryProperty("Prop", propType);
            DataContract c = new DataContract("TestContract", new MemoryProperty[] { prop }, new DataContract[0]);
            Assert.ThrowsException<TypePropertyException>(() => new DataType("ChildType", new MemoryProperty[0], new MemoryProperty[0], new DataContract[] { c }));
        }

        [TestMethod]
        public void PrivatePropertyInContract()
        {
            DataType propType = new DataType("PropType", new MemoryProperty[0], new MemoryProperty[0]);
            MemoryProperty prop = new MemoryProperty("Prop", propType);
            DataContract c = new DataContract("TestContract", new MemoryProperty[] { prop }, new DataContract[0]);
            Assert.ThrowsException<TypePropertyException>(() => new DataType("ChildType", new MemoryProperty[0], new MemoryProperty[] { prop }, new DataContract[] { c }));
        }

        [TestMethod]
        public void DuplicateProperty()
        {
            DataType propType = new DataType("PropType", new MemoryProperty[0], new MemoryProperty[0]);
            MemoryProperty prop = new MemoryProperty("Prop", propType);
            Assert.ThrowsException<TypePropertyException>(() => new DataType("ChildType", new MemoryProperty[] { prop, prop }, new MemoryProperty[0]));
        }

        [TestMethod]
        public void DuplicatePrivatePublicProperty()
        {
            DataType propType = new DataType("PropType", new MemoryProperty[0], new MemoryProperty[0]);
            MemoryProperty prop = new MemoryProperty("Prop", propType);
            Assert.ThrowsException<TypePropertyException>(() => new DataType("ChildType", new MemoryProperty[] { prop }, new MemoryProperty[] { prop }));
        }

        [TestMethod]
        public void DuplicateNameProperty()
        {
            DataType propType = new DataType("PropType", new MemoryProperty[0], new MemoryProperty[0]);
            DataType prop2Type = new DataType("Prop2Type", new MemoryProperty[0], new MemoryProperty[0]);
            MemoryProperty prop = new MemoryProperty("Prop", propType);
            MemoryProperty prop2 = new MemoryProperty("Prop", prop2Type);
            Assert.ThrowsException<TypePropertyException>(() => new DataType("ChildType", new MemoryProperty[] { prop, prop2 }, new MemoryProperty[0]));
        }

        #endregion
        #region Binding

        [TestMethod]
        public void CreateObject()
        {
            DataType propType = new DataType("PropType", typeof(string), new MemoryProperty[0], new MemoryProperty[0]);
            DataObject o = new DataObject(propType);
            Assert.IsTrue(o.DataType == propType, "DataType type not set.");
        }

        [TestMethod]
        public void TrySetTypeBinding()
        {
            DataType propType = new DataType("PropType", typeof(string), new MemoryProperty[0], new MemoryProperty[0]);
            DataObject o = new DataObject(propType);
            o.TrySetObject<string>("This is a test");
            Assert.AreEqual("This is a test", o.GetObject<string>(), "Could not store and retreive bound object.");
        }

        [TestMethod]
        public void InvalidTypeBinding()
        {
            DataType propType = new DataType("PropType", typeof(string), new MemoryProperty[0], new MemoryProperty[0]);
            DataObject o = new DataObject(propType);
            Assert.ThrowsException<TypeBindingException>(() => o.TrySetObject<int>(13));
        }

        #endregion
    }
}
