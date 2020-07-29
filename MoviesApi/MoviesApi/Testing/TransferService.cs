using System;

namespace MoviesApi.Testing
{
    public class TransferService
    {
        private readonly IValidateWireTransfer _validateWireTransfer;

        public TransferService(IValidateWireTransfer validateWireTransfer)
        {
            _validateWireTransfer = validateWireTransfer;
        }

        public void WireTransfer(Account origin, Account destination, decimal amount)
        {
            var state = _validateWireTransfer.Validate(origin, destination, amount);

            if (!state.IsSuccessful)
                throw new ApplicationException(state.ErrorMessage);

            origin.Funds -= amount;
            destination.Funds += amount;

        }
    }
}
