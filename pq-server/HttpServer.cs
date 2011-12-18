using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading;
using System.Text;
using AForge.Imaging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
namespace pq_server
{
    class HttpServer : IDisposable
    {
        private readonly int _maxThreads;
        private readonly HttpListener _listener;
        private readonly Thread _listenerThread;
        private readonly ManualResetEvent _stop, _idle;
        private readonly Semaphore _busy;

        public HttpServer(int maxThreads)
        {
            _maxThreads = maxThreads;
            _stop = new ManualResetEvent(false);
            _idle = new ManualResetEvent(false);
            _busy = new Semaphore(maxThreads, maxThreads);
            _listener = new HttpListener();
            _listenerThread = new Thread(HandleRequests);
        }

        public void Start(int port)
        {
            _listener.Prefixes.Add(String.Format(@"http://+:{0}/", port));
            _listener.Start();
            _listenerThread.Start();
        }

        public void Dispose()
        { Stop(); }

        public void Stop()
        {
            _stop.Set();
            _listenerThread.Join();
            _idle.Reset();

            //aquire and release the semaphore to see if anyone is running, wait for idle if they are.
            _busy.WaitOne();
            if (_maxThreads != 1 + _busy.Release())
                _idle.WaitOne();

            _listener.Stop();
        }

        private void HandleRequests()
        {
            while (_listener.IsListening)
            {
                var context = _listener.BeginGetContext(ListenerCallback, null);

                if (0 == WaitHandle.WaitAny(new[] { _stop, context.AsyncWaitHandle }))
                    return;
            }
        }

        private void ListenerCallback(IAsyncResult ar)
        {
            _busy.WaitOne();
            HttpListenerContext context = null;
            try
            {
                try
                { context = _listener.EndGetContext(ar); }
                catch (HttpListenerException)
                { return; }

                if (_stop.WaitOne(0, false))
                    return;

                Console.WriteLine("{0} {1}", context.Request.HttpMethod, context.Request.RawUrl);



                context.Response.SendChunked = true;

                string connectionstring = "mongodb://localhost";
                string id = context.Request.QueryString.Get("id");
                StreamWriter sw;
                if (String.IsNullOrWhiteSpace(id))
                {
                    sw = new StreamWriter(context.Response.OutputStream);
                    sw.WriteLine("Accept-Ranges: bytes");
                    sw.WriteLine("Content-Length: 0");
                    sw.WriteLine("Content-Type: text/plain");
                    sw.WriteLine();
                    context.Response.Close();
                    throw new ArgumentException("No id!");
                }

                MongoServer server = MongoServer.Create(connectionstring);
                server.Connect();
                MongoDatabase pq = server.GetDatabase("pq");
                MongoCollection<BsonDocument> images = new MongoCollection<BsonDocument>(pq, new MongoCollectionSettings<BsonDocument>(pq, "images"));
                List<IMongoQuery> mqconstraints = new List<IMongoQuery>();
                mqconstraints.Add(Query.EQ("_id", new ObjectId(id)));

                // Check for other params
                int width = -1;
                int height = -1;
                int cropx1 = -1;
                int cropy1 = -1;
                int cropx2 = -1;
                int cropy2 = -1;
                bool constrain = false;
                bool cropPossible = false;
                bool resizePossible = false;
                try
                {
                    width = Convert.ToInt32(String.IsNullOrWhiteSpace(context.Request.QueryString.Get("width")) ? "-1" : context.Request.QueryString.Get("width"));
                    height = Convert.ToInt32(String.IsNullOrWhiteSpace(context.Request.QueryString.Get("height")) ? "-1" : context.Request.QueryString.Get("height"));
                    constrain = (context.Request.QueryString.Get("constrain") == "0" ? false : true);
                    cropx1 = Convert.ToInt32(String.IsNullOrWhiteSpace(context.Request.QueryString.Get("cropx1")) ? "-1" : context.Request.QueryString.Get("cropx1"));
                    cropy1 = Convert.ToInt32(String.IsNullOrWhiteSpace(context.Request.QueryString.Get("cropy1")) ? "-1" : context.Request.QueryString.Get("cropy1"));
                    cropx2 = Convert.ToInt32(String.IsNullOrWhiteSpace(context.Request.QueryString.Get("cropx2")) ? "-1" : context.Request.QueryString.Get("cropx2"));
                    cropy2 = Convert.ToInt32(String.IsNullOrWhiteSpace(context.Request.QueryString.Get("cropy2")) ? "-1" : context.Request.QueryString.Get("cropy2"));
                    if (cropx1 >= 0 && cropy1 >= 0 && cropx2 >= 0 && cropy2 >= 0)
                    {
                        cropPossible = true;
                        mqconstraints.Add(Query.EQ("cropx1", cropx1));
                        mqconstraints.Add(Query.EQ("cropy1", cropy1));
                        mqconstraints.Add(Query.EQ("cropx2", cropx2));
                        mqconstraints.Add(Query.EQ("cropy2", cropy2));
                       
                    }
                    if (width > 0 && height > 0)
                    {
                        resizePossible = true;
                        mqconstraints.Add(Query.EQ("width", width));
                        mqconstraints.Add(Query.EQ("height", height));
                    }
                    if (cropPossible || resizePossible)
                    {
                        mqconstraints.RemoveAt(0);
                    }
                }
                catch (Exception exc)
                {
                    sw = new StreamWriter(context.Response.OutputStream);
                    sw.WriteLine("Accept-Ranges: bytes");
                    sw.WriteLine("Content-Length: 0");
                    sw.WriteLine("Content-Type: text/plain");
                    sw.WriteLine();
                    Console.WriteLine("Constraint Error");
                    context.Response.Close();
                    server.Disconnect();
                    throw exc;
                }
                if (server.State == MongoServerState.Disconnected)
                {
                    throw new InvalidDataException("Invalid data!");
                }
                mqconstraints.Add(Query.EQ("parent", new ObjectId(id)));
                BsonDocument doc = images.FindOne(Query.And(mqconstraints.ToArray()));
                if (doc == null)
                {
                    doc = images.FindOne(Query.EQ("_id", new ObjectId(id)));
                    if (doc==null) 
                    {
                        sw = new StreamWriter(context.Response.OutputStream);
                        sw.WriteLine("Accept-Ranges: bytes");
                        sw.WriteLine("Content-Length: 0");
                        sw.WriteLine("Content-Type: text/plain");
                        sw.WriteLine();
                        context.Response.Close();
                        server.Disconnect();
                        throw new ArgumentException("Invalid id!");
                    }
                    string fromfilename = "C:\\mongo\\" + doc["_id"].ToString() + doc["ext"].ToString();
                    Bitmap result = (Bitmap)Bitmap.FromFile(fromfilename);
                    

                    if (cropPossible)
                    {
                            
                        result = result.Clone(new Rectangle(cropx1, cropy1, cropx2 - cropx1, cropy2 - cropy1), PixelFormat.Format32bppRgb);
                    }

                    if (resizePossible)
                    {
                        int sourceWidth = result.Width;
                        int sourceHeight = result.Height;

                        int destWidth;
                        int destHeight;


                        if (constrain)
                        {
                            float percent = 0;
                            float percentH = ((float)height / (float)sourceHeight);
                            float percentW = ((float)width / (float)sourceWidth);
                            if (sourceHeight > sourceWidth)
                            {
                                percent = percentH;
                            }
                            else
                            {
                                percent = percentW;
                            }
                            destHeight = (int)(sourceHeight * percent);
                            destWidth = (int)(sourceWidth * percent);
                        }
                        else
                        {
                            destWidth = width;
                            destHeight = height;
                        }
                        Bitmap b = new Bitmap(destWidth, destHeight);
                        Graphics g = Graphics.FromImage((System.Drawing.Image)b);

                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(result, 0, 0, destWidth, destHeight);
                        g.Dispose();
                        result = b;
                    }
                    BsonDocument subimage = new BsonDocument { 
                        { "owner", doc["owner"] },
                        { "mime", doc["mime"] },
                        { "ext", doc["ext"] },
                        // add size
                        { "parent", doc["_id"] }
                    };
                    if (resizePossible || cropPossible)
                    {
                        if (resizePossible)
                        {
                            subimage.Add("height", height);
                            subimage.Add("width", width);
                        }
                        if (cropPossible)
                        {
                            subimage.Add("cropx1", cropx1);
                            subimage.Add("cropy1", cropy1);
                            subimage.Add("cropx2", cropx2);
                            subimage.Add("cropy2", cropy2);
                        }
                    }
                    images.Insert(subimage);
                    string imgname = "C:\\mongo\\" + subimage["_id"].ToString() + subimage["ext"].ToString();
                    result.Save(imgname);
                    result.Dispose();
                    long len = new FileInfo(imgname).Length;
                    subimage.Add("size", len);
                    images.Update(Query.EQ("_id", subimage["_id"]), Update.Set("size", len));
                    doc = subimage;
                }

                sw = new StreamWriter(context.Response.OutputStream);
                sw.WriteLine("Accept-Ranges: bytes");
                sw.WriteLine("Content-Length: " + doc["size"].ToString());
                sw.WriteLine("Content-Type: " + doc["mime"].ToString());
                string fname = "C:\\mongo\\" + doc["_id"].ToString() + doc["ext"].ToString();
                if (File.Exists(fname))
                {
                    FileStream fs = new FileStream(fname, FileMode.Open);
                    CopyStream(fs, context.Response.OutputStream);
                    fs.Close();
                }
                Console.WriteLine("done");
                context.Response.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("fuck\n"+e.Message+e.StackTrace);
                context.Response.Close();
            }
            finally
            {
                if (_maxThreads == 1 + _busy.Release())
                    _idle.Set();
            }
        }

        private static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }
    }
}
