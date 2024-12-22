namespace Luc.Lwx.Interface;

public class LwxConfigException(
    string message, 
    Exception? ex = null
) : Exception(message, ex)
{
}
