using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesApi.Testing;

namespace MoviesApi.Tests
{
    [TestClass]
    public class WireTransferValidatorTests
    {
        [TestMethod]
        public void ValidateReturnsErrorWhenInsuffidientFunds()
        {
            Account origin = new Account
            {
                Funds = 0
            };

            Account destination = new Account
            {
                Funds = 0
            };

            decimal amountToTransfer = 5m;

            var service = new WireTransferValidator();
            var response = service.Validate(origin, destination, amountToTransfer);

            Assert.IsFalse(response.IsSuccessful);
            Assert.AreEqual("The origin account does not have enoigh funds available", response.ErrorMessage);
        }

        [TestMethod]
        public void ValidateReturnsSuccessfullOperation()
        {
            Account origin = new Account
            {
                Funds = 7
            };

            Account destination = new Account
            {
                Funds = 0
            };

            decimal amountToTransfer = 5m;

            var service = new WireTransferValidator();
            var response = service.Validate(origin, destination, amountToTransfer);

            Assert.IsTrue(response.IsSuccessful);
        }
    }
}
