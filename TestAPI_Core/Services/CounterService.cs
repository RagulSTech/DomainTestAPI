namespace TestAPI_Core.Services
{
    public class CounterService
    {
        private int _count;

        public int Increment()
        {
            _count = ++_count;
            return _count;     
        }
    }

}
