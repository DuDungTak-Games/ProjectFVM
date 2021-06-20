namespace DuDungTakGames.Input
{
    public interface IBeginInputHandler
    {
        public void OnBeginInput(InputData inputData);
    }

    public interface IInputHandler
    {
        public void OnInput(InputData inputData);
    }

    public interface IEndInputHandler
    {
        public void OnEndInput(InputData inputData);
    }
}