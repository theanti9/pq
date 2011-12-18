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
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                if (String.IsNullOrWhiteSpace(txtUsername.Text) || String.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    lblLoginError.Text = "Missing username or password";
                }
                else
                {

                    string connectionstring = "mongodb://localhost";
                    MongoServer server = MongoServer.Create(connectionstring);
                    MongoDatabase pq = server.GetDatabase("pq");
                    MongoCollection<BsonDocument> users = new MongoCollection<BsonDocument>(pq, new MongoCollectionSettings<BsonDocument>(pq, "users"));
                    var query = Query.EQ("username", txtUsername.Text);
                    BsonDocument u = users.FindOne(query);
                    if (u == null)
                    {
                        lblLoginError.Text = "Error: No user found";
                        server.Disconnect();
                    }
                    else if (u.Contains("username") && u.Contains("password"))
                    {
                        if (u["username"] != txtUsername.Text || u["password"] != txtPassword.Text)
                        {
                            lblLoginError.Text = "Error: Invalid user/password combination";
                            server.Disconnect();
                            //Response.End();
                        }
                        else
                        {
                            if (u.Contains("user_id"))
                            {
                                Session.Add("username", u["username"].ToString());
                                Session.Add("user_id", u["_id"].ToString());
                                lblLoginError.Text = "Login Successful!";
                                server.Disconnect();
                                Response.Redirect("Default.aspx");
                            }
                            else
                            {
                                lblLoginError.Text = "Unknown login error";
                                server.Disconnect();
                                //Response.End();
                            }
                        }
                    }
                    else
                    {
                        lblLoginError.Text = "Error: User not found";
                        server.Disconnect();
                        //Response.End();
                    }
                    //Response.End();
                }
            }
        }
    }
}