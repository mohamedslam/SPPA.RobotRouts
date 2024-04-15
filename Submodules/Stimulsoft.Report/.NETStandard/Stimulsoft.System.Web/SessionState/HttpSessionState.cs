using Microsoft.AspNetCore.Http;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Stimulsoft.System.Web.SessionState
{
    public sealed class HttpSessionState
    {
        #region Fields
        private Microsoft.AspNetCore.Http.HttpContext httpContext;
        private ISession session = null;
        #endregion

        // Convert an object to a byte array
        private static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);

            return ms.ToArray();
        }

        // Convert a byte array to an Object
        private static object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            return binForm.Deserialize(memStream);
        }

        public object this[string name]
        {
            get
            {
                return Get(name);
            }

            set
            {
                Add(name, value);
            }
        }

        public object Add(string name, object value)
        {
            if (session != null)
            {
                session.Set(name, ObjectToByteArray(value));
                return value;
            }
            return null;
        }

        private object Get(string name)
        {
            if (session != null)
            {
                byte[] value;
                session.TryGetValue(name, out value);

                if (value != null)
                    return ByteArrayToObject(value);
            }
            return null;
        }

        public object Remove(string name)
        {
            if (session != null) session.Remove(name);
            return null;
        }

        public HttpSessionState(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            this.httpContext = httpContext;
            try
            {
                this.session = httpContext.Session;
            }
            catch
            {
            }
        }
    }
}
