using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
namespace pq
{
    public partial class image : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Params.HasKeys())
            {
                try
                {

                    string id = Request.Params.GetValues("id")[0].ToString();
                    if (id == null)
                    {
                        lblTitle.Text = "Error no: id given!";
                    }
                    else
                    {
                        string connectionstring = "mongodb://localhost";
                        MongoServer server = MongoServer.Create(connectionstring);
                        MongoDatabase pq = server.GetDatabase("pq");
                        MongoCollection<BsonDocument> images = new MongoCollection<BsonDocument>(pq, new MongoCollectionSettings<BsonDocument>(pq, "images"));
                        var q = Query.EQ("_id", new ObjectId(id));
                        BsonDocument image = images.FindOne(q);
                        if (image.Contains("title"))
                        {
                            lblTitle.Text = image["title"].ToString();
                        }
                        lblImageId.Text = image["_id"].ToString();
                        lblOwner.Text = image["owner"].ToString();
                        lblMime.Text = image["mime"].ToString();
                        lblSize.Text = image["size"].ToString();
                        string url = "http://localhost:8085/?id=" + image["_id"].ToString();
                        if (image["height"] > 400 || image["width"] > 400)
                        {
                            url += "&width=400&height=400";
                        }
                        imgImage.ImageUrl = url;
                        server.Disconnect();
                    }
                }
                catch (Exception ex)
                {
                    lblTitle.Text = "Error no: id given!";
                }
            }

        }
    }
}