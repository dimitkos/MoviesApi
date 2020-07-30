using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MoviesApi.Testing;
using System;

namespace MoviesApi.Tests.UnitTests
{
    [TestClass]
    public class TransferServiceTest
    {
        [TestMethod]
        public void WireTranserWithInsufficientFundsThrowsAnError()
        {
            //Preparation
            Account origin = new Account
            {
                Funds = 0
            };

            Account destination = new Account
            {
                Funds = 0
            };

            decimal amountToTransfer = 5m;

            var mockValidateWireTransfer = new Mock<IValidateWireTransfer>();
            mockValidateWireTransfer.Setup(x => x.Validate(origin, destination, amountToTransfer)).Returns(new OperationResult(false, "error message"));

            var service = new TransferService(mockValidateWireTransfer.Object);
            Exception expectedException = null;

            //Testing
            try
            {
                service.WireTransfer(origin, destination, amountToTransfer);
            }
            catch (Exception ex)
            {

                expectedException = ex;
            }

            //Verification

            if (expectedException == null)
            {
                Assert.Fail("");
            }

            Assert.IsTrue(expectedException is ApplicationException);
            Assert.AreEqual("error message", expectedException.Message);
        }

        [TestMethod]
        public void WireTranserCorreectlyEditFunds()
        {
            //Preparation
            Account origin = new Account
            {
                Funds = 10
            };

            Account destination = new Account
            {
                Funds = 5
            };

            decimal amountToTransfer = 7m;

            var mockValidateWireTransfer = new Mock<IValidateWireTransfer>();
            mockValidateWireTransfer.Setup(x => x.Validate(origin, destination, amountToTransfer)).Returns(new OperationResult(true));
            var service = new TransferService(mockValidateWireTransfer.Object);

            //Testing

            service.WireTransfer(origin, destination, amountToTransfer);

            //Verification
            Assert.AreEqual(3, origin.Funds);
            Assert.AreEqual(12, destination.Funds);
        }
    }
}
