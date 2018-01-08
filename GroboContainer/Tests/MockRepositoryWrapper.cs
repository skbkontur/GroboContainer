using Rhino.Mocks;

namespace Tests
{
    public class MockRepositoryWrapper
    {
        private readonly MockRepository mockRepository;

        public MockRepositoryWrapper()
        {
            mockRepository = new MockRepository();
        }

        public T GetMock<T>()
        {
            var mock = mockRepository.StrictMock<T>();
            mock.Replay();
            return mock;
        }

        public void VerifyAll()
        {
            mockRepository.VerifyAll();
            mockRepository.BackToRecordAll(BackToRecordOptions.None);
            mockRepository.ReplayAll();
        }
    }
}