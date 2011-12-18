using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
namespace pq
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["username"] == null || Session["user_id"] == null)
            {
                Response.Redirect("login.aspx");
            }
            if (IsPostBack)
            {
                if (imgFileUpload.HasFile)
                {
                    string connectionstring = "mongodb://localhost";
                    Configuration config = WebConfigurationManager.OpenWebConfiguration("~/");

                    MongoServer server = MongoServer.Create(connectionstring);
                    MongoDatabase pq = server.GetDatabase("pq");
                    MongoCollection<BsonDocument> images = new MongoCollection<BsonDocument>(pq, new MongoCollectionSettings<BsonDocument>(pq, "images"));
                    BsonDocument doc = new BsonDocument {
                        { "owner", Session["user_id"].ToString() },
                        { "mime", imgFileUpload.PostedFile.ContentType },
                        { "ext", imgFileUpload.FileName.Substring(imgFileUpload.FileName.LastIndexOf(".")) },
                        { "size", imgFileUpload.PostedFile.ContentLength },
                        { "title", (String.IsNullOrWhiteSpace(txtTitle.Text) ? null : txtTitle.Text )}
                    };
                    System.Drawing.Image img = System.Drawing.Image.FromStream(imgFileUpload.PostedFile.InputStream);
                    doc.Add("height", img.Height);
                    doc.Add("width", img.Width);
                    images.Insert(doc);
                    
                    string fname = "C:\\mongo\\" + doc["_id"].ToString() + doc["ext"];
                    imgFileUpload.SaveAs(fname);
                    
                    server.Disconnect();
                    Response.Redirect("image.aspx?id=" + doc["_id"].ToString());
                }
            }
            
            lblUsername.Text = Session["username"].ToString();
        }
    }
}
