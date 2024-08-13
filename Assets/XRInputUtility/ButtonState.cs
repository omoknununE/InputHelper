namespace XRInputUtility
{
    public interface IReadOnlyButton
    {
        public bool Press { get; }
        public bool Click { get; }
        public bool Release { get; }
    }

    public class ButtonState : IReadOnlyButton
    {
        public bool Press { get; private set; }
        public bool Click { get; private set; }
        public bool Release { get; private set; }

        public void Update(bool isPress)
        {
            var prevPress = Press;
            Press = isPress;
            Click = prevPress == false && isPress;
            Release = prevPress && isPress == false;
        }

        public void Copy(IReadOnlyButton other)
        {
            Press = other.Press;
            Click = other.Click;
            Release = other.Release;
        }
    }
}