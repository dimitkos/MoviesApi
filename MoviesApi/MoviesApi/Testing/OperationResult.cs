namespace MoviesApi.Testing
{
    public class OperationResult
    {
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }

        public OperationResult(bool isSuccessful, string errorMessage = null)
        {
            IsSuccessful = isSuccessful;
            ErrorMessage = errorMessage;
        }
    }
}
