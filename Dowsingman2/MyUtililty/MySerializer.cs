using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Dowsingman2.MyUtility
{
    class MySerializer
    {
        // 排他ロックに使うSemaphoreSlimオブジェクト
        // （プロセス間の排他が必要なときはSemaphoreオブジェクトに変える）
        static System.Threading.SemaphoreSlim _semaphore
          = new System.Threading.SemaphoreSlim(1, 1);

        // シリアライズする
        public static async Task SerializeAsync<T>(T data, string filePath)
        {
            await _semaphore.WaitAsync(); // ロックを取得する
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                using (var streamWriter = new StreamWriter(filePath, false, new UTF8Encoding(false)))
                {
                    await Task.Run(() => xmlSerializer.Serialize(streamWriter, data));
                    await streamWriter.FlushAsync();  // .NET Framework 4.5以降
                }
            }
            finally
            {
                _semaphore.Release(); // ロックを解放する
            }
        }

        public static void Serialize<T>(T data, string filePath)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            using (var streamWriter = new StreamWriter(filePath, false, new UTF8Encoding(false)))
            {
                xmlSerializer.Serialize(streamWriter, data);
            }
        }

        // デシリアライズする
        public static async Task<T> DeserializeAsync<T>(string filePath)
        {
            await _semaphore.WaitAsync(); // ロックを取得する
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                var xmlSettings = new System.Xml.XmlReaderSettings()
                {
                    CheckCharacters = false,
                };
                using (var streamReader = new StreamReader(filePath, new UTF8Encoding(false)))
                using (var xmlReader = System.Xml.XmlReader.Create(streamReader, xmlSettings))
                {
                    return await Task.Run(() => (T)xmlSerializer.Deserialize(xmlReader));
                }
            }
            finally
            {
                _semaphore.Release(); // ロックを解放する
            }
        }

        public static T Deserialize<T>(string filePath)
        {

            var xmlSerializer = new XmlSerializer(typeof(T));
            var xmlSettings = new System.Xml.XmlReaderSettings()
            {
                CheckCharacters = false,
            };
            using (var streamReader = new StreamReader(filePath, new UTF8Encoding(false)))
            using (var xmlReader = System.Xml.XmlReader.Create(streamReader, xmlSettings))
            {
                return (T)xmlSerializer.Deserialize(xmlReader);
            }
        }
    }
}
