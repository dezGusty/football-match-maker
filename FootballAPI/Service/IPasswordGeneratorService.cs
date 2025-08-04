public interface IPasswordGeneratorService
{
    string Generate(int length = 10);
}