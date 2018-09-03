using Rhino.Mocks;

namespace GroboContainer.Tests
{
    public class MockRepositoryWrapper
    {
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

        private readonly MockRepository mockRepository;
    }
}