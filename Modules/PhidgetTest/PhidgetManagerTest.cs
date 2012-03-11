using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhidgetModules;
using Phidgets;
using Phidgets.Events;

namespace PhidgetTest
{
	[TestClass]
	public class PhidgetManagerTest
	{

		[TestMethod]
		public void TestTwoInterfaceKitsAreEqual()
		{
			var k1 = PhidgetManager.Get<InterfaceKit>(throwIfNotAttached: false);

			var k2 = PhidgetManager.Get<InterfaceKit>(throwIfNotAttached: false);

			Assert.AreEqual(k1, k2);

			PhidgetManager.Release<InterfaceKit>(ref k1);
			PhidgetManager.Release<InterfaceKit>(ref k2);
		}

		[TestMethod]
		public void TestTwoInterfaceKitsOneWithEventAreEqual()
		{
			var k1 = PhidgetManager.Get<InterfaceKit>(throwIfNotAttached: false);
			k1.Attach += delegate(object sender, AttachEventArgs e)    
            {        
            };

			var k2 = PhidgetManager.Get<InterfaceKit>(throwIfNotAttached: false);

			Assert.AreEqual(k1, k2);
			Assert.AreSame(k1, k2);

			PhidgetManager.Release<InterfaceKit>(ref k1);
			PhidgetManager.Release<InterfaceKit>(ref k2);
		}

		[TestMethod]
		public void TestOneKitIsOneCount()
		{
			var k1 = PhidgetManager.Get<InterfaceKit>(throwIfNotAttached: false);
			Assert.AreEqual(1, PhidgetManager.GetReferenceCount<InterfaceKit>());

			PhidgetManager.Release<InterfaceKit>(ref k1);
		}

		[TestMethod]
		public void TestTwoKitIsTwoCount()
		{
			var k1 = PhidgetManager.Get<InterfaceKit>(throwIfNotAttached: false);
			var k2 = PhidgetManager.Get<InterfaceKit>(throwIfNotAttached: false);
			Assert.AreEqual(2, PhidgetManager.GetReferenceCount<InterfaceKit>());

			PhidgetManager.Release<InterfaceKit>(ref k1);
			PhidgetManager.Release<InterfaceKit>(ref k2);
		}

		[TestMethod]
		public void TestTwoKitReleaseOneIsOneCount()
		{
			var k1 = PhidgetManager.Get<InterfaceKit>(throwIfNotAttached: false);
			var k2 = PhidgetManager.Get<InterfaceKit>(throwIfNotAttached: false);

			PhidgetManager.Release<InterfaceKit>(ref k1);
			Assert.AreEqual(1, PhidgetManager.GetReferenceCount<InterfaceKit>());

			PhidgetManager.Release<InterfaceKit>(ref k2);
		}

		[TestMethod]
		public void TestOneKitThrowOnNotAttachIsOneCount()
		{
			InterfaceKit k1 = null;
			try
			{
				k1 = PhidgetManager.Get<InterfaceKit>(throwIfNotAttached: true);

				// We shouldn't get this far, since we did, we have to release this otherwise
				// We will break other tests
				PhidgetManager.Release<InterfaceKit>(ref k1);
				Assert.Fail("Shouldn't get here, Interface kit should not be attached for this test");
			}
			catch(InvalidOperationException) { }

			Assert.AreEqual(0, PhidgetManager.GetReferenceCount<InterfaceKit>());

			var k2 = PhidgetManager.Get<InterfaceKit>(throwIfNotAttached: false);

			Assert.AreEqual(1, PhidgetManager.GetReferenceCount<InterfaceKit>());

			PhidgetManager.Release<InterfaceKit>(ref k2);
			Assert.AreEqual(0, PhidgetManager.GetReferenceCount<InterfaceKit>());
		}

		[TestMethod]
		public void TestOneKitReleaseIsNullified()
		{
			var k1 = PhidgetManager.Get<InterfaceKit>(throwIfNotAttached: false);
			var k2 = PhidgetManager.Get<InterfaceKit>(throwIfNotAttached: false);
			PhidgetManager.Release<InterfaceKit>(ref k1);

			Assert.IsNull(k1);
			Assert.AreEqual(1, PhidgetManager.GetReferenceCount<InterfaceKit>());
			Assert.AreNotEqual(k1, k2);

			PhidgetManager.Release<InterfaceKit>(ref k2);
		}
	}
}
