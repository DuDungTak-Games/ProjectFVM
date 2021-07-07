namespace DuDungTakGames.Input
{
    public interface IInputHandler
    {
        public void OnBeginInput(InputData inputData);
        public void OnInput(InputData inputData);
        public void OnEndInput(InputData inputData);
    }
}