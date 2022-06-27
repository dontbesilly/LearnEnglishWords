namespace LearnEnglishWords.Exceptions;

public class AlreadyCreatedException : ApplicationException
{
    public AlreadyCreatedException(string message) : base(message)
    {
    }
}