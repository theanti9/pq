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
    public partial class register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["username"] != null)
            {
                Response.Redirect("Default.aspx");
            }
            if (IsPostBack)
            {
                if (String.IsNullOrWhiteSpace(txtUsername.Text) || String.IsNullOrWhiteSpace(txtPassword.Text) || String.IsNullOrWhiteSpace(txtConfirm.Text))
                {
                    lblRegisterError.Text = "Error: Please fill out the form completely!";
                }
                else
                {
                    if (txtPassword.Text != txtConfirm.Text)
                    {
                        lblRegisterError.Text = "Passwords do not match!";
                    }
                    else
                    {
                        string connectionstring = "mongodb://localhost";
                        MongoServer server = MongoServer.Create(connectionstring);
                        MongoDatabase pq = server.GetDatabase("pq");
                        MongoCollection<BsonDocument> users = new MongoCollection<BsonDocument>(pq, new MongoCollectionSettings<BsonDocument>(pq, "users"));
                        var q = Query.EQ("username", txtUsername.Text);
                        BsonDocument curuser = users.FindOne(q);
                        if (curuser != null)
                        {
                            lblRegisterError.Text = "Username already exists!";
                        }
                        else
                        {

                            BsonDocument doc = new BsonDocument {
                                { "username", txtUsername.Text },
                                { "password", txtPassword.Text }
                            };
                            users.Insert(doc);
                            Response.Redirect("/login.aspx");
                        }
                    }
                }
            }
        }
    }
}