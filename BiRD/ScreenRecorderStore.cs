namespace BiRD
{
    public static class ScreenRecorderStore
    {
        private static ScreenRecorder _screenRecorder;
        public static ScreenRecorder
            GetRecorder()
        {
            if (_screenRecorder == null)
            {
                _screenRecorder = new ScreenRecorder();
            }
            return _screenRecorder;
        }
    }
}
