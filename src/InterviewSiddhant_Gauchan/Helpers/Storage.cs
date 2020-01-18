using Hanssens.Net;

namespace InterviewSiddhant_Gauchan.Helpers
{
    public interface IStorage
    {
        void Store<T>(T credentials,string key);
        T Get<T>(string key);
        void Clear();
    }


    public class Storage : IStorage
    {
        LocalStorage storage = new LocalStorage();
       
        public void Store<T>(T credentials,string key) {
            storage.Store(key, credentials);
        }
        public T Get<T>(string key)
        {
            return storage.Get<T>(key);
        }
        public void Clear()
        {
            storage.Clear();     
            
        }
    }
}
